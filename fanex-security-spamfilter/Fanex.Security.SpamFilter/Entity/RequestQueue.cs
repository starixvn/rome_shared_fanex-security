// --------------------------------------------------------------------------
// <copyright file="RequestQueue.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   
// </history>

namespace Fanex.Security.SpamFilter.Entity
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Threading;

    using Fanex.Security.SpamFilter.Factory;
    using Fanex.Security.SpamFilter.Repository;

    /// <summary>
    /// Represent a queue of requests.
    /// </summary>
    internal class RequestQueue : IRequestQueue<IRequest>
    {
        private const string BlockerSuffix = "/Blocker";

        /// <summary>
        /// One billion.
        /// </summary>
        private const int MaxCount = int.MaxValue / 2;

        private const string RequestTokenSuffix = "/Token";

        /// <summary>
        /// Initializes a new instance of the <see cref="RequestQueue"/> class.
        /// </summary>
        /// <param name="name">Queue name.</param>
        /// <param name="key">Queue item key.</param>
        /// <param name="rule">Rule element.</param>
        /// <param name="blocker">Blocker object.</param>
        /// <param name="repository">Repository object.</param>
        internal RequestQueue(string name, string key, RuleElement rule, IRequestBlocker blocker, IPersistentRepository repository)
        {
            this.Queue = new Queue();
            this.Name = name;

            this.Interval = TimeSpan.FromMinutes(rule.Minutes);
            this.Threshold = rule.Threshold;
            this.Key = key;

            this.Repository = repository;
            this.ThresholdOverflow = false;
            this.Repository.OnPreDispose += new EventHandler(this.Repository_OnPreDispose);

            string token = this.GetToken();
            if (string.IsNullOrEmpty(token))
            {
                this.NewToken();
            }

            this.Blocker = blocker;

            // Init remote blocker
            this.SetBlocker();

            // Init remote count
            this.Repository.Increment(this.Key, 0);
        }

        /// <summary>
        /// Gets a value indicating whether maximum number of requests over Threshold in a TimeSpan time.
        /// </summary>
        public bool Blocked
        {
            get
            {
                // Performance boots
                // Check remote storage each 5s only
                if (this.Blocker.Blocked &&
                    DateTime.UtcNow.CompareTo(this.LastChecked.AddSeconds(5)) < 0)
                {
                    return this.Blocker.Blocked;
                }

                this.LastChecked = DateTime.UtcNow;

                // Remove expired item, check expired blocker
                this.Synchronize();

                // Update local threshold overflow
                this.ThresholdOverflow = this.RemoteTotal >= (ulong)this.Threshold;

                // For explicit blocker (has blockDuration != 0)
                if (this.ExplicitBlocker)
                {
                    this.ExplicitBlockerCheck();
                }
                else
                {
                    // For implicit blocker
                    this.ImplicitBlockerCheck();
                }

                return this.Blocker.Blocked;
            }
        }

        /// <summary>
        /// Gets a value indicating current blocker.
        /// </summary>
        public IRequestBlocker Blocker
        {
            get;
            private set;
        }

        public int Count
        {
            get
            {
                return this.Queue.Count;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating remote storage count.
        /// </summary>
        public ulong RemoteTotal
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets a value indicating time period that limit max count.
        /// </summary>
        public TimeSpan Interval
        {
            get;
            set;
        }

        public bool IsSynchronized
        {
            get
            {
                return this.Queue.IsSynchronized;
            }
        }

        public string Key
        {
            get;
            set;
        }

        public DateTime LastChecked
        {
            get;
            set;
        }

        public string Name
        {
            get;
            set;
        }

        public IPersistentRepository Repository
        {
            get;
            private set;
        }

        public object SyncRoot
        {
            get
            {
                return this.Queue.SyncRoot;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating maximum number of request.
        /// </summary>
        public int Threshold
        {
            get;
            set;
        }

        internal Queue Queue
        {
            get;
            set;
        }

        private string BlockerKey
        {
            get
            {
                return this.Key + BlockerSuffix;
            }
        }

        private bool ExplicitBlocker
        {
            get
            {
                return this.Blocker.Duration.Minutes != 0;
            }
        }

        private bool ThresholdOverflow
        {
            get;
            set;
        }

        private string TokenKey
        {
            get
            {
                return this.Name + RequestTokenSuffix;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating token to group request and mark request as obsolete (user manually call reset queue).
        /// </summary>
        private string TokenValue
        {
            get;
            set;
        }

        /// <summary>
        /// Clear all items in queue and reset other properties.
        /// </summary>
        public void Clear()
        {
            this.Unblock();
            this.LastChecked = DateTime.UtcNow;

            this.Queue.Clear();
            this.Repository.Zero(this.Key);

            this.SetBlocker();

            this.RemoteTotal = 0;
            this.ThresholdOverflow = false;

            this.NewToken();
        }

        public bool Contains(IRequest request)
        {
            return this.Queue.Contains(request);
        }

        public void CopyTo(Array array, int index)
        {
            this.Queue.CopyTo(array, index);
        }

        public IRequest Dequeue()
        {
            var request = this.Queue.Dequeue() as IRequest;
            return request;
        }

        public void Dispose()
        {
        }

        /// <summary>
        /// Add an item to queue and update other relevant properties.
        /// </summary>
        /// <param name="item">Item to enqueue.</param>
        public void Enqueue(IRequest item)
        {
            item.Token = this.TokenValue;

            this.Queue.Enqueue(item);

            // Avoid consume too much memory, do not need to update repository when threshold overflow
            if (this.Count > this.Threshold)
            {
                // Rolling
                this.Queue.Dequeue();
                return;
            }
            else
            {
                this.RemoteTotal = this.Repository.Increment(this.Key, item.Weight, this.Interval.Minutes + this.Blocker.Duration.Minutes);
            }
        }

        /// <summary>
        /// Loop through last item, check whether it is expired and take it away queue, sync/update blocker from remote.
        /// </summary>
        public void Synchronize()
        {
            var now = DateTime.UtcNow;

            // Avoid error when server flush
            var remoteTotal = this.Repository.Get(this.Key, typeof(ulong));
            this.RemoteTotal = remoteTotal == null ? 0 : (ulong)remoteTotal;

            this.GetToken();
            this.GetBlocker();

            ulong delta = 0;
            var item = this.Peek();

            while (item == null && this.Count > 0)
            {
                item = this.Dequeue();
            }

            while (item != null && this.Count > 0)
            {
                if (item.Token != this.TokenValue || // New token
                    now.CompareTo(item.RequestTime.AddMinutes(this.Interval.Minutes)) > 0)
                {
                    // Expired
                    if (item.Token == this.TokenValue)
                    {
                        // Decrement when item is expired only
                        delta += item.Weight;
                    }

                    item = this.Queue.Dequeue() as IRequest;
                }
                else
                {
                    break;
                }
            }

            if (delta != 0)
            {
                this.RemoteTotal = this.Repository.Decrement(this.Key, delta);
            }
        }

        public IEnumerator GetEnumerator()
        {
            return this.Queue.GetEnumerator();
        }

        /// <summary>
        /// Get current token from repository.
        /// </summary>
        /// <returns>Return current token.</returns>
        public string GetToken()
        {
            string token = this.Repository.Get(this.TokenKey, typeof(string)) + string.Empty;
            if (!string.IsNullOrEmpty(token))
            {
                this.TokenValue = token;
            }

            return token;
        }

        /// <summary>
        /// Get current blocker object from repository.
        /// </summary>
        /// <returns>Return current blocker.</returns>
        public IRequestBlocker GetBlocker()
        {
            try
            {
                var blocker = this.Repository.Get(this.BlockerKey, this.Blocker.GetType());
                if (blocker != null)
                {
                    this.Blocker = (IRequestBlocker)blocker;
                }
            }
            catch
            {
                // Blocker is wrong, try to reset it
                this.SetBlocker();
            }

            return this.Blocker;
        }

        /// <summary>
        /// Create new token for repository.
        /// </summary>
        public void NewToken()
        {
            this.TokenValue = Guid.NewGuid().ToString();
            this.Repository.Set(this.TokenKey, this.TokenValue);
        }

        public IRequest Peek()
        {
            // This operation doesn't change queue content so no need to update to remote repository
            var item = (this.Count == 0) ? null : this.Queue.Peek() as IRequest;
            return item;
        }

        /// <summary>
        /// Local block.
        /// </summary>
        private void Block()
        {
            this.Blocker.Blocked = true;
            this.Blocker.BlockedTime = DateTime.UtcNow;
        }

        /// <summary>
        /// Check Blocked status for explicit counter.
        /// </summary>
        private void ExplicitBlockerCheck()
        {
            // Is blocked
            if (this.Blocker.Blocked)
            {
                // Has been expired
                if (this.Blocker.Expired)
                {
                    // Reset queue and remote storage
                    this.Clear();
                }
            }
            else
            {
                // Hasn't been blocked
                // Threshold is overflowed
                if (this.ThresholdOverflow)
                {
                    this.Block();
                    this.SetBlocker();
                }
            }
        }

        /// <summary>
        /// Check Blocked status for implicit counter.
        /// </summary>
        private void ImplicitBlockerCheck()
        {
            // Is blocked
            if (this.Blocker.Blocked)
            {
                // Threshold has not overflowed
                if (!this.ThresholdOverflow)
                {
                    this.Unblock();
                    this.SetBlocker();
                }
            }
            else
            {
                // Hasn't been blocked
                // Threshold is overflowed
                if (this.ThresholdOverflow)
                {
                    this.Block();
                    this.SetBlocker();
                }
            }
        }

        private void Repository_OnPreDispose(object sender, EventArgs e)
        {
            this.Clear();
        }

        private void SetBlocker()
        {
            this.Repository.Set(this.BlockerKey, this.Blocker, this.Interval.Minutes + this.Blocker.Duration.Minutes);
        }

        /// <summary>
        /// Local unblock.
        /// </summary>
        private void Unblock()
        {
            this.Blocker.Blocked = false;
            this.Blocker.BlockedTime = DateTime.MinValue.ToUniversalTime();
        }
    }
}
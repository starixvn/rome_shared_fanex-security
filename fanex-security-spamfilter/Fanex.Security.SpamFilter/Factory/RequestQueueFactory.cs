// --------------------------------------------------------------------------
// <copyright file="RequestQueueFactory.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   
// </history>

namespace Fanex.Security.SpamFilter.Factory
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Fanex.Security.SpamFilter.Entity;
    using Fanex.Security.SpamFilter.Repository;

    /// <summary>
    /// Create and manage request queue in memory, singleton for each request id.
    /// </summary>
    internal static class RequestQueueFactory
    {
        /// <summary>
        /// One billion.
        /// </summary>
        private const int MaxItem = int.MaxValue / 2;

        private static IDictionary<string, IRequestQueue<IRequest>> cache = new Dictionary<string, IRequestQueue<IRequest>>();
        private static object locker = new object();
        private static Task flusher = null;

        static RequestQueueFactory()
        {
            // Keep all queues in sync with remote storage
            flusher = Task.Factory.StartNew(
            () =>
            {
                while (true)
                {
                    if (flusher == null)
                    {
                        continue;
                    }

                    foreach (IRequestQueue<IRequest> item in cache.Values)
                    {
                        if (DateTime.UtcNow.CompareTo(item.LastChecked.AddSeconds(10)) > 0)
                        {
                            try
                            {
                                if (item != null && item.Count > 0)
                                {
                                    lock (item)
                                    {
                                        if (item != null && item.Count > 0)
                                        {
                                            item.Synchronize();
                                        }
                                    }
                                }
                            }
                            catch
                            {
                            }
                        }

                        flusher.Wait(1);
                    }

                    flusher.Wait(5000);
                }
            },
            TaskCreationOptions.LongRunning);
        }

        /// <summary>
        /// Create a request queue.
        /// </summary>
        /// <param name="rule">Rule element.</param>
        /// <param name="sample">Sample request.</param>
        /// <param name="repository">Repository instance.</param>
        /// <returns>Return an IRequestQueue instance.</returns>
        public static IRequestQueue<IRequest> Create(RuleElement rule, IRequest sample, IPersistentRepository repository)
        {
            /*
             * Because request uid depend on user ip so the number of queues could be very large
             * Each queue has LastTouch property, we monitor it and destroy the queue  that hasn't checked in 2 days
             */

            if (!cache.ContainsKey(sample.Key))
            {
                lock (locker)
                {
                    if (!cache.ContainsKey(sample.Key))
                    {
                        var queue = new RequestQueue(rule.FullName, repository);

                        queue.Interval = TimeSpan.FromMinutes(rule.Minutes);
                        queue.Threshold = rule.Threshold;
                        queue.Key = sample.Key;

                        IRequestBlocker blocker = RequestBlockerFactory.Create(rule);

                        queue.SetBlocker(blocker);

                        // Avoid memory overflow, we need to guarantee that there is a limit number of queues
                        // Monitor max item and remove expired one
                        // Remote repository will be expired after timespan time, so no need to update repository after removal
                        if (cache.Count > MaxItem)
                        {
                            foreach (IRequestQueue<IRequest> item in cache.Values)
                            {
                                if (DateTime.UtcNow.CompareTo(item.LastChecked.AddDays(2)) > 0)
                                {
                                    cache.Remove(queue.Key);
                                }
                            }
                        }

                        cache.Add(queue.Key, queue);
                    }
                }
            }

            return cache[sample.Key];
        }
    }
}
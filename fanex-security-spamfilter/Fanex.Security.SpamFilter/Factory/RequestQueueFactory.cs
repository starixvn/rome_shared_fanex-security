// --------------------------------------------------------------------------
// <copyright file="RequestQueueFactory.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------

namespace Fanex.Security.SpamFilter.Factory
{
    using System;
    using System.Collections.Concurrent;
    using System.Linq;
    using System.Threading.Tasks;
    using Fanex.Logging;
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

        private static ConcurrentDictionary<string, IRequestQueue<IRequest>> cache = new ConcurrentDictionary<string, IRequestQueue<IRequest>>();
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
                    try
                    {
                        if (flusher == null)
                        {
                            continue;
                        }

                        for (int i = 0; i < cache.Keys.Count; i++)
                        {
                            IRequestQueue<IRequest> item = null;
                            cache.TryGetValue(cache.Keys.ElementAt(i), out item);

                            if (item != null
                                && DateTime.UtcNow.CompareTo(item.LastChecked.AddSeconds(7)) > 0
                                && item.Count > 0)
                            {
                                lock (item)
                                {
                                    if (item != null && item.Count > 0)
                                    {
                                        // Try to refresh queue by checking its blocked status
                                        if (item.Blocked || !item.Blocker.Blocked)
                                        {
                                            // Try to synchronize again
                                            item.Synchronize();
                                        }
                                    }
                                }
                            }

                            flusher.Wait(1);
                        }

                        flusher.Wait(5000);
                    }
                    catch (Exception ex)
                    {
                        LogService.Error(ex.Message, ex);
                    }
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
                        IRequestBlocker blocker = RequestBlockerFactory.Create(rule);
                        var queue = new RequestQueue(rule.FullName, sample.Key, rule, blocker, repository);

                        // Avoid memory overflow, we need to guarantee that there is a limit number of queues
                        // Monitor max item and remove expired one
                        // Remote repository will be expired after timespan time, so no need to update repository after removal
                        if (cache.Count > MaxItem)
                        {
                            var requestQueues = cache.Values.ToList();
                            foreach (IRequestQueue<IRequest> item in requestQueues)
                            {
                                if (DateTime.UtcNow.CompareTo(item.LastChecked.AddDays(2)) > 0)
                                {
                                    IRequestQueue<IRequest> removingItem = null;
                                    cache.TryRemove(item.Key, out removingItem);
                                }
                            }
                        }

                        cache.TryAdd(queue.Key, queue);
                    }
                }
            }

            IRequestQueue<IRequest> requestQueue = null;

            cache.TryGetValue(sample.Key, out requestQueue);

            return requestQueue;
        }
    }
}
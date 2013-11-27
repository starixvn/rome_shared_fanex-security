// --------------------------------------------------------------------------
// <copyright file="HttpContextExtension.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   
// </history>

namespace Fanex.Security.SpamFilter
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web;

    using Fanex.Security.SpamFilter.Entity;
    using Fanex.Security.SpamFilter.Factory;
    using Fanex.Security.SpamFilter.Filter;
    using Fanex.Security.SpamFilter.Repository;
    using Fanex.Security.SpamFilter.Resolver;

    /// <summary>
    /// Extend http context.
    /// </summary>
    public static class HttpContextExtension
    {
        private static object locker = new object();

        /// <summary>
        /// Get collection of spam filter blocker at current timestamp for current request.
        /// </summary>
        /// <param name="context">Current HttpContext.</param>
        /// <param name="blocked">Filter by Blocked status.</param>
        /// <returns>Return a collection.</returns>
        public static ICollection<IRequestBlocker> GetSpamFilterBlockers(this HttpContext context, bool blocked = true)
        {
            ICollection<IRequestQueue<IRequest>> collection = context.GetQueues();
            List<IRequestBlocker> lockers = new List<IRequestBlocker>();
            foreach (IRequestQueue<IRequest> queue in collection)
            {
                if (blocked == queue.Blocked)
                {
                    lockers.Add(queue.Blocker);
                }
            }

            return lockers;
        }

        /// <summary>
        /// Explicit increment success count by delta unit of current request context.
        /// </summary>
        /// <param name="context">Current HttpContext.</param>
        /// <param name="delta">Delta need to increase.</param>
        public static void SpamFilterIncrement(this HttpContext context, ulong delta = 1)
        {
            if (delta == 0)
            {
                return;
            }

            ICollection<RuleElement> rules = RuleProcessor.MatchRules(context.Request);

            // Processing rules with auto increment = false
            RuleProcessor.ProcessTask(
                context,
                rules,
                (RuleElement rule, IRequest request, IRequestQueue<IRequest> queue, IResolver resolver) =>
            {
                if (rule.AutoIncrement)
                {
                    return;
                }

                lock (queue)
                {
                    RuleProcessor.ProcessRule(context, rule, request, queue, resolver, delta);
                }
            });
        }

        /// <summary>
        /// Reset increment of provided rule name for current request context.
        /// </summary>
        /// <param name="context">Current HttpContext instance.</param>
        /// <param name="name">Name of rule need to be reset.</param>
        public static void SpamFilterResetRule(this HttpContext context, string name)
        {
            IRequestQueue<IRequest> queue = context.GetQueue(name);

            if (queue != null)
            {
                queue.Clear();
            }
        }

        /// <summary>
        /// Return current queue instance of current request context, otherwise return null.
        /// </summary>
        /// <param name="context">Current HttpContext.</param>
        /// <param name="name">Name of queue need to get.</param>
        /// <returns>Return an IRequestQueue instance.</returns>
        internal static IRequestQueue<IRequest> GetQueue(this HttpContext context, string name)
        {
            ICollection<IRequestQueue<IRequest>> queues = context.GetQueues();
            return (from IRequestQueue<IRequest> queue in queues
                    where queue.Name == RuleElement.GetFullName(name)
                    select queue).FirstOrDefault();
        }

        /// <summary>
        /// Return all queue instances of current request context, otherwise return empty.
        /// </summary>
        /// <param name="context">Current HttpContext.</param>
        /// <param name="autoIncrement">Filter by AutoIncrement flag.</param>
        /// <returns>Return a collection.</returns>
        internal static ICollection<IRequestQueue<IRequest>> GetQueues(this HttpContext context, bool autoIncrement = false)
        {
            List<IRequestQueue<IRequest>> collection = new List<IRequestQueue<IRequest>>();
            ICollection<RuleElement> rules = RuleProcessor.MatchRules(context.Request);

            RuleProcessor.ProcessTask(
                context,
                rules,
                (RuleElement rule, IRequest request, IRequestQueue<IRequest> queue, IResolver resolver) =>
            {
                if (rule.AutoIncrement == autoIncrement)
                {
                    collection.Add(queue);
                }
            });

            return collection;
        }
    }
}
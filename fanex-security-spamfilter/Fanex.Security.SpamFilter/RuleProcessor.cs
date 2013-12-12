// --------------------------------------------------------------------------
// <copyright file="RuleProcessor.cs" company="Nexcel Solutions Vietnam">
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
    using System.Threading;
    using System.Web;

    using Fanex.Security.SpamFilter.Entity;
    using Fanex.Security.SpamFilter.Factory;
    using Fanex.Security.SpamFilter.Filter;
    using Fanex.Security.SpamFilter.Repository;
    using Fanex.Security.SpamFilter.Resolver;

    /// <summary>
    /// Rule processor.
    /// </summary>
    internal static class RuleProcessor
    {
        public delegate void RuleTask(RuleElement rule, IRequest request, IRequestQueue<IRequest> queue, IResolver resolver);

        private static IResolver DefaulResolver
        {
            get
            {
                return ResolverFactory.Create(Configuration.Current.Resolver);
            }
        }

        /// <summary>
        /// Matching current rule binding to current request.
        /// </summary>
        /// <param name="request">Current request.</param>
        /// <returns>Return a collection of matching rules.</returns>
        public static ICollection<RuleElement> MatchRules(HttpRequest request)
        {
            var rules = Configuration.Current.RuleCollection;
            List<RuleElement> matchedRules = new List<RuleElement>();

            if (rules.Count == 0)
            {
                return matchedRules;
            }

            try
            {
                // Matching request to create/get corresponding rules
                // Performance critical code
                // Number of pre-defined rules is not huge, so we don't worry about the foreach loop
                foreach (RuleElement rule in rules)
                {
                    foreach (string path in rule.PathCollection)
                    {
                        if (string.IsNullOrEmpty(path))
                        {
                            continue;
                        }

                        // Match current execution file path
                        // "~/path/" match "/path/"
                        if (request.AppRelativeCurrentExecutionFilePath.ToLower().IndexOf(path.ToLower()) == 1)
                        {
                            // Match http method
                            if (rule.MethodCollection.Contains("*") || rule.MethodCollection.Contains(request.HttpMethod.ToUpper()))
                            {
                                matchedRules.Add(rule);
                                continue;
                            }
                        }
                    }
                }
            }
            catch 
            { 
            }

            return matchedRules;
        }

        /// <summary>
        /// Process spam filter rule.
        /// </summary>
        /// <param name="context">Current HttpContext.</param>
        /// <param name="rule">Rule element.</param>
        /// <param name="request">Current request.</param>
        /// <param name="queue">Current queue.</param>
        /// <param name="resolver">Current resolver.</param>
        /// <param name="delta">Delta need to be increased.</param>
        public static void ProcessRule(
            HttpContext context,
            RuleElement rule,
            IRequest request,
            IRequestQueue<IRequest> queue,
            IResolver resolver,
            ulong delta = 1)
        {
            bool rejected = false;
            bool autoIncrement = rule.AutoIncrement;
            bool includeRejectedRequest = rule.IncludeRejectedRequest;

            // One queue allow to work in one thread only
            lock (queue)
            {
                // Flush/check blocked status
                rejected = queue.Blocked;

                if (!rejected || includeRejectedRequest)
                {
                    // Perform increment violated count
                    request.Weight = delta;
                    queue.Enqueue(request);
                }
            }

            // Perform reject or accept response
            if (rejected)
            {
                resolver.Reject(context);
            }
            else
            {
                resolver.Accept(context);
            }
        }

        /// <summary>
        /// Iterate through all rules and processing a task.
        /// </summary>
        /// <param name="context">Current HttpContext.</param>
        /// <param name="rules">Rules collection.</param>
        /// <param name="task">Task to be executed.</param>
        public static void ProcessTask(HttpContext context, ICollection<RuleElement> rules, RuleTask task)
        {
            /*
             * IMPORTANT NOTES
             * Put request to a queue in memory base on its key, check and remove expired requests, count requests and fire Reject or Accept
             * Each request, having a unique request queue
             * When a request belongs to a queue hit, we refresh that queue to remove expired requests, count and return the blocked status
             * that indicates the request count is reach count limit in a timespan
             */

            // Performance critical code
            if (rules.Count == 0)
            {
                return;
            }

            // Loop all matched rules and update corresponding queues
            foreach (RuleElement rule in rules)
            {
                // Create/get corresponding filter
                IFilter filter = FilterFactory.Create(rule.Filter);
                IRequest request = RequestFactory.Create(Configuration.Current.System, context, rule, filter, 0);

                // Filter doesn't work
                if (request == null)
                {
                    continue;
                }

                // Create/get persistent storage
                var repositoryType = Enum.Parse(typeof(RepositoryType), Configuration.Current.Repository, true);
                IPersistentRepository repository = RepositoryFactory.Create((RepositoryType)repositoryType);

                // Create/get corresponding request queue
                IRequestQueue<IRequest> queue = RequestQueueFactory.Create(rule, request, repository);

                // Create/get corresponding resolver
                IResolver resolver = string.IsNullOrEmpty(rule.Resolver) ? DefaulResolver : ResolverFactory.Create(rule.Resolver);

                try
                {
                    task(rule, request, queue, resolver);
                }
                catch
                { 
                }
            }
        }
    }
}
// --------------------------------------------------------------------------
// <copyright file="RequestFactory.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   
// </history>

namespace Fanex.Security.SpamFilter.Factory
{
    using System;
    using System.Web;

    using Fanex.Security.SpamFilter.Entity;
    using Fanex.Security.SpamFilter.Filter;

    /// <summary>
    /// Implementation of IRequest factory.
    /// </summary>
    internal static class RequestFactory
    {
        /// <summary>
        /// Create an implementation instance of IRequest.
        /// </summary>
        /// <param name="system">System name.</param>
        /// <param name="context">Current HttpContext.</param>
        /// <param name="rule">Rule element.</param>
        /// <param name="filter">Filter instance.</param>
        /// <param name="weight">Number of request delta.</param>
        /// <returns>Return an instance of IRequest.</returns>
        internal static IRequest Create(string system, HttpContext context, RuleElement rule, IFilter filter, ulong weight = 1)
        {
            Request item = Activator.CreateInstance<Request>();

            string filterBy = filter.GetIdentity(context);

            if (string.IsNullOrEmpty(filterBy))
            {
                throw new Exception(filter + " return empty identity value.");
            }

            var composite = new string[]
            {
                system,
                "/",
                rule.Name,
                "/",
                filterBy
            };

            var uid = string.Join(null, composite);

            item.Key = uid;
            item.RequestTime = DateTime.UtcNow;
            item.Weight = weight;

            return item;
        }
    }
}
// --------------------------------------------------------------------------
// <copyright file="FilterFactory.cs" company="Nexcel Solutions Vietnam">
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

    using Fanex.Security.SpamFilter.Filter;

    /// <summary>
    /// Implementation of IFilter factory.
    /// </summary>
    public static class FilterFactory
    {
        private static IDictionary<string, IFilter> cache = new Dictionary<string, IFilter>();
        private static object locker = new object();

        /// <summary>
        /// Singleton create/get unique instance for each filter type.
        /// </summary>
        /// <param name="type">Strong type of implementation of IFilter need to be created.</param>
        /// <returns>Return an IFilter instance.</returns>
        public static IFilter Create(string type)
        {
            if (!cache.ContainsKey(type))
            {
                lock (locker)
                {
                    if (!cache.ContainsKey(type))
                    {
                        // Filter by IP as default
                        var filterType = string.IsNullOrEmpty(type) ? typeof(IPFilter) : Type.GetType(type);
                        if (filterType == null)
                        {
                            throw new TypeLoadException("Could not resolve type of " + type);
                        }

                        var item = Activator.CreateInstance(filterType);
                        var filter = item as IFilter;

                        if (filter != null)
                        {
                            cache.Add(type, filter);
                            return filter;
                        }
                        else
                        {
                            throw new TypeLoadException(type + " is not an implementation of " + typeof(IFilter));
                        }
                    }
                }
            }

            return cache[type];
        }
    }
}
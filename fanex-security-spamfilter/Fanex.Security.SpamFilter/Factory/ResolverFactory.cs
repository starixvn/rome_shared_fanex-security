// --------------------------------------------------------------------------
// <copyright file="ResolverFactory.cs" company="Nexcel Solutions Vietnam">
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

    using Fanex.Security.SpamFilter.Resolver;

    /// <summary>
    /// Implementation of IResolver factory.
    /// </summary>
    public static class ResolverFactory
    {
        private static IDictionary<string, IResolver> cache = new Dictionary<string, IResolver>();
        private static object locker = new object();

        /// <summary>
        /// Singleton create/get unique instance for each resolver type.
        /// </summary>
        /// <param name="type">Strong type of implementation of IResolver need to be created.</param>
        /// <returns>Return an IResolver instance.</returns>
        public static IResolver Create(string type)
        {
            if (!cache.ContainsKey(type))
            {
                lock (locker)
                {
                    if (!cache.ContainsKey(type))
                    {
                        // Default is plain text
                        var resolverType = string.IsNullOrEmpty(type) ? typeof(PlainTextResolver) : Type.GetType(type);
                        if (resolverType == null)
                        {
                            throw new TypeLoadException("Could not resolve type of " + type);
                        }

                        var item = Activator.CreateInstance(resolverType);
                        var resolver = item as IResolver;

                        if (resolver != null)
                        {
                            cache.Add(type, resolver);
                            return resolver;
                        }
                        else
                        {
                            throw new TypeLoadException(type + " is not a type of " + typeof(IResolver));
                        }
                    }
                }
            }

            return cache[type];
        }
    }
}
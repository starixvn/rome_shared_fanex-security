// --------------------------------------------------------------------------
// <copyright file="RepositoryFactory.cs" company="Nexcel Solutions Vietnam">
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
    using System.Diagnostics;
    using System.Net;
    using Fanex.Logging;
    using Fanex.Security.SpamFilter.Repository;

    /// <summary>
    /// Remote repository storage type.
    /// </summary>
    internal enum RepositoryType
    {
        /// <summary>
        /// Couch-base repository.
        /// </summary>
        Couchbase,

        /// <summary>
        /// State server repository.
        /// </summary>
        StateServer
    }

    /// <summary>
    /// Implementation of IPersistentRepository factory.
    /// </summary>
    internal static class RepositoryFactory
    {
        private const string HostConnected = " - connected";
        private static IDictionary<RepositoryType, IPersistentRepository> cache = new Dictionary<RepositoryType, IPersistentRepository>();
        private static object locker = new object();

        static RepositoryFactory()
        {
            AppDomain.CurrentDomain.DomainUnload += new EventHandler(Repository_Dispose);
            Process current = Process.GetCurrentProcess();
            current.EnableRaisingEvents = true;
            current.Exited += new EventHandler(Repository_Dispose);
        }

        /// <summary>
        /// Singleton create/get unique instance for each repository type.
        /// </summary>
        /// <param name="type">Strong type of implementation of IPersistentRepository need to be created.</param>
        /// <returns>Return an IPersistentRepository instance.</returns>
        internal static IPersistentRepository Create(RepositoryType type)
        {
            if (!cache.ContainsKey(type))
            {
                lock (locker)
                {
                    if (!cache.ContainsKey(type))
                    {
                        IPersistentRepository item = null;

                        if (type == RepositoryType.Couchbase)
                        {
                            CouchbaseRepository couchbase = new CouchbaseRepository(Configuration.Current.ConnectionString);

                            try
                            {
                                couchbase.Set(Dns.GetHostName() + HostConnected, true);
                                couchbase.Initialized = true;
                            }
                            catch (Exception ex)
                            {
                                LogService.Error(ex.Message, ex);
                            }

                            item = couchbase;
                        }

                        if (item != null)
                        {
                            cache.Add(type, item);
                            return item as IPersistentRepository;
                        }
                        else
                        {
                            throw new NotImplementedException(type + " is not an implementation of " + typeof(IPersistentRepository));
                        }
                    }
                }
            }

            return cache[type];
        }

        internal static void Initialize()
        {
            Create(RepositoryType.Couchbase);
        }

        /// <summary>
        /// Important note - this code guarantee that remote storage in web-farm environment work well.
        /// We must update remote storage when an application exit or unload.
        /// </summary>
        /// <param name="sender">Default sender object.</param>
        /// <param name="e">Default event argument.</param>
        private static void Repository_Dispose(object sender, EventArgs e)
        {
            foreach (IPersistentRepository repository in cache.Values)
            {
                try
                {
                    repository.Set(Dns.GetHostName() + HostConnected, false);
                    repository.Dispose();
                }
                catch (Exception ex)
                {
                    LogService.Error(ex.Message, ex);
                }
            }
        }
    }
}
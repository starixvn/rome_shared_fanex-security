// --------------------------------------------------------------------------
// <copyright file="HttpInterceptorModule.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//
// </history>

[assembly: System.Web.PreApplicationStartMethod(typeof(Fanex.Security.SpamFilter.HttpInterceptorModule), "Start")]

namespace Fanex.Security.SpamFilter
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web;
    using Fanex.Logging;
    using Fanex.Security.SpamFilter.Entity;
    using Fanex.Security.SpamFilter.Factory;
    using Fanex.Security.SpamFilter.Repository;
    using Fanex.Security.SpamFilter.Resolver;
    using Microsoft.Web.Infrastructure.DynamicModuleHelper;

    /// <summary>
    /// Intercept http module and apply rule.
    /// </summary>
    public class HttpInterceptorModule : IHttpModule
    {
        private static object locker = new object();

        /// <summary>
        /// Gets or sets a value indicating module id.
        /// </summary>
        private string UID
        {
            get;
            set;
        }

        /// <summary>
        /// Module entry point.
        /// </summary>
        public static void Start()
        {
            DynamicModuleUtility.RegisterModule(typeof(HttpInterceptorModule));
        }

        public void Dispose()
        {
        }

        /// <summary>
        /// When application initialize.
        /// </summary>
        /// <param name="context">Current application context.</param>
        public void Init(HttpApplication context)
        {
            this.UID = Guid.NewGuid().ToString();
            RepositoryFactory.Initialize();

            /*
             *
                BeginRequest
                AuthenticateRequest
                AuthorizeRequest
                ResolveRequestCache
                AcquireRequestState
                PreRequestHandlerExecute        <== Capture here and lock
                PostRequestHandlerExecute       <== Capture here and unlock
                ReleaseRequestState
                UpdateRequestCache
                EndRequest
             *
             *
             */

            context.PreRequestHandlerExecute += new EventHandler(this.Context_PreRequestHandlerExecute);
            context.PostRequestHandlerExecute += new EventHandler(this.Context_PostRequestHandlerExecute);
        }

        private void Abort(HttpApplication application)
        {
            // Too Many Requests
            application.Context.Response.StatusDescription = "Too Many Requests";
            application.Context.Response.StatusCode = 403;
            application.CompleteRequest();
            application.Context.Response.End();
            application.Context.Response.Close();
        }

        /// <summary>
        /// After request was processed.
        /// </summary>
        /// <param name="sender">Application send.</param>
        /// <param name="e">Default event argument.</param>
        private void Context_PostRequestHandlerExecute(object sender, EventArgs e)
        {
            HttpApplication application = sender as HttpApplication;
            ICollection<RuleElement> rules = RuleProcessor.MatchRules(application.Context.Request);

            if (rules.Count == 0)
            {
                return;
            }

            string key = string.Join(
                             "#",
                             (from rule in rules where rule.AutoIncrement == false select rule.Name).ToArray());

            // There is an explicit increment rule
            if (!string.IsNullOrEmpty(key))
            {
                // Create/get persistent storage
                var repositoryType = Enum.Parse(typeof(RepositoryType), Configuration.Current.Repository, true);
                IPersistentRepository repository = RepositoryFactory.Create((RepositoryType)repositoryType);
                bool unlocked = repository.Unlock(this.GetLockKey(key), this.UID);

                if (!unlocked)
                {
                    this.Abort(application);
                    return;
                }
            }

            RuleProcessor.ProcessTask(
                application.Context,
                rules,
                (RuleElement rule, IRequest request, IRequestQueue<IRequest> queue, IResolver resolver) =>
                {
                    try
                    {
                        application.Context.Response.StatusDescription += string.Concat(" >> ", queue.Count, ",", queue.RemoteTotal);
                    }
                    catch (Exception ex)
                    {
                        LogService.Warn(ex.Message, ex);
                    }
                });
        }

        /// <summary>
        /// Before request is processed.
        /// </summary>
        /// <param name="sender">Application send.</param>
        /// <param name="e">Default event argument.</param>
        private void Context_PreRequestHandlerExecute(object sender, EventArgs e)
        {
            HttpApplication application = sender as HttpApplication;
            ICollection<RuleElement> rules = RuleProcessor.MatchRules(application.Context.Request);

            if (rules.Count == 0)
            {
                return;
            }

            string key = string.Join(
                             "#",
                             (from rule in rules where rule.AutoIncrement == false select rule.Name).ToArray());

            // There is an explicit increment rule
            if (!string.IsNullOrEmpty(key))
            {
                // Create/get persistent storage
                var repositoryType = Enum.Parse(typeof(RepositoryType), Configuration.Current.Repository, true);
                IPersistentRepository repository = RepositoryFactory.Create((RepositoryType)repositoryType);
                bool locked = repository.Lock(this.GetLockKey(key), this.UID);

                if (!locked)
                {
                    this.Abort(application);
                    return;
                }
            }

            // Processing rules with auto increment = true
            RuleProcessor.ProcessTask(
                application.Context,
                rules,
                (RuleElement rule, IRequest request, IRequestQueue<IRequest> queue, IResolver resolver) =>
                {
                    if (!rule.AutoIncrement)
                    {
                        return;
                    }

                    lock (queue)
                    {
                        RuleProcessor.ProcessRule(application.Context, rule, request, queue, resolver, 1);
                    }
                });
        }

        private string GetLockKey(string key)
        {
            return Configuration.Current.System + "/" + key;
        }
    }
}
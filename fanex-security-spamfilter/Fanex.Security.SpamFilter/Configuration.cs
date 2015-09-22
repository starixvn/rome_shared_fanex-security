// --------------------------------------------------------------------------
// <copyright file="Configuration.cs" company="Nexcel Solutions Vietnam">
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
    using System.Configuration;
    using System.Diagnostics.CodeAnalysis;
    using Fanex.Logging;

    /// <summary>
    /// Handle configuration for spam filter module.
    /// </summary>
    public class Configuration : ConfigurationSection
    {
        private static Configuration current = null;

        /// <summary>
        /// Gets a value indicating current Configuration instance value.
        /// </summary>
        /// <value>The current configuration value.</value>
        public static Configuration Current
        {
            get
            {
                if (current == null)
                {
                    try
                    {
                        current = (Configuration)ConfigurationManager.GetSection("SpamFilterConfiguration");
                    }
                    catch (Exception ex)
                    {
                        LogService.Error(ex.Message, ex);

                        return null;
                    }

                    if (string.IsNullOrEmpty(current.System))
                    {
                        return null;
                    }

                    foreach (RuleElement element in current.RuleCollection)
                    {
                        if (element.Threshold > int.MaxValue / 2)
                        {
                            return null;
                        }

                        if (element.Minutes > 24 * 60)
                        {
                            return null;
                        }
                    }
                }

                return current;
            }
        }

        [ConfigurationProperty("connectionString", IsRequired = true)]
        public string ConnectionString
        {
            get
            {
                return (string)this["connectionString"];
            }
        }

        /// <summary>
        /// Gets a value indicating filter type for each request, default is filtered by IP.
        /// </summary>
        /// <value>The filter name.</value>
        [ConfigurationProperty("filter")]
        public string Filter
        {
            get
            {
                return (string)this["filter"];
            }
        }

        /// <summary>
        /// Gets a value indicating repository type.
        /// </summary>
        /// <value>The repository name.</value>
        [ConfigurationProperty("repository", IsRequired = true)]
        public string Repository
        {
            get
            {
                return (string)this["repository"];
            }
        }

        /// <summary>
        /// Gets a value indicating resolver to process response, default is plain text resolver.
        /// </summary>
        /// <value>The resolver name.</value>
        [ConfigurationProperty("resolver", IsRequired = true)]
        public string Resolver
        {
            get
            {
                return (string)this["resolver"];
            }
        }

        /// <summary>
        /// Gets a value indicating rule collection.
        /// </summary>
        /// <value>The rule collection value.</value>
        [ConfigurationProperty("rules", IsDefaultCollection = true)]
        [ConfigurationCollection(typeof(RuleCollection), AddItemName = "add", RemoveItemName = "remove", ClearItemsName = "clear")]
        public RuleCollection RuleCollection
        {
            get
            {
                return (RuleCollection)base["rules"];
            }
        }

        [ConfigurationProperty("system", IsRequired = true)]
        public string System
        {
            get
            {
                return (string)this["system"];
            }
        }
    }

    /// <summary>
    /// Rule configuration element collection.
    /// </summary>
    [SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1402:FileMayOnlyContainASingleClass", Justification = "Reviewed.")]
    public class RuleCollection : ConfigurationElementCollection
    {
        public new RuleElement this[string name]
        {
            get
            {
                RuleElement element = (RuleElement)this.BaseGet(name);
                return element;
            }
        }

        protected override ConfigurationElement CreateNewElement()
        {
            return new RuleElement();
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((RuleElement)element).Name;
        }
    }

    /// <summary>
    /// Rule configuration element.
    /// </summary>
    public class RuleElement : ConfigurationElement
    {
        private List<string> _methodCollection = new List<string>();
        private List<string> _pathCollection = new List<string>();

        [ConfigurationProperty("autoIncrement")]
        public bool AutoIncrement
        {
            get
            {
                return (bool)this["autoIncrement"];
            }
        }

        /// <summary>
        /// Gets a value indicating timespan in minute to lock request.
        /// </summary>
        /// <value>The block duration value.</value>
        [ConfigurationProperty("blockDuration", DefaultValue = 0)]
        public int BlockDuration
        {
            get
            {
                return (int)this["blockDuration"];
            }
        }

        /// <summary>
        /// Gets a value indicating filter for each request, default is filtered by IP.
        /// </summary>
        /// <value>The filter name.</value>
        [ConfigurationProperty("filter")]
        public string Filter
        {
            get
            {
                return (string)this["filter"];
            }
        }

        public string FullName
        {
            get
            {
                return GetFullName(this.Name);
            }
        }

        [ConfigurationProperty("includeRejectedRequest")]
        public bool IncludeRejectedRequest
        {
            get
            {
                if (this["includeRejectedRequest"] == null)
                {
                    return false;
                }

                return (bool)this["includeRejectedRequest"];
            }
        }

        public List<string> MethodCollection
        {
            get
            {
                if (this._methodCollection.Count == 0)
                {
                    this._methodCollection.AddRange(this.Method.Split(new char[] { ',', ';' }, StringSplitOptions.RemoveEmptyEntries));
                }

                return this._methodCollection;
            }
        }

        /// <summary>
        /// Gets a value indicating timespan in minute to check frequent requests.
        /// </summary>
        /// <value>The minutes value.</value>
        [ConfigurationProperty("minutes", IsRequired = true)]
        public int Minutes
        {
            get
            {
                return (int)this["minutes"];
            }
        }

        [ConfigurationProperty("name", IsRequired = true)]
        public string Name
        {
            get
            {
                return (string)this["name"];
            }
        }

        /// <summary>
        /// Gets a value indicating list of path need to filter.
        /// </summary>
        /// <value>The path collection value.</value>
        public List<string> PathCollection
        {
            get
            {
                if (this._pathCollection.Count == 0)
                {
                    this._pathCollection.AddRange(this.Path.Split(new char[] { ',', ';', '\n', '\t', '\r', ' ' }, StringSplitOptions.RemoveEmptyEntries));
                }

                return this._pathCollection;
            }
        }

        /// <summary>
        /// Gets a value indicating resolver to process response, default is plain text resolver.
        /// </summary>
        /// <value>The resolver name.</value>
        [ConfigurationProperty("resolver")]
        public string Resolver
        {
            get
            {
                return (string)this["resolver"];
            }
        }

        /// <summary>
        /// Gets a value indicating threshold to limit frequent requests.
        /// </summary>
        /// <value>The threshold value.</value>
        [ConfigurationProperty("threshold", IsRequired = true)]
        public int Threshold
        {
            get
            {
                return (int)this["threshold"];
            }
        }

        /// <summary>
        /// Gets a value indicating path need to filter.
        /// </summary>
        [ConfigurationProperty("path", IsRequired = true)]
        internal string Path
        {
            get
            {
                return (string)this["path"];
            }
        }

        /// <summary>
        /// Gets a value indicating method to limit frequent requests.
        /// </summary>
        [ConfigurationProperty("method", IsRequired = true)]
        private string Method
        {
            get
            {
                return (string)this["method"];
            }
        }

        public static string GetFullName(string ruleName)
        {
            return Configuration.Current.System + "/" + ruleName;
        }
    }
}
// ***********************************************************************
// Assembly         : Fanex.Security.Captcha
// Author           : Steven.Nguyen
// Created          : 04-18-2014
//
// Last Modified By : Steven.Nguyen
// Last Modified On : 04-18-2014
// ***********************************************************************
// <copyright file="MonitorController.cs" company="Nexcel">
//     Copyright (c) Nexcel. All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************

namespace Fanex.Security.Captcha.Controllers
{
    using System;
    using System.Linq;
    using System.Web.Mvc;
    using Couchbase;
    using Couchbase.Configuration;
    using Newtonsoft.Json;

    /// <summary>
    /// Class MonitorController.
    /// </summary>
    public class MonitorController : Controller
    {
        /// <summary>
        /// The configuration.
        /// </summary>
        private CouchbaseClientConfiguration _config = new CouchbaseClientConfiguration();

        /// <summary>
        /// The JSON setting.
        /// </summary>
        private JsonSerializerSettings _jsonSetting = new JsonSerializerSettings();
        
        /// <summary>
        /// The _client.
        /// </summary>
        private CouchbaseClient _client;

        /// <summary>
        /// Initializes a new instance of the <see cref="MonitorController"/> class.
        /// </summary>
        /// <exception cref="System.InvalidCastException">Invalid connection string, the format should be.</exception>
        public MonitorController()
        {
            var connectionString = Fanex.Security.SpamFilter.Configuration.Current.ConnectionString;
            var items = connectionString.Split(
                                    new string[]
                                    {
                                        ";Bucket=",
                                        ";BucketPassword="
                                    },
                                    StringSplitOptions.None);
            if (items.Length != 3)
            {
                var exceptionMsg =
                    "Invalid connnection string, the format should be http://serverip:port/pools;Bucket=bucket1;BucketPassword=password1.";
                throw new InvalidCastException(exceptionMsg);
            }

            _config.Urls.Add(new Uri(items[0]));
            _config.Bucket = items[1];
            _config.BucketPassword = items[2];

            _config.SocketPool.MinPoolSize = 1;
            _config.SocketPool.MaxPoolSize = 5;
            _client = new CouchbaseClient(_config);
        }

        /// <summary>
        /// Indexes the specified start key.
        /// </summary>
        /// <param name="startKey">The start key.</param>
        /// <param name="pageSize">Size of the page.</param>
        /// <returns>Action Result.</returns>
        public ActionResult Index(string startKey, int pageSize = 5)
        {
            var view = _client.GetView("spamfilter", "spamfilterbycustomer");

            return View("~/Areas/SpamFilter/Views/Monitor/Index.cshtml", view.Take(pageSize));
        }
    }
}

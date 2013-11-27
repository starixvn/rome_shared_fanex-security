using System;
using System.Linq;
using System.Web.Mvc;
using Couchbase;
using Couchbase.Configuration;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Fanex.Security.Captcha.Controllers
{
    public class MonitorController : Controller
    {
        private CouchbaseClientConfiguration config = new CouchbaseClientConfiguration();
        private JsonSerializerSettings jsonSetting = new JsonSerializerSettings();

       
        private CouchbaseClient _client;

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
                throw new InvalidCastException("Invalid connnection string, the format should be http://serverip:port/pools;Bucket=bucket1;BucketPassword=password1.");
            }

            config.Urls.Add(new Uri(items[0]));
            config.Bucket = items[1];
            config.BucketPassword = items[2];

            config.SocketPool.MinPoolSize = 1;
            config.SocketPool.MaxPoolSize = 5;
            _client = new CouchbaseClient(config);
           
        }

        public ActionResult Index(string startKey, int pageSize = 5)
        {
            var view = _client.GetView("spamfilter", "spamfilterbycustomer");

            return View("~/Areas/SpamFilter/Views/Monitor/Index.cshtml", view.Take(pageSize));
        }


    }
}

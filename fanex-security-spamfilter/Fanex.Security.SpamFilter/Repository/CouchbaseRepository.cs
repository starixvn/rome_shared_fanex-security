// --------------------------------------------------------------------------
// <copyright file="CouchbaseRepository.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   
// </history>

namespace Fanex.Security.SpamFilter.Repository
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Threading;
    using System.Threading.Tasks;
    using Couchbase;
    using Couchbase.Configuration;
    using Enyim.Caching.Memcached;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Converters;
    using Newtonsoft.Json.Serialization;

    /// <summary>
    /// Implement IPersistentCounter using Couch-base.
    /// </summary>
    internal class CouchbaseRepository : IPersistentRepository
    {
        private const string LockedSuffix = "$Locked";

        private CouchbaseClient _client;
        private CouchbaseClientConfiguration _config = new CouchbaseClientConfiguration();
        private JsonSerializerSettings _jsonSetting = new JsonSerializerSettings();

        internal CouchbaseRepository(string connectionString)
        {
            // Example connectionString="http://10.18.200.209:8091/pools;Bucket=Fanex.Security.SpamFilter;BucketPassword="
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

            this._config.Urls.Add(new Uri(items[0]));
            this._config.Bucket = items[1];
            this._config.BucketPassword = items[2];

            this._config.SocketPool.MinPoolSize = 1;
            this._config.SocketPool.MaxPoolSize = 5;

            this._jsonSetting.NullValueHandling = NullValueHandling.Ignore;
            this._jsonSetting.Formatting = Formatting.None;
            this._jsonSetting.ContractResolver = new CamelCasePropertyNamesContractResolver();
            this._jsonSetting.DateFormatHandling = DateFormatHandling.IsoDateFormat;
            this._jsonSetting.DateTimeZoneHandling = DateTimeZoneHandling.Utc;
            this._jsonSetting.TypeNameHandling = TypeNameHandling.None;
            this._jsonSetting.Converters.Add(new ReadableDateTimeConvertor());

            this._client = new CouchbaseClient(this._config);
        }

        public event EventHandler OnPreDispose;

        public bool Initialized
        {
            get;
            set;
        }

        public ulong Decrement(string key, ulong delta = 1)
        {
            ulong value = this._client.Decrement(key, 0, delta);
            this._client.FlushAll();
            return value;
        }

        /// <summary>
        /// Dispose and notify event.
        /// </summary>
        public void Dispose()
        {
            try
            {
                if (this.OnPreDispose != null)
                {
                    this.OnPreDispose(this, EventArgs.Empty);
                }
            }
            catch
            {
            }
            finally
            {
                this._client.Dispose();
                this.Initialized = false;
            }
        }

        public object Get(string key, Type type)
        {
            var value = this._client.Get(key) as string;
            if (value == null)
            {
                return null;
            }

            return JsonConvert.DeserializeObject(value, type, this._jsonSetting);
        }

        public ulong Increment(string key, ulong delta = 1, int minutes = 1440)
        {
            ulong value = this._client.Increment(key, 0, delta, TimeSpan.FromMinutes(minutes));
            this._client.FlushAll();
            return value;
        }

        /// <summary>
        /// Timeout after 10 seconds if current thread cannot gain the lock.
        /// </summary>
        /// <param name="key">Key need to lock.</param>
        /// <param name="owner">Owner name.</param>
        /// <returns>Return a success value.</returns>
        public bool Lock(string key, string owner)
        {
            if (string.IsNullOrEmpty(owner))
            {
                return false;
            }

            CasResult<bool> casResult;
            bool done = false;
            CasResult<string> cr;
            int retryCount = 2000;
            string lockString = key + LockedSuffix;

            do
            {
                Task.WaitAll(
                    Task.Factory.StartNew(
                        () =>
                {
                    cr = _client.GetWithCas<string>(lockString);
                    string guid = cr.Result;

                    if (string.IsNullOrEmpty(guid))
                    {
                        casResult = _client.Cas(StoreMode.Set, lockString, owner, TimeSpan.FromSeconds(20), cr.Cas);
                        done = casResult.Result;
                    }

                    if (done == false)
                    {
                        Thread.CurrentThread.Join(5);
                    }
                },
                TaskCreationOptions.PreferFairness));

                if (done == false)
                {
                    Thread.CurrentThread.Join(5);
                }
            }
            while (done == false && this.Initialized && retryCount-- > 0);

            return done;
        }

        public void Set(string key, object value, int minutes = 1440)
        {
            this._client.Store(StoreMode.Set, key, JsonConvert.SerializeObject(value, this._jsonSetting), TimeSpan.FromMinutes(minutes));
            this._client.FlushAll();
        }

        public void Set(string key, object value, TimeSpan duration)
        {
            this._client.Store(StoreMode.Set, key, JsonConvert.SerializeObject(value, this._jsonSetting), duration);
            this._client.FlushAll();
        }

        /// <summary>
        /// Timeout after 10 seconds if current thread cannot release the lock.
        /// </summary>
        /// <param name="key">Key to unlock.</param>
        /// <param name="owner">Owner name.</param>
        /// <returns>Return a success value.</returns>
        public bool Unlock(string key, string owner)
        {
            if (string.IsNullOrEmpty(owner))
            {
                return false;
            }

            CasResult<bool> casResult;
            bool done = false;
            int retryCount = 2000;
            string lockString = key + LockedSuffix;

            do
            {
                Task.WaitAll(
                    Task.Factory.StartNew(
                        () =>
                {
                    CasResult<string> cr = _client.GetWithCas<string>(lockString);

                    string guid = cr.Result;
                    done = string.IsNullOrEmpty(guid);

                    if (guid == owner)
                    {
                        casResult = _client.Cas(StoreMode.Set, lockString, string.Empty, cr.Cas);
                        done = casResult.Result;
                    }
                    else
                    {
                        done = true;
                    }

                    if (done == false)
                    {
                        Thread.CurrentThread.Join(5);
                    }
                },
                TaskCreationOptions.PreferFairness));

                if (done == false)
                {
                    Thread.CurrentThread.Join(5);
                }
            }
            while (done == false && this.Initialized && retryCount-- > 0);

            return done;
        }

        public void Zero(string key)
        {
            this._client.Decrement(key, 0, ulong.MaxValue);
        }
    }

    /// <summary>
    /// Custom date time converter for JSON.
    /// </summary>
    [SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1402:FileMayOnlyContainASingleClass", Justification = "Reviewed.")]
    internal class ReadableDateTimeConvertor : DateTimeConverterBase
    {
        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            DateTime result = DateTime.MinValue.ToUniversalTime();
            bool success = DateTime.TryParse(reader.Value.ToString(), out result);
            if (!success)
            {
                result = DateTime.MinValue.ToUniversalTime();
            }

            return result;
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            writer.WriteValue(((DateTime)value).ToString("G"));
        }
    }
}
// --------------------------------------------------------------------------
// <copyright file="IRequestBlocker.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   
// </history>

namespace Fanex.Security.SpamFilter.Entity
{
    using System;
    using System.Runtime.Serialization;

    /// <summary>
    /// Request blocker interface.
    /// </summary>
    public interface IRequestBlocker
    {
        /// <summary>
        /// Gets or sets a value indicating whether current IRequestBlocker is activated.
        /// </summary>
        /// <value>The blocked status.</value>
        [DataMember]
        bool Blocked
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets a value indicating the time that blocker had been activated.
        /// </summary>
        /// <value>The blocked time value.</value>
        [DataMember]
        DateTime BlockedTime
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets a value indicating block duration indicate when blocker is expired.
        /// </summary>
        /// <value>The duration value.</value>
        [DataMember]
        TimeSpan Duration
        {
            get;
            set;
        }

        /// <summary>
        /// Gets a value indicating whether blocker is activated or expired.
        /// </summary>
        /// <value>The expired status.</value>
        [IgnoreDataMember]
        bool Expired
        {
            get;
        }

        /// <summary>
        /// Gets a value indicating time span to limit threshold requests.
        /// </summary>
        /// <value>The interval value.</value>
        [DataMember]
        TimeSpan Interval
        {
            get;
        }

        /// <summary>
        /// Gets a value indicating blocker name or rule name.
        /// </summary>
        /// <value>The name value.</value>
        [DataMember]
        string Name
        {
            get;
        }

        /// <summary>
        /// Gets a value indicating number of requests limit in interval time.
        /// </summary>
        /// <value>The threshold value.</value>
        [DataMember]
        int Threshold
        {
            get;
        }
    }
}
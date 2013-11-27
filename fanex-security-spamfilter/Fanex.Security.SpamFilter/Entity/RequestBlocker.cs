// --------------------------------------------------------------------------
// <copyright file="RequestBlocker.cs" company="Nexcel Solutions Vietnam">
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
    /// Implementation of IRequestBlocker interface.
    /// </summary>
    [DataContract]
    internal class RequestBlocker : IRequestBlocker
    {
        public bool Blocked
        {
            get;
            set;
        }

        public DateTime BlockedTime
        {
            get;
            set;
        }

        public TimeSpan Duration
        {
            get;
            set;
        }

        public bool Expired
        {
            get
            {
                return DateTime.UtcNow.CompareTo(this.BlockedTime.AddMinutes(this.Duration.Minutes)) > 0;
            }
        }

        public TimeSpan Interval
        {
            get;
            set;
        }

        public string Name
        {
            get;
            set;
        }

        public int Threshold
        {
            get;
            set;
        }
    }
}
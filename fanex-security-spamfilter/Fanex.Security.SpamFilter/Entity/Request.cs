// --------------------------------------------------------------------------
// <copyright file="Request.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   
// </history>

namespace Fanex.Security.SpamFilter.Entity
{
    using System;

    /// <summary>
    /// Represent a request object.
    /// </summary>
    [Serializable]
    internal class Request : IRequest
    {
        public Request()
        {
            this.Weight = 1;
        }

        /// <summary>
        /// Gets or sets a value indicating unique identity for current request (SHA1 implementation, a Base64 string).
        /// </summary>
        public string Key
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets a value indicating timestamp when request reaches server.
        /// </summary>
        public DateTime RequestTime
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets a value indicating token to group request.
        /// </summary>
        public string Token
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets a value indicating increment use.
        /// </summary>
        public ulong Weight
        {
            get;
            set;
        }
    }
}
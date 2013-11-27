// --------------------------------------------------------------------------
// <copyright file="IRequest.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   
// </history>

namespace Fanex.Security.SpamFilter.Entity
{
    using System;

    internal interface IRequest
    {
        /// <summary>
        /// Gets or sets a value indicating unique identity for request (SHA1 implementation, a Base64 string).
        /// </summary>
        string Key
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets a value indicating timestamp when the request reach server.
        /// </summary>
        DateTime RequestTime
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets a value indicating token to group request.
        /// </summary>
        string Token
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets a value indicating current increment count.
        /// </summary>
        ulong Weight
        {
            get;
            set;
        }
    }
}
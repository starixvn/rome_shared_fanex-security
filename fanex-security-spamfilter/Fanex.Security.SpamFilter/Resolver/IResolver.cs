// --------------------------------------------------------------------------
// <copyright file="IResolver.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   
// </history>

namespace Fanex.Security.SpamFilter.Resolver
{
    using System.Web;

    /// <summary>
    /// Resolver interface.
    /// </summary>
    public interface IResolver
    {
        /// <summary>
        /// Accept current request.
        /// </summary>
        /// <param name="context">Current HttpContext.</param>
        void Accept(HttpContext context);

        /// <summary>
        /// Reject  current request.
        /// </summary>
        /// <param name="context">Current HttpContext.</param>
        void Reject(HttpContext context);
    }
}
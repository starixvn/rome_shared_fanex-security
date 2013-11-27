// --------------------------------------------------------------------------
// <copyright file="IFilter.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   
// </history>

namespace Fanex.Security.SpamFilter.Filter
{
    using System.Web;

    /// <summary>
    /// IFilter interface.
    /// </summary>
    public interface IFilter
    {
        /// <summary>
        /// Return identity string need to filter (session id, IP, user id...).
        /// </summary>
        /// <param name="current">Current HttpContext.</param>
        /// <returns>Return a identity string.</returns>
        string GetIdentity(HttpContext current);
    }
}
// --------------------------------------------------------------------------
// <copyright file="IPFilter.cs" company="Nexcel Solutions Vietnam">
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
    /// Filter by IP.
    /// </summary>
    public class IPFilter : IFilter
    {
        public string GetIdentity(HttpContext current)
        {
            return current.Request.UserHostAddress;
        }
    }
}
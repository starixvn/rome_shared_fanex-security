// --------------------------------------------------------------------------
// <copyright file="IdFilter.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   
// </history>

namespace Fanex.Security.SpamFilter.Filter
{
    using System.Web;
    using System.Web.Helpers;

    /// <summary>
    /// Filter by custom ID.
    /// </summary>
    public class IdFilter : IFilter
    {
        public string GetIdentity(HttpContext current)
        {
            return "UID";
        }
    }
}
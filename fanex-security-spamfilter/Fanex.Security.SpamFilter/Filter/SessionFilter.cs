// --------------------------------------------------------------------------
// <copyright file="SessionFilter.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   
// </history>

namespace Fanex.Security.SpamFilter.Filter
{
    using System;
    using System.Web;

    /// <summary>
    /// Session filter.
    /// </summary>
    public class SessionFilter : IFilter
    {
        public string GetIdentity(HttpContext current)
        {
            return current.Session.SessionID;
        }
    }
}
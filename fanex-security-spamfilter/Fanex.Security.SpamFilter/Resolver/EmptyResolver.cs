// --------------------------------------------------------------------------
// <copyright file="EmptyResolver.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   
// </history>

namespace Fanex.Security.SpamFilter.Resolver
{
    using System;

    /// <summary>
    /// Empty resolver.
    /// </summary>
    public class EmptyResolver : IResolver
    {
        public void Accept(System.Web.HttpContext context)
        {
            // Do nothing
        }

        public void Reject(System.Web.HttpContext context)
        {
            // Do nothing
        }
    }
}
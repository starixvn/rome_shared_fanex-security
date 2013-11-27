// --------------------------------------------------------------------------
// <copyright file="PlainTextResolver.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   
// </history>

namespace Fanex.Security.SpamFilter.Resolver
{
    using System;
    using System.Collections.Generic;
    using System.Web;

    /// <summary>
    /// Plain text resolver.
    /// </summary>
    public class PlainTextResolver : IResolver
    {
        public void Accept(HttpContext context)
        {
        }

        public void Reject(HttpContext context)
        {
            context.Response.ContentType = "text/plain; charset=utf-8";
            context.Response.StatusDescription = "No content";
            context.Response.StatusCode = 204;
            context.Response.Write("No content.");
            context.Response.End();
        }
    }
}
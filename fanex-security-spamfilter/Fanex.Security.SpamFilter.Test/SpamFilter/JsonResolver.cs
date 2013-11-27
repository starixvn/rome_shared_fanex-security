// --------------------------------------------------------------------------
// <copyright file="JsonResolver.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   
// </history>

namespace Fanex.Security.SpamFilter.Resolver
{
    using System.Web;
    using System.Web.Helpers;

    /// <summary>
    /// Custom resolver.
    /// </summary>
    public class JsonResolver : IResolver
    {
        /// <summary>
        /// Handling rejected request.
        /// </summary>
        /// <param name="context">Current HttpContext.</param>
        public void Reject(HttpContext context)
        {
            string result = Json.Encode(
                new 
                { 
                    errors = new string[] { "Hi " + (context.Session["Username"] ?? "there") + ". Access denied." }
                });

            context.Response.ContentType = "application/json; charset=utf-8";
            context.Response.Write(result);
            context.Response.End();
            context.Response.Close();
        }

        public void Accept(HttpContext context)
        {
        }
    }
}
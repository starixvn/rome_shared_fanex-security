// --------------------------------------------------------------------------
// <copyright file="ICounterAdvisor.cs" company="Nexcel Solutions Vietnam">
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
    /// Counter advisor.
    /// </summary>
    public interface ICounterAdvisor
    {
        /// <summary>
        /// Return true indicate current request should be counted.
        /// </summary>
        /// <param name="context">Current HttpContext.</param>
        /// <returns>Return a success value.</returns>
        bool NeedCount(HttpContext context);
    }
}
// ***********************************************************************
// Assembly         : Fanex.Security.Captcha
// Author           : steven.nguyen
// Created          : 04-18-2014
//
// Last Modified By : steven.nguyen
// Last Modified On : 04-18-2014
// ***********************************************************************
// <copyright file="Global.asax.cs" company="Nexcel">
//     Copyright (c) . All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************

namespace Fanex.Security.Captcha
{
    using System.Web.Mvc;
    using System.Web.Routing;
    using Fanex.Security.Captcha.App_Start;

    /// <summary>
    /// Class MVC Application.
    /// </summary>
    public class MvcApplication : System.Web.HttpApplication
    {
        /// <summary>
        /// Application_s the start.
        /// </summary>
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
        }
    }
}
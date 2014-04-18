// ***********************************************************************
// Assembly         : Fanex.Security.Captcha
// Author           : Steven.Nguyen
// Created          : 04-18-2014
//
// Last Modified By : Steven.Nguyen
// Last Modified On : 04-18-2014
// ***********************************************************************
// <copyright file="BlockPageController.cs" company="Nexcel">
//     Copyright (c) Nexcel. All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************

namespace Fanex.Security.Captcha.Controllers
{
    using System;
    using System.IO;
    using System.Linq;
    using System.Web.Mvc;
    using Fanex.Security.Captcha.ViewModels;
    using Fanex.Security.SpamFilter;
    using Fanex.Security.SpamFilter.Entity;

    /// <summary>
    /// Class BlockPageController.
    /// </summary>
    public class BlockPageController : Controller
    {
        // GET: /CreateOver100Users/

        /// <summary>
        /// Indexes the specified page name.
        /// </summary>
        /// <param name="pageName">Name of the page.</param>
        /// <returns>ActionResult.</returns>
        public ActionResult Index(string pageName)
        {
            var blocker = (from blockerItem in System.Web.HttpContext.Current.GetSpamFilterBlockers() select blockerItem).FirstOrDefault();
            blocker = blocker ?? new DefaultBlocker
                {
                    Blocked = false, Duration = new TimeSpan(0), BlockedTime = DateTime.Now.AddMinutes(-50)
                };

            return View("~/Areas/SpamFilter/Views/Captcha/PageAccess/BlockPage.cshtml", blocker);
        }

        /// <summary>
        /// Renders the script.
        /// </summary>
        /// <returns>JavaScriptResult.</returns>
        public JavaScriptResult RenderScript()
        {
            var pageName = Request["rule"] ?? string.Empty;
            var rootURL = Request["RootURL"] ?? string.Empty;
            string blockView = "~/Areas/SpamFilter/Views/Captcha/PageAccess/Block.cshtml";
            var test = Render(this, blockView, new PageInfo { PageName = pageName, RootURL = rootURL });
            return JavaScript(test);
        }

        /// <summary>
        /// Renders the specified controller.
        /// </summary>
        /// <param name="controller">The controller.</param>
        /// <param name="viewName">Name of the view.</param>
        /// <param name="model">The model.</param>
        /// <returns>System.String.</returns>
        public string Render(Controller controller, string viewName, object model)
        {
            controller.ViewData.Model = model;

            using (var sw = new StringWriter())
            {
                var viewResult = ViewEngines.Engines.FindPartialView(controller.ControllerContext, viewName);
                var viewContext = new ViewContext(controller.ControllerContext, viewResult.View, controller.ViewData, controller.TempData, sw);
                viewResult.View.Render(viewContext, sw);
                viewResult.ViewEngine.ReleaseView(controller.ControllerContext, viewResult.View);
                return sw.GetStringBuilder().ToString();
            }
        }

        /// <summary>
        /// Class Default Blocker.
        /// </summary>
        public class DefaultBlocker : IRequestBlocker
        {
            /// <summary>
            /// Gets or sets the blocked time.
            /// </summary>
            /// <value>The blocked time.</value>
            public DateTime BlockedTime { get; set; }

            /// <summary>
            /// Gets or sets the duration.
            /// </summary>
            /// <value>The duration.</value>
            public TimeSpan Duration { get; set; }

            /// <summary>
            /// Gets the threshold.
            /// </summary>
            /// <value>The threshold.</value>
            public int Threshold { get; private set; }

            /// <summary>
            /// Gets the interval.
            /// </summary>
            /// <value>The interval.</value>
            public TimeSpan Interval { get; private set; }

            /// <summary>
            /// Gets or sets a value indicating whether this <see cref="DefaultBlocker"/> is blocked.
            /// </summary>
            /// <value><c>true</c> if blocked; otherwise, <c>false</c>.</value>
            public bool Blocked { get; set; }

            /// <summary>
            /// Gets the name.
            /// </summary>
            /// <value>The block name.</value>
            public string Name { get; private set; }

            /// <summary>
            /// Gets a value indicating whether this <see cref="DefaultBlocker"/> is expired.
            /// </summary>
            /// <value><c>true</c> if expired; otherwise, <c>false</c>.</value>
            public bool Expired { get; private set; }
        }
    }
}

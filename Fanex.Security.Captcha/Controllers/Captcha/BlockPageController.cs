using System;
using System.Linq;
using System.Web.Mvc;
using System.IO;
using Fanex.Security.Captcha.ViewModels;
using Fanex.Security.SpamFilter;
using Fanex.Security.SpamFilter.Entity;

namespace Fanex.Security.Captcha.Controllers
{
    public class BlockPageController : Controller
    {
        //
        // GET: /CreateOver100Users/

        public ActionResult Index(string pageName)
        {
            
            var blocker = (from blockerItem in System.Web.HttpContext.Current.GetSpamFilterBlockers() select blockerItem).FirstOrDefault();
            //blocker = null;

            blocker = blocker ?? new DefaultBlocker
                {
                    Blocked = false, Duration = new TimeSpan(0), BlockedTime = DateTime.Now.AddMinutes(-50)
                };

            return View("~/Areas/SpamFilter/Views/Captcha/PageAccess/BlockPage.cshtml", blocker);
        }

        public JavaScriptResult RenderScript()
        {
            var pageName = Request["rule"] ?? "";
            var rootURL = Request["RootURL"] ?? "";
            var test = Render(this, "~/Areas/SpamFilter/Views/Captcha/PageAccess/Block.cshtml", new PageInfo { PageName = pageName, RootURL = rootURL });
            return JavaScript(test);
        }

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

        public class DefaultBlocker: IRequestBlocker
        {
            public DateTime BlockedTime { get; set; }
            public TimeSpan Duration { get; set; }
            public int Threshold { get; private set; }
            public TimeSpan Interval { get; private set; }
            public bool Blocked { get; set; }
            public string Name { get; private set; }
            public bool Expired { get; private set; }
        }
    }
}

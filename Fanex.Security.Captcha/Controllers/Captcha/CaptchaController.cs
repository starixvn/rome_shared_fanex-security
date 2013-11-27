using System.Web.Mvc;


namespace Fanex.Security.Captcha.Controllers
{
    public class CaptchaController : BaseController
    {
        private readonly BaseCaptchaController _captchaController;

        public CaptchaController()
        {
            _captchaController = new SylouCaptchaController();
        }
      
        public ActionResult PageAccessCaptcha(string rule)
        {
            return _captchaController.GenerateCaptcha(rule);
        }

        public JavaScriptResult RenderCaptchaScript(string rule, BaseCaptchaController.CaptchaMode mode)
        {

            return _captchaController.RenderCaptchaScript(this, rule, mode);
        }

        [HttpPost]
        [AllowAnonymous]
        public ActionResult CheckCaptcha(string pageName)
        {
            return _captchaController.CheckCaptcha(pageName, HttpContext, RouteData, User.Identity.Name);
        }
       

    }
}

using System.Configuration;

namespace Fanex.Security.Captcha.Configurations
{
    public static class AppSettings
    {
        private static string _recaptchaPrivateKey; 
        private static string _recaptchaPublicKey;      


        static AppSettings()
        {
            _recaptchaPrivateKey = ConfigurationManager.AppSettings["RecaptchaPrivateKey"];
            _recaptchaPublicKey = ConfigurationManager.AppSettings["RecaptchaPublicKey"];
        }

        public static string RecaptchaPublicKey
        {
            get {return _recaptchaPublicKey;}
        }

        public static string RecaptchaPrivateKey
        {
            get {return _recaptchaPrivateKey;}
        }
    }
}
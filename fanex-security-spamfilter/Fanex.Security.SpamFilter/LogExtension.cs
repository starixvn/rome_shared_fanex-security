namespace Fanex.Security.SpamFilter
{
    using System;
    using System.Threading;
    using System.Web;

    public static class LogExtension
    {
        public static string ExceptionToString(Exception ex, HttpContext context)
        {
            if (context == null)
            {
                return "HttpContext Is Null";
            }

            return ExceptionToString(ex, context.Request);
        }

        public static string ExceptionToString(Exception ex, HttpRequest request)
        {
            if (ex == null || request == null)
            {
                return "Ex or Request Is Null";
            }

            string message = string.Join(
                "\r\n",
                ex.Message,
                "URL: " + request.Url.OriginalString,
                "REFERRER: " + request.UrlReferrer,
                "FORM: " + request.Form.ToString(),
                "SOURCE: " + ex.Source,
                "TARGETSITE: " + ex.TargetSite,
                "DETAIL: " + ex,
                "STACKTRACE: " + ex.StackTrace,
                "CULTURE: " + Thread.CurrentThread.CurrentUICulture,
                "BROWSER CAPABILITIES:" + ClientInformation(request));

            return message;
        }

        private static string ClientInformation(HttpRequest request)
        {
            if (request.Browser == null)
            {
                return string.Empty;
            }

            var browser = request.Browser;
            var browserInformation = string.Join(
                "\r\n",
                "Type: " + browser.Type,
                "Name: " + browser.Browser,
                "Version: " + browser.Version,
                "Major Version: " + browser.MajorVersion,
                "Minor Version: " + browser.MinorVersion,
                "Platform: " + browser.Platform,
                "Is Beta: " + browser.Beta,
                "Is Crawler: " + browser.Crawler,
                "Is AOL: " + browser.AOL,
                "Is Win16: " + browser.Win16,
                "Is Win32: " + browser.Win32,
                "Supports Frames: " + browser.Frames,
                "Supports Tables: " + browser.Tables,
                "Supports Cookies: " + browser.Cookies);

            return browserInformation;
        }
    }
}
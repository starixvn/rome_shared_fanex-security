using System;

namespace Fanex.Security.Captcha.Extensions
{
    /// <summary>
    /// Log handler
    /// </summary>
    public class LogExtension
    {
        public static void Log(Exception ex)
        {
            Fanex.Logging.LogService.Error(ex.Message, ex);
        }
    }
}
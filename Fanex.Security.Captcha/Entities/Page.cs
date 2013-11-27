using System;

namespace Fanex.Security.Captcha.Entities
{
    [Serializable]
    public class Page
    {
        public enum Status
        {
            Opening,
            Blocked,
            Captcha
        }

        public Status PageStatus { get; set; }

    }
}
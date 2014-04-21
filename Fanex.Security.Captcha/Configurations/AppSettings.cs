// ***********************************************************************
// Assembly         : Fanex.Security.Captcha
// Author           : Steven.Nguyen
// Created          : 04-18-2014
//
// Last Modified By : Steven.Nguyen
// Last Modified On : 04-18-2014
// ***********************************************************************
// <copyright file="AppSettings.cs" company="Nexcel">
//     Copyright (c) Nexcel. All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************

namespace Fanex.Security.Captcha.Configurations
{
    using System.Configuration;

    /// <summary>
    /// Class AppSettings.
    /// </summary>
    public static class AppSettings
    {
        /// <summary>
        /// The captcha private key.
        /// </summary>
        private static string _recaptchaPrivateKey;

        /// <summary>
        /// The captcha public key.
        /// </summary>
        private static string _recaptchaPublicKey;

        /// <summary>
        /// Initializes static members of the <see cref="AppSettings"/> class.
        /// </summary>
        static AppSettings()
        {
            _recaptchaPrivateKey = ConfigurationManager.AppSettings["RecaptchaPrivateKey"];
            _recaptchaPublicKey = ConfigurationManager.AppSettings["RecaptchaPublicKey"];
        }

        /// <summary>
        /// Gets the captcha public key.
        /// </summary>
        /// <value>The captcha public key.</value>
        public static string RecaptchaPublicKey
        {
            get { return _recaptchaPublicKey; }
        }

        /// <summary>
        /// Gets the captcha private key.
        /// </summary>
        /// <value>The captcha private key.</value>
        public static string RecaptchaPrivateKey
        {
            get { return _recaptchaPrivateKey; }
        }
    }
}
// ***********************************************************************
// Assembly         : Fanex.Security.Captcha
// Author           : Steven.Nguyen
// Created          : 04-18-2014
//
// Last Modified By : Steven.Nguyen
// Last Modified On : 04-18-2014
// ***********************************************************************
// <copyright file="LogExtension.cs" company="Nexcel">
//     Copyright (c) Nexcel. All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************

namespace Fanex.Security.Captcha.Extensions
{
    using System;

    /// <summary>
    /// Class LogExtension.
    /// </summary>
    public class LogExtension
    {
        /// <summary>
        /// Logs the specified ex.
        /// </summary>
        /// <param name="ex">The exception instance.</param>
        public static void Log(Exception ex)
        {
            Fanex.Logging.LogService.Error(ex.Message, ex);
        }
    }
}
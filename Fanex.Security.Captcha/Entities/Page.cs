// ***********************************************************************
// Assembly         : Fanex.Security.Captcha
// Author           : Steven.Nguyen
// Created          : 04-18-2014
//
// Last Modified By : Steven.Nguyen
// Last Modified On : 04-18-2014
// ***********************************************************************
// <copyright file="Page.cs" company="Nexcel">
//     Copyright (c) Nexcel. All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************

namespace Fanex.Security.Captcha.Entities
{
    using System;

    /// <summary>
    /// Class Page.
    /// </summary>
    [Serializable]
    public class Page
    {
        /// <summary>
        /// Enum Status.
        /// </summary>
        public enum Status
        {
            /// <summary>
            /// The opening.
            /// </summary>
            Opening,

            /// <summary>
            /// The blocked.
            /// </summary>
            Blocked,

            /// <summary>
            /// The captcha.
            /// </summary>
            Captcha
        }

        /// <summary>
        /// Gets or sets the page status.
        /// </summary>
        /// <value>The page status.</value>
        public Status PageStatus { get; set; }
    }
}
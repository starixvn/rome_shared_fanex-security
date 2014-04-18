// ***********************************************************************
// Assembly         : Fanex.Security.Captcha
// Author           : Steven.Nguyen
// Created          : 04-18-2014
//
// Last Modified By : Steven.Nguyen
// Last Modified On : 04-18-2014
// ***********************************************************************
// <copyright file="User.cs" company="Nexcel">
//     Copyright (c) Nexcel. All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************

namespace Fanex.Security.Captcha.Entities
{
    using System;

    /// <summary>
    /// Class User.
    /// </summary>
    [Serializable]
    public class User
    {
        /// <summary>
        /// Gets or sets the username.
        /// </summary>
        /// <value>The username.</value>
        public string Username { get; set; }

        /// <summary>
        /// Gets or sets the identifier.
        /// </summary>
        /// <value>The identifier.</value>
        public int Id { get; set; }
    }
}
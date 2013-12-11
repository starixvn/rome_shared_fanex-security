// --------------------------------------------------------------------------
// <copyright file="RequestBlockerFactory.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   
// </history>

namespace Fanex.Security.SpamFilter.Factory
{
    using System;

    using Fanex.Security.SpamFilter.Entity;

    /// <summary>
    /// Implementation of IRequestBlocker factory.
    /// </summary>
    internal static class RequestBlockerFactory
    {
        public static IRequestBlocker Create(RuleElement rule)
        {
            RequestBlocker blocker = new RequestBlocker();
            blocker.BlockedTime = DateTime.MinValue.ToUniversalTime();
            blocker.Duration = TimeSpan.FromMinutes(rule.BlockDuration);
            blocker.Name = rule.Name;
            blocker.Threshold = rule.Threshold;
            blocker.Interval = TimeSpan.FromMinutes(rule.Minutes);

            return blocker as IRequestBlocker;
        }
    }
}
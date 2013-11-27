// --------------------------------------------------------------------------
// <copyright file="IPersistentRepository.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   
// </history>

namespace Fanex.Security.SpamFilter.Repository
{
    using System;

    /// <summary>
    /// Safe persistence storage interface.
    /// </summary>
    internal interface IPersistentRepository : IDisposable
    {
        /// <summary>
        /// PreDispose event handler.
        /// </summary>
        event EventHandler OnPreDispose;

        /// <summary>
        /// Atomic decrement unsigned long value.
        /// </summary>
        /// <param name="key">Key to access/store value.</param>
        /// <param name="delta">Delta to decrease.</param>
        /// <returns>Return an integer value.</returns>
        ulong Decrement(string key, ulong delta = 1);

        /// <summary>
        /// Get a value.
        /// </summary>
        /// <param name="key">Key to access/store value.</param>
        /// <param name="type">Type of value need to get.</param>
        /// <returns>Return an object.</returns>
        object Get(string key, Type type);

        /// <summary>
        /// Atomic increment unsigned long value.
        /// </summary>
        /// <param name="key">Key to access/store value.</param>
        /// <param name="delta">Delta to increase.</param>
        /// <param name="minutes">Expired period in minutes.</param>
        /// <returns>Return an integer value.</returns>
        ulong Increment(string key, ulong delta = 1, int minutes = 1440);

        /// <summary>
        /// Lock current key string.
        /// Asynchronous: no.
        /// </summary>
        /// <param name="key">Key to lock.</param>
        /// <param name="owner">Unique string owner.</param>
        /// <returns>true -> lock successful otherwise timeout.</returns>
        bool Lock(string key, string owner);

        /// <summary>
        /// Set a value.
        /// </summary>
        /// <param name="key">Key to access/store value.</param>
        /// <param name="value">Object value.</param>
        /// <param name="minutes">Expired period in minutes.</param>
        void Set(string key, object value, int minutes = 1440);

        /// <summary>
        /// Unlock current key string.
        /// Asynchronous: no.
        /// </summary>
        /// <param name="key">Key to lock.</param>
        /// <param name="owner">Unique string owner.</param>
        /// <returns>true -> lock successful otherwise timeout.</returns>
        bool Unlock(string key, string owner);

        /// <summary>
        /// Set current value to 0.
        /// </summary>
        /// <param name="key">Set to zero.</param>
        void Zero(string key);
    }
}
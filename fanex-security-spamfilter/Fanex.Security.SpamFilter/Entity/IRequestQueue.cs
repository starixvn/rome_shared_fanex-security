// --------------------------------------------------------------------------
// <copyright file="IRequestQueue.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   
// </history>

namespace Fanex.Security.SpamFilter.Entity
{
    using System;
    using System.Collections;

    using Fanex.Security.SpamFilter.Repository;

    /// <summary>
    /// Keep a number of object base on start time and end time.
    /// </summary>
    /// <typeparam name="T">The first generic type parameter.</typeparam>
    internal interface IRequestQueue<T> : ICollection, IDisposable
    {
        /// <summary>
        /// Gets a value indicating whether maximum number of increment over the threshold in a timespan time.
        /// </summary>
        bool Blocked
        {
            get;
        }

        IRequestBlocker Blocker
        {
            get;
        }

        ulong RemoteTotal
        {
            get;
        }

        /// <summary>
        /// Gets a value indicating time period that limit max hit count.
        /// </summary>
        TimeSpan Interval
        {
            get;
        }

        /// <summary>
        /// Gets a value indicating unique id for the queue.
        /// </summary>
        string Key
        {
            get;
        }

        /// <summary>
        /// Gets a value indicating last time that the queue is touched.
        /// </summary>
        DateTime LastChecked
        {
            get;
        }

        string Name
        {
            get;
        }

        IPersistentRepository Repository
        {
            get;
        }

        /// <summary>
        /// Gets a value indicating maximum number of request per sliding time.
        /// </summary>
        int Threshold
        {
            get;
        }

        /// <summary>
        ///  Removes all objects from the System.Collections.Queue.
        /// </summary>
        void Clear();

        /// <summary>
        /// Determines whether an element is in the System.Collections.Queue.
        /// </summary>
        /// <param name="item">Item need to be checked.</param>
        /// <returns>Return true false.</returns>
        bool Contains(T item);

        /// <summary>
        /// Removes and returns the object at the beginning of the System.Collections.Queue.
        /// </summary>
        /// <returns>Return an item.</returns>
        T Dequeue();

        /// <summary>
        /// Adds an object to the end of the System.Collections.Queue.
        /// </summary>
        /// <param name="item">Item need to enqueue.</param>
        void Enqueue(T item);

        /// <summary>
        /// Loop through last item, check whether it is expired and take it away queue.
        /// </summary>
        void Synchronize();

        /// <summary>
        /// Get current token from repository.
        /// </summary>
        /// <returns>Return a token string.</returns>
        string GetToken();

        /// <summary>
        /// Create new token for repository.
        /// </summary>
        void NewToken();

        /// <summary>
        /// Returns the object at the beginning of the System.Collections.Queue without.
        /// removing it.
        /// </summary>
        /// <returns>Return an item.</returns>
        T Peek();

        /// <summary>
        /// Set remote blocker.
        /// </summary>
        /// <param name="blocker">Blocker object need to be set.</param>
        void SetBlocker(IRequestBlocker blocker);
    }
}
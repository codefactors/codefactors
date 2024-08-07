// Copyright (c) 2024, Codefactors Ltd.
//
// Codefactors Ltd licenses this file to you under the following license(s):
//
//   * The MIT License, see https://opensource.org/license/mit/

using System.Collections.ObjectModel;

namespace Codefactors.DataFabric;

/// <summary>
/// Provides a thread-safe collection that supports concurrent read and write operations.
/// </summary>
/// <typeparam name="T">Type of item for the collection.</typeparam>
/// <remarks>
/// Initializes a new instance of the <see cref="ConcurrentCollection{T}"/> class.
/// </remarks>
/// <param name="lockTimeout">Optional lock timeout in milliseconds. (Default is one second.)</param>
public class ConcurrentCollection<T>(in int lockTimeout = ConcurrentCollection<T>.DefaultLockTimeout)
    where T : class, IEquatable<T>
{
    private const int DefaultLockTimeout = 1000;

    private readonly int _lockTimeout = lockTimeout;
    private readonly Collection<T> _items = [];
    private readonly ReaderWriterLockSlim _lock = new ReaderWriterLockSlim();

    /// <summary>
    /// Attempts to add an item to the collection.
    /// </summary>
    /// <param name="item">Item to add.</param>
    /// <param name="avoidDuplicates">Set to true to prevent duplicate items being added to the collection. Items
    /// must comparable (see generic type constraints).</param>
    /// <returns>True if the item was successfully added; false otherwise.</returns>
    public bool TryAdd(in T item, bool avoidDuplicates = false)
    {
        if (_lock.TryEnterWriteLock(_lockTimeout))
        {
            try
            {
                if (avoidDuplicates && _items.Contains(item))
                    return false;

                _items.Add(item);
            }
            finally
            {
                _lock.ExitWriteLock();
            }

            return true;
        }

        return false;
    }

    /// <summary>
    /// Attempts to remove the specified item from the collection.
    /// </summary>
    /// <param name="item">Item to be removed.</param>
    /// <returns>True if the item could be removed; false otherwise.</returns>
    public bool TryRemove(in T item)
    {
        if (_lock.TryEnterWriteLock(_lockTimeout))
        {
            try
            {
                if (!_items.Contains(item))
                    return false;

                _items.Remove(item);
            }
            finally
            {
                _lock.ExitWriteLock();
            }

            return true;
        }

        return false;
    }

    /// <summary>
    /// Clears the collection.
    /// </summary>
    /// <exception cref="InvalidOperationException">Thrown if the collection cannot be locked.</exception>
    public void Clear()
    {
        if (_lock.TryEnterWriteLock(_lockTimeout))
        {
            try
            {
                _items.Clear();
            }
            finally
            {
                _lock.ExitWriteLock();
            }

            return;
        }

        throw new InvalidOperationException("Unable to clear collection; failed to get writer lock within timeout period");
    }

    /// <summary>
    /// Iterates over each item in the collection, invoking the specified action for each item.
    /// </summary>
    /// <param name="action">Action to execute against each item in the collection.</param>
    /// <exception cref="AggregateException">Thrown if one or more action invocations throws.</exception>
    /// <remarks>Note that this method locks the collection while it iterates over all the items.  Care should be taken
    /// not to invoke actions that could result in a deadline, for example, calling another method on the collection.</remarks>
    public void ForEach(in Action<T> action)
    {
        var exceptions = new List<Exception>();

        if (_lock.TryEnterReadLock(_lockTimeout))
        {
            try
            {
                foreach (var item in _items)
                {
                    try
                    {
                        action(item);
                    }
                    catch (Exception ex)
                    {
                        exceptions.Add(ex);
                    }
                }
            }
            finally
            {
                _lock.ExitReadLock();
            }
        }

        if (exceptions.Count != 0)
            throw new AggregateException("One or more cxceptions thrown during collection ForEach operation", exceptions);
    }

    /// <summary>
    /// Iterates over each item in the collection, invoking the specified asynchronous action for each item.
    /// </summary>
    /// <param name="action">Async action to invoke against each item in the collection.</param>
    /// <returns>A <see cref="Task"/>.</returns>
    /// <exception cref="InvalidOperationException">Thrown if the collection cannot be locked.</exception>
    /// <exception cref="AggregateException">Thrown if one or more action invocations throws.</exception>
    /// <remarks>Implementation note: As it it not possible to await within a lock, we need to copy the collection
    /// and then iterate over the copy, awaiting each item in turn. This avoids possible deadlock scenarios.
    /// </remarks>
    public async Task ForEachAsync(/* in */ Func<T, Task> action)
    {
        T[] itemsCopy;

        if (_lock.TryEnterReadLock(_lockTimeout))
        {
            try
            {
                itemsCopy = new T[_items.Count];

                _items.CopyTo(itemsCopy, 0);
            }
            finally
            {
                _lock.ExitReadLock();
            }
        }
        else
        {
            throw new InvalidOperationException("Unable to lock collection for reading within timeout period");
        }

        var exceptions = new List<Exception>();

        foreach (var item in itemsCopy)
        {
            try
            {
                await action(item);
            }
            catch (Exception ex)
            {
                exceptions.Add(ex);
            }
        }

        if (exceptions.Count != 0)
            throw new AggregateException("One or more exceptions thrown during collection ForEachAsync operation", exceptions);
    }
}
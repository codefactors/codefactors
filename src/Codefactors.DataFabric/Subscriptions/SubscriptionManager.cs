// Copyright (c) 2024, Codefactors Ltd.
//
// Codefactors Ltd licenses this file to you under the following license(s):
//
//   * The MIT License, see https://opensource.org/license/mit/

using Codefactors.Common.Model;
using Codefactors.DataFabric.Diagnostics;
using Codefactors.DataFabric.Reflection;
using Microsoft.Extensions.Primitives;
using System.Collections.Concurrent;

namespace Codefactors.DataFabric.Subscriptions;

/// <summary>
/// Subscription manager entity.
/// </summary>
/// <param name="subscriptionMatcher">Subscription matcher.</param>
/// <param name="subscriptionFactory">Subscription factory.</param>
/// <param name="logger">Logger for diagnostic logging.</param>
public class SubscriptionManager(
    SubscriptionMatcher subscriptionMatcher,
    ISubscriptionFactory subscriptionFactory,
    ILogger<SubscriptionManager> logger)
    : ISubscriptionManager
{
    private readonly SubscriptionMatcher _subscriptionMatcher = subscriptionMatcher;
    private readonly ISubscriptionFactory _subscriptionFactory = subscriptionFactory;
    private readonly ConcurrentDictionary<string, SubscriptionCollection> _subscriptions = new ConcurrentDictionary<string, SubscriptionCollection>();
    private readonly ILogger _logger = logger;

    /// <summary>
    /// Gets the invocation that matches the supplied path.
    /// </summary>
    /// <param name="path">Subscription path.</param>
    /// <returns>Invocation helper that pertains to the specified subcription path.</returns>
    /// <exception cref="SubscriptionException">>Thrown if the subscription path cannot be matched.</exception>
    public InvocationHelper MatchPath(string path) => _subscriptionMatcher.Match(path);

    /// <summary>
    /// Adds a subscription.
    /// </summary>
    /// <param name="requestContext">Request context.</param>
    /// <param name="path">Subscription path.</param>
    /// <param name="queryParameters">Array containing query parameters as key/value pairs. May be empty.</param>
    /// <returns>Current data for the subscription.</returns>
    public async Task<object> AddSubscriptionAsync(
        IRequestContext requestContext,
        string path,
        IEnumerable<KeyValuePair<string, StringValues>> queryParameters)
    {
        try
        {
            var invocationData = MatchPath(path) ??
                throw new SubscriptionException($"No matching path found for '{path}'");

            var result = await invocationData.InvokeAsync(requestContext, queryParameters);

            var collection = _subscriptions.GetOrAdd(path, _ => new SubscriptionCollection());

            var subscription = _subscriptionFactory.Create(requestContext, path);

            var added = collection.TryAdd(subscription, true);

            _logger.LogInformation("Subscription added: {path}, {subscription}; already present = {added}", path, subscription, !added);

            return result;
        }
        catch (ArgumentException ex)
        {
            _logger.LogError(ex, "Failed to add subscription");

            throw new SubscriptionException(ex.Message);
        }
    }

    /// <summary>
    /// Removes a subscription.
    /// </summary>
    /// <param name="requestContext">Request context.</param>
    /// <param name="path">Subscription path.</param>
    /// <returns><see cref="Task"/>.</returns>
    public Task RemoveSubscriptionAsync(
        IRequestContext requestContext,
        string path)
    {
        if (_subscriptions.TryGetValue(path, out var subscribers))
        {
            var subscription = _subscriptionFactory.Create(requestContext, path);

            subscribers.TryRemove(subscription);
        }

        return Task.CompletedTask;
    }

    /// <summary>
    /// Notifies subscribers of an update.
    /// </summary>
    /// <param name="path">Subscription path.</param>
    /// <param name="update">Update content.</param>
    /// <returns><see cref="Task"/>.</returns>
    public async Task NotifySubscribersAsync(string path, object update)
    {
        try
        {
            if (_subscriptions.TryGetValue(path, out var subscribers))
                await subscribers.NotifyAllAsync(path, update);
        }
        catch (AggregateException ex)
        {
            _logger.LogError(ex, "One or more notifications failed");
        }
    }
}
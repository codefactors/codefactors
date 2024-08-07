// Copyright (c) 2024, Codefactors Ltd.
//
// Codefactors Ltd licenses this file to you under the following license(s):
//
//   * The MIT License, see https://opensource.org/license/mit/

using Microsoft.Extensions.Logging;
using System.Collections.Concurrent;

namespace Codefactors.DataFabric.Subscriptions;

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

    public InvocationHelper MatchPath(string path) => _subscriptionMatcher.Match(path);

    public async Task<object> AddSubscriptionAsync(IRequestContext requestContext, string path)
    {
        try
        {
            var invocationData = MatchPath(path) ??
                throw new SubscriptionException($"No matching path found for '{path}'");

            var result = await invocationData.InvokeAsync(requestContext);

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

    public Task RemoveSubscriptionAsync(IRequestContext requestContext, string path)
    {
        if (_subscriptions.TryGetValue(path, out var subscribers))
        {
            var subscription = _subscriptionFactory.Create(requestContext, path);

            subscribers.TryRemove(subscription);
        }

        return Task.CompletedTask;
    }

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

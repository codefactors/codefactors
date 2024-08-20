// Copyright (c) 2024, Codefactors Ltd.
//
// Codefactors Ltd licenses this file to you under the following license(s):
//
//   * The MIT License, see https://opensource.org/license/mit/

using Codefactors.Common.Model;
using Codefactors.DataFabric.Diagnostics;
using Codefactors.DataFabric.Subscriptions;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Xunit.Abstractions;

namespace Codefactors.DataFabric.Tests;

public class SubscriptionManagerTests(ITestOutputHelper outputHelper)
{
    private readonly ITestOutputHelper _outputHelper = outputHelper;

    [Fact]
    public async Task TestAddSubscriptions()
    {
        var entityObject = new TestEntityObject(_outputHelper.WriteLine);

        var entities = new SubscriptionDataSource[]
        {
            new SubscriptionDataSource(entityObject, "GetEmployers"),
            new SubscriptionDataSource(entityObject, "GetEmployer"),
            new SubscriptionDataSource(entityObject, "GetEmployees"),
            new SubscriptionDataSource(entityObject, "GetEmployee"),
        };

        var updates = new Stack<object>();
        var matcher = new SubscriptionMatcher(CreateSubscriptionTree(entities));
        var factory = new SubscriptionFactory(updates, _outputHelper);
        var logger = MakeLogger<SubscriptionManager>();

        var manager = new SubscriptionManager(matcher, factory, logger);

        var path = "employers/d2cfbaf7-a83e-426d-825a-0e0994cbe076/employees/a1ef7e3b-bf25-409e-a260-6f0c51c1af68";
        var path2 = "employers/d2cfbaf7-a83e-426d-825a-0e0994cbe076";

        var result = await manager.AddSubscriptionAsync(
            new RequestContext("user_2hSSGlTnDZZuAyd2I0sOKYyhI9M.sess_2hVkIPElDgWGXoeGSapN4ntLMdV"),
            path,
            []);

        result.Should().Be("some employee");

        var result2 = await manager.AddSubscriptionAsync(
            new RequestContext("user_3hSSGlTnDZZuAyd2I0sOKYyhI9M.sess_2hVkIPElDgWGXoeGSapN4ntLMdX"),
            path,
            []);

        result2.Should().Be("some employee");

        var result3 = await manager.AddSubscriptionAsync(
            new RequestContext("user_3hSSGlTnDZZuAyd2I0sOKYyhI9M.sess_2hVkIPElDgWGXoeGSapN4ntLMdX"),
            path2,
            []);

        result3.Should().Be("some employer");

        await manager.NotifySubscribersAsync(path, new { Message = "Hello, World!" });
        await manager.NotifySubscribersAsync(path2, new { Message = "Hello, World 2!" });

        updates.Count.Should().Be(3);
        updates.Pop().Should().BeEquivalentTo(new { Message = "Hello, World 2!" });
        updates.Pop().Should().BeEquivalentTo(new { Message = "Hello, World!" });
        updates.Pop().Should().BeEquivalentTo(new { Message = "Hello, World!" });
    }

    [Fact]
    public async Task AddInvalidSubscriptions()
    {
        var entityObject = new TestEntityObject(_outputHelper.WriteLine);

        var entities = new SubscriptionDataSource[]
        {
            new SubscriptionDataSource(entityObject, "GetEmployers"),
            new SubscriptionDataSource(entityObject, "GetEmployer"),
            new SubscriptionDataSource(entityObject, "GetEmployees"),
            new SubscriptionDataSource(entityObject, "GetEmployee"),
        };

        var updates = new Stack<object>();
        var matcher = new SubscriptionMatcher(CreateSubscriptionTree(entities));
        var factory = new SubscriptionFactory(updates, _outputHelper);
        var logger = MakeLogger<SubscriptionManager>();

        var manager = new SubscriptionManager(matcher, factory, logger);

        var path = "employers/hampster/employees";

        Func<Task> act = async () => await manager.AddSubscriptionAsync(
            new RequestContext("user_2hSSGlTnDZZuAyd2I0sOKYyhI9M.sess_2hVkIPElDgWGXoeGSapN4ntLMdV"),
            path,
            []);

        await act.Should().ThrowAsync<SubscriptionException>()
            .WithMessage("Invalid value 'hampster' for parameter 'employerId'");
    }

    private SubscriptionTree CreateSubscriptionTree(SubscriptionDataSource[] entities)
    {
        var tree = new SubscriptionTree();

        tree.RegisterSubscriptionPath("employers", entities[0]);
        tree.RegisterSubscriptionPath("employers/{employerId}", entities[1]);
        tree.RegisterSubscriptionPath("employers/{employerId}/employees", entities[2]);
        tree.RegisterSubscriptionPath("employers/{employerId}/employees/{employeeId}", entities[3]);

        return tree;
    }

    private ILogger<T> MakeLogger<T>() where T : class
    {
        var provider = new TestLoggerProvider<T>(_outputHelper);

        return provider.CreateLogger(nameof(SubscriptionManager)) as ILogger<T> ??
                throw new InvalidOperationException("Unable to create logger for test");
    }
}

internal class TestLoggerProvider<T>(ITestOutputHelper outputHelper) : ILoggerProvider
{
    private readonly ITestOutputHelper _outputHelper = outputHelper;

    public ILogger CreateLogger(string categoryName)
    {
        return new TestOutputLogger<T>(_outputHelper);
    }

    public void Dispose()
    {
        // No resources to dispose
    }
}

internal class Subscription(string subscriptionKey, Action<object> notification) : ISubscription
{
    private readonly string _sessionId = subscriptionKey;

    public Task NotifyAsync(string subscriptionPath, object update)
    {
        notification(update);

        return Task.CompletedTask;
    }

    bool IEquatable<ISubscription>.Equals(ISubscription? other) =>
         (other as Subscription)?._sessionId == _sessionId;
}

internal class SubscriptionKeyGenerator : ISubscriptionKeyGenerator
{
    public string GenerateKey(IRequestContext requestContext) => requestContext?.ToString() ?? throw new ArgumentNullException(nameof(requestContext));
}

internal class SubscriptionFactory(Stack<object> updates, ITestOutputHelper outputHelper) : ISubscriptionFactory
{
    private readonly Stack<object> _updates = updates;
    private readonly ITestOutputHelper _outputHelper = outputHelper;
    private readonly ISubscriptionKeyGenerator _subscriptionKeyGenerator = new SubscriptionKeyGenerator();

    public ISubscription Create(IRequestContext requestContext, string subscriptionPath)
    {
        var subscriptionKey = _subscriptionKeyGenerator.GenerateKey(requestContext);

        return new Subscription(subscriptionKey, (update) =>
        {
            _updates.Push(update);

            _outputHelper.WriteLine($"Subscriber with key '{subscriptionKey}' received update: {update}");
        });
    }
}

internal class RequestContext : IRequestContext
{
    public object SessionId { get; }

    public object TenantId { get; }

    public object UserId { get; }

    public RequestContext(object sessionId)
    {
        SessionId = sessionId;
        TenantId = new object();
        UserId = new object();
    }

    public override string? ToString()
    {
        return SessionId.ToString();
    }
}
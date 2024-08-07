// Copyright (c) 2024, Codefactors Ltd.
//
// Codefactors Ltd licenses this file to you under the following license(s):
//
//   * The MIT License, see https://opensource.org/license/mit/

using Codefactors.DataFabric.Diagnostics;
using Codefactors.DataFabric.Reflection;
using Codefactors.DataFabric.Subscriptions;
using FluentAssertions;
using Xunit.Abstractions;

namespace Codefactors.DataFabric.Tests;

public class SubscriptionMatcherTests
{
    ITestOutputHelper _outputHelper;

    public SubscriptionMatcherTests(ITestOutputHelper outputHelper)
    {
        _outputHelper = outputHelper;
    }

    [Fact]
    public void TestValidMatches()
    {
        var dataSources = TestDataSource.MakeDataSources((text) => { });

        var tree = MakeSubscriptionTree(dataSources);

        var matcher = new SubscriptionMatcher(tree);

        matcher.Match("/employers")
            .Should().BeEquivalentTo(new InvocationHelper(dataSources[0], new Dictionary<string, string>()));

        var match2 = matcher.Match("/employers/6de5ce43-4f82-4990-b5be-882fde5abe5d");

        match2.SubscriptionDataSource.Should().Be(dataSources[1]);
        match2.Parameters["employerId"].Should().Be("6de5ce43-4f82-4990-b5be-882fde5abe5d");

        var match3 = matcher.Match("/employers/6de5ce43-4f82-4990-b5be-882fde5abe5d/employees");

        match3.SubscriptionDataSource.Should().Be(dataSources[2]);
        match3.Parameters["employerId"].Should().Be("6de5ce43-4f82-4990-b5be-882fde5abe5d");

        var match4 = matcher.Match("/employers/6de5ce43-4f82-4990-b5be-882fde5abe5d/employees/47e5f868-83fa-4222-ac76-891200025ee7");

        match4.SubscriptionDataSource.Should().Be(dataSources[3]);
        match4.Parameters["employerId"].Should().Be("6de5ce43-4f82-4990-b5be-882fde5abe5d");
        match4.Parameters["employeeId"].Should().Be("47e5f868-83fa-4222-ac76-891200025ee7");
    }

    [Fact]
    public void TestInvalidMatches()
    {
        var entityProviders = TestDataSource.MakeDataSources((text) => { });
        var tree = MakeSubscriptionTree(entityProviders);

        var matcher = new SubscriptionMatcher(tree);

        Action act = () => matcher.Match("");

        act.Should().Throw<SubscriptionException>()
            .WithMessage("Subscription path cannot be null or empty (Parameter 'path')");

        act = () => matcher.Match("/employer/");

        act.Should().Throw<SubscriptionException>()
            .WithMessage("Unable to match path against available subscriptions");

        act = () => matcher.Match("/employers//employees");

        act.Should().Throw<SubscriptionException>()
            .WithMessage("Subscription path cannot contain empty segments (Parameter 'path')");

        act = () => matcher.Match("/employer/e2a3482f-35d7-426a-952e-d95d92e85a60/hampster");

        act.Should().Throw<SubscriptionException>()
            .WithMessage("Unable to match path against available subscriptions");

        act = () => matcher.Match("/employer/e2a3482f-35d7-426a-952e-d95d92e85a60/employees/a540a771-052d-4939-898f-bbb09f35417a/blah");

        act.Should().Throw<SubscriptionException>()
            .WithMessage("Unable to match path against available subscriptions");
    }

    private static SubscriptionTree MakeSubscriptionTree(TestDataSource[] entityProviders)
    {

        var tree = new SubscriptionTree();

        tree.RegisterSubscriptionPath("/employers", entityProviders[0]);
        tree.RegisterSubscriptionPath("/employers/{employerId}", entityProviders[1]);
        tree.RegisterSubscriptionPath("/employers/{employerId}/employees", entityProviders[2]);
        tree.RegisterSubscriptionPath("/employers/{employerId}/employees/{employeeId}", entityProviders[3]);

        return tree;
    }
}
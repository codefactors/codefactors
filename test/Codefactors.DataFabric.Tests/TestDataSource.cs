// Copyright (c) 2024, Codefactors Ltd.
//
// Codefactors Ltd licenses this file to you under the following license(s):
//
//   * The MIT License, see https://opensource.org/license/mit/

using Codefactors.DataFabric.Subscriptions;

namespace Codefactors.DataFabric.Tests;

internal class TestDataSource : SubscriptionDataSource
{
    public TestDataSource(object instance, string methodName)
        : base(instance, methodName)
    {
    }

    public static TestDataSource[] MakeDataSources(Action<string> action)
    {
        var entityObject = new TestEntityObject(action);

        return
        [
            new TestDataSource(entityObject, "GetEmployers"),
            new TestDataSource(entityObject, "GetEmployer"),
            new TestDataSource(entityObject, "GetEmployees"),
            new TestDataSource(entityObject, "GetEmployee"),
        ];
    }
}
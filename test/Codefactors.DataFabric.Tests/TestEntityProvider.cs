// Copyright (c) 2024, Codefactors Ltd.
//
// Codefactors Ltd licenses this file to you under the following license(s):
//
//   * The MIT License, see https://opensource.org/license/mit/

namespace Codefactors.DataFabric.Tests;

internal class TestEntityProvider : EntityProvider
{
    public TestEntityProvider(object instance, string methodName)
        : base(instance, methodName)
    {
    }

    public static TestEntityProvider[] MakeEntityProviders(Action<string> action)
    {
        var entityObject = new EntityObject(action);

        return
        [
            new TestEntityProvider(entityObject, "GetEmployers"),
            new TestEntityProvider(entityObject, "GetEmployer"),
            new TestEntityProvider(entityObject, "GetEmployees"),
            new TestEntityProvider(entityObject, "GetEmployee"),
        ];
    }
}
// Copyright (c) 2024, Codefactors Ltd.
//
// Codefactors Ltd licenses this file to you under the following license(s):
//
//   * The MIT License, see https://opensource.org/license/mit/

using System.Reflection;

namespace Codefactors.DataFabric.Subscriptions;

public interface IEntityProvider
{
    object Instance { get; }

    MethodInfo MethodInfo { get; }

    Task<object> InvokeAsync(params object[] parameters);
}

// Copyright (c) 2024, Codefactors Ltd.
//
// Codefactors Ltd licenses this file to you under the following license(s):
//
//   * The MIT License, see https://opensource.org/license/mit/

namespace Codefactors.DataFabric.Transport;

public interface IDataFabricTransport
{
    Task SendAsync(string subscriptionKey, string subscriptionPath, object update);
}
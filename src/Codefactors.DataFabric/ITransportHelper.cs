// Copyright (c) 2024, Codefactors Ltd.
//
// Codefactors Ltd licenses this file to you under the following license(s):
//
//   * The MIT License, see https://opensource.org/license/mit/

using Microsoft.Extensions.DependencyInjection;

namespace Codefactors.DataFabric.Transport;

public interface ITransportHelper
{
    void Initialise(IServiceCollection services);

    void InitialiseMiddleware(object app);

    void Start(object app);
}
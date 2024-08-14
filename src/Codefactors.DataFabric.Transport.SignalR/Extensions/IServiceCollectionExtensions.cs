// Copyright (c) 2024, Codefactors Ltd.
//
// Codefactors Ltd licenses this file to you under the following license(s):
//
//   * The MIT License, see https://opensource.org/license/mit/

using Codefactors.DataFabric.Subscriptions;
using Microsoft.AspNetCore.SignalR;

namespace Codefactors.DataFabric.Transport.SignalR.Extensions;

/// <summary>
/// Extension methods for <see cref="IServiceCollection"/>.
/// </summary>
public static class IServiceCollectionExtensions
{
    /// <summary>
    /// Adds the SignalR transport as the data fabric notification transport.
    /// </summary>
    /// <param name="services">This <see cref="IServiceCollection"/>.</param>
    /// <param name="initialiseSignalR">Optional parameter; set to false if AddSignalR() is being called elsewhere.</param>
    /// <returns><see cref="IServiceCollection"/> supplied at invocation.</returns>
    public static IServiceCollection AddSignalRTransport(this IServiceCollection services, bool initialiseSignalR = true)
    {
        // Required to ensure SignalR uses the subscription key rather than the regular user id
        services.AddSingleton<IUserIdProvider, SignalRUserIdProvider>();

        if (initialiseSignalR)
            services.AddSignalR();

        services.AddSingleton<IDataFabricTransport, SignalRTransport>();
        services.AddSingleton<ISubscriptionFactory, SignalRSubscriptionFactory>();

        return services;
    }
}
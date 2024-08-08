// Copyright (c) 2024, Codefactors Ltd.
//
// Codefactors Ltd licenses this file to you under the following license(s):
//
//   * The MIT License, see https://opensource.org/license/mit/

namespace Codefactors.DataFabric.Transport.SignalR.Extensions;

/// <summary>
/// Static class containing extension methods for <see cref="IApplicationBuilder"/>.
/// </summary>
public static class IApplicationBuilderExtensions
{
    /// <summary>
    /// Adds the <see cref="SignalRAuthenticationMiddleware"/> to the pipeline.
    /// </summary>
    /// <param name="app"><see cref="IApplicationBuilder"/> instance.</param>
    /// <returns><see cref="IApplicationBuilder"/>.</returns>
    public static IApplicationBuilder UseSignalRAuthentication(this IApplicationBuilder app) =>
        app.UseMiddleware<SignalRAuthenticationMiddleware>();
}
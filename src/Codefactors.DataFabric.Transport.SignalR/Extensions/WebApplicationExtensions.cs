// Copyright (c) 2024, Codefactors Ltd.
//
// Codefactors Ltd licenses this file to you under the following license(s):
//
//   * The MIT License, see https://opensource.org/license/mit/

namespace Codefactors.DataFabric.Transport.SignalR.Extensions;

/// <summary>
/// Extension methods for <see cref="WebApplication"/>.
/// </summary>
public static class WebApplicationExtensions
{
    /// <summary>
    /// Installs the SignalR middleware that allows WebSocket connections to correctly handle
    /// authentication (as WebSockets don't support HTTP headers).
    /// </summary>
    /// <param name="app">This <see cref="WebApplication"/> instance.</param>
    /// <param name="hubPath">SignalR hub path (URL).</param>
    /// <returns>Original <see cref="WebApplication"/> instance.</returns>
    public static WebApplication UseSignalRAuthenticationMiddleware(this WebApplication app, string hubPath)
    {
        app.UseMiddleware<SignalRAuthenticationMiddleware>(hubPath);

        return app;
    }
}
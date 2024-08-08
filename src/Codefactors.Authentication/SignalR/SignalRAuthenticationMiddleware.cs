// Copyright (c) 2024, Codefactors Ltd.
//
// Codefactors Ltd licenses this file to you under the following license(s):
//
//   * The MIT License, see https://opensource.org/license/mit/

namespace Codefactors.DataFabric.Transport.SignalR;

/// <summary>
/// Middleware to add the access token from query param to the header before authentication middleware runs.  This is needed
/// as web sockets cannot pass headers.
/// </summary>
/// <param name="next">Next <see cref="RequestDelegate"/> in the pipeline to invoke.</param>
/// <param name="hubPath">Path to the SignalR hub URL path.</param>
public class SignalRAuthenticationMiddleware(RequestDelegate next, string hubPath)
{
    private readonly RequestDelegate _next = next;

    /// <summary>
    /// Copies the access token from query param to the header before authentication middleware runs.
    /// </summary>
    /// <param name="httpContext"><see cref="HttpContext"/>.</param>
    /// <returns>Result of invoking next entry in middleware pipline.</returns>
    public async Task Invoke(HttpContext httpContext)
    {
        var request = httpContext.Request;

        // web sockets cannot pass headers so we must take the access token from query param and
        // add it to the header before authentication middleware runs
        if (request.Path.StartsWithSegments(hubPath, StringComparison.OrdinalIgnoreCase) &&
            request.Query.TryGetValue("access_token", out var accessToken))
        {
            request.Headers.TryAdd("Authorization", $"Bearer {accessToken}");
        }

        await _next(httpContext);
    }
}
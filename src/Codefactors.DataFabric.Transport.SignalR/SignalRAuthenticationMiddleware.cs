// Copyright (c) 2024, Codefactors Ltd.
//
// Codefactors Ltd licenses this file to you under the following license(s):
//
//   * The MIT License, see https://opensource.org/license/mit/

using Microsoft.AspNetCore.Http;

namespace Codefactors.DataFabric.Transport.SignalR;

public class SignalRAuthenticationMiddleware(RequestDelegate next, string hubPath)
{
    private readonly RequestDelegate _next = next;

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
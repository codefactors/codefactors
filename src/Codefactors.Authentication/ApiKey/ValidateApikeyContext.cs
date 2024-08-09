// Copyright (c) 2024, Codefactors Ltd.
//
// Codefactors Ltd licenses this file to you under the following license(s):
//
//   * The MIT License, see https://opensource.org/license/mit/

using Microsoft.AspNetCore.Authentication;

namespace Codefactors.Authentication.ApiKey;

/// <summary>
/// Context used when validating API keys.
/// </summary>
/// <param name="context">Context.</param>
/// <param name="scheme">Scheme.</param>
/// <param name="options">Options.</param>
/// <param name="apiKey">Credentials.</param>
public class ValidateApikeyContext(
    HttpContext context,
    AuthenticationScheme scheme,
    ApikeyAuthenticationSchemeOptions options,
    string apiKey) : ResultContext<ApikeyAuthenticationSchemeOptions>(context, scheme, options)
{
    /// <summary>
    /// Gets the credentials.
    /// </summary>
    public string ApiKey { get; } = apiKey;
}
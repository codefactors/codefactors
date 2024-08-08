// Copyright (c) 2024, Codefactors Ltd.
//
// Codefactors Ltd licenses this file to you under the following license(s):
//
//   * The MIT License, see https://opensource.org/license/mit/

using Microsoft.AspNetCore.Authentication;

namespace Codefactors.Authentication.Basic;

/// <summary>
/// Context used when validating credentials.
/// </summary>
/// <param name="context">Context.</param>
/// <param name="scheme">Scheme.</param>
/// <param name="options">Options.</param>
/// <param name="credentials">Credentials.</param>
public class ValidateCredentialsContext(
    HttpContext context,
    AuthenticationScheme scheme,
    BasicAuthenticationSchemeOptions options,
    string[] credentials) : ResultContext<BasicAuthenticationSchemeOptions>(context, scheme, options)
{
    /// <summary>
    /// Gets the credentials.
    /// </summary>
    public string[] Credentials { get; } = credentials;
}
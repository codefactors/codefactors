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
/// <param name="context">HTTP context.</param>
/// <param name="scheme">Authentication scheme.</param>
/// <param name="options">Basic authentication options.</param>
/// <param name="credentials">Credentials.</param>
public class ValidateCredentialsContext(
    HttpContext context,
    AuthenticationScheme scheme,
    BasicAuthenticationSchemeOptions options,
    BasicAuthenticationCredentials credentials)
    : ResultContext<BasicAuthenticationSchemeOptions>(context, scheme, options)
{
    /// <summary>
    /// Gets the credentials.
    /// </summary>
    public BasicAuthenticationCredentials Credentials { get; } = credentials;

    /// <summary>
    /// Gets or sets any validation data generated from the validation process. Optional.
    /// </summary>
    public object? ValidationData { get; set; }
}
// Copyright (c) 2024, Codefactors Ltd.
//
// Codefactors Ltd licenses this file to you under the following license(s):
//
//   * The MIT License, see https://opensource.org/license/mit/

using Codefactors.Authentication.Basic;
using Microsoft.AspNetCore.Authentication;

namespace Codefactors.Authentication.Extensions;

/// <summary>
/// Extension methods for <see cref="AuthenticationBuilder"/>.
/// </summary>
public static class AuthenticationBuilderExtensions
{
    /// <summary>
    /// Adds the basic authentication scheme.
    /// </summary>
    /// <param name="builder">Builder to add scheme to.</param>
    /// <param name="configureOptions">Method to configure basic authentication options.</param>
    /// <returns>Builder.</returns>
    public static AuthenticationBuilder AddBasicScheme(this AuthenticationBuilder builder, Action<BasicAuthenticationSchemeOptions> configureOptions)
    {
        builder.AddScheme<BasicAuthenticationSchemeOptions, BasicAuthenticationSchemeHandler>(
            BasicAuthenticationSchemeDefaults.AuthenticationScheme,
            "Basic Authentication",
            configureOptions);

        return builder;
    }

    /// <summary>
    /// Adds the basic authentication scheme without options.  Depends on the Options pattern
    /// for configuration.
    /// </summary>
    /// <param name="builder">Builder to add scheme to.</param>
    /// <returns>Builder.</returns>
    public static AuthenticationBuilder AddBasicScheme(this AuthenticationBuilder builder)
    {
        builder.AddScheme<BasicAuthenticationSchemeOptions, BasicAuthenticationSchemeHandler>(
            BasicAuthenticationSchemeDefaults.AuthenticationScheme,
            "Basic Authentication",
            null);

        return builder;
    }
}

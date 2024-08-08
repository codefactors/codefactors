// Copyright (c) 2024, Codefactors Ltd.
//
// Codefactors Ltd licenses this file to you under the following license(s):
//
//   * The MIT License, see https://opensource.org/license/mit/

using Microsoft.AspNetCore.Authentication;

namespace Codefactors.Authentication.Basic;

/// <summary>
/// Basic authentication scheme options.
/// </summary>
public class BasicAuthenticationSchemeOptions : AuthenticationSchemeOptions
{
    /// <summary>Gets or sets the Events.</summary>
    public new BasicAuthenticationSchemeEvents Events
    {
        get => (BasicAuthenticationSchemeEvents)base.Events!;
        set => base.Events = value;
    }
}

// Copyright (c) 2024, Codefactors Ltd.
//
// Codefactors Ltd licenses this file to you under the following license(s):
//
//   * The MIT License, see https://opensource.org/license/mit/

using Microsoft.AspNetCore.Authentication;

namespace Codefactors.Authentication.ApiKey;

/// <summary>
/// Apikey authentication scheme options.
/// </summary>
public class ApikeyAuthenticationSchemeOptions : AuthenticationSchemeOptions
{
    /// <summary>Gets or sets the Events.</summary>
    public new ApikeyAuthenticationSchemeEvents Events
    {
        get => (ApikeyAuthenticationSchemeEvents)base.Events!;
        set => base.Events = value;
    }
}

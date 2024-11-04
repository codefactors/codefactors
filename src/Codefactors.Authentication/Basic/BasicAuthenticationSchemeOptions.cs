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

    /// <summary>
    /// Gets or sets the validator for validating basic authentication credentials.
    /// </summary>
    public Func<ValidateCredentialsContext, Task<IValidationResult>> ValidateCredentials { get; set; } = default!;

    /// <summary>
    /// Initializes a new instance of the <see cref="BasicAuthenticationSchemeOptions"/> class.
    /// </summary>
    public BasicAuthenticationSchemeOptions()
    {
        Events = new BasicAuthenticationSchemeEvents();
    }
}

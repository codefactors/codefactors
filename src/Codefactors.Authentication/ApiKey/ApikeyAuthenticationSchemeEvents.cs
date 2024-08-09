// Copyright (c) 2024, Codefactors Ltd.
//
// Codefactors Ltd licenses this file to you under the following license(s):
//
//   * The MIT License, see https://opensource.org/license/mit/

namespace Codefactors.Authentication.ApiKey;

/// <summary>
/// Basic authentication scheme events.
/// </summary>
public class ApikeyAuthenticationSchemeEvents
{
    /// <summary>
    /// Gets or sets the delegate that is called when validating credentials.
    /// </summary>
    public Func<ValidateApikeyContext, Task> OnValidateCredentials { get; set; } = default!;

    /// <summary>
    /// Validates the credentials.
    /// </summary>
    /// <param name="context">Validation context.</param>
    /// <returns><see cref="Task"/>.</returns>
    public virtual Task ValidateCredentials(ValidateApikeyContext context) => OnValidateCredentials(context);
}

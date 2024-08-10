// Copyright (c) 2024, Codefactors Ltd.
//
// Codefactors Ltd licenses this file to you under the following license(s):
//
//   * The MIT License, see https://opensource.org/license/mit/

namespace Codefactors.Authentication.Basic;

/// <summary>
/// Basic authentication scheme events.
/// </summary>
public class BasicAuthenticationSchemeEvents
{
    /// <summary>
    /// Validates the credentials.
    /// </summary>
    /// <param name="context">Validation context.</param>
    /// <returns><see cref="Task"/>.</returns>
    /// <exception cref="InvalidOperationException">Thrown if the credentials validator has not been set.</exception>
    public virtual async Task ValidateCredentialsAsync(ValidateCredentialsContext context)
    {
        if (context.Options.CredentialsValidator == null)
            throw new InvalidOperationException("CredentialsValidator property on BasicAuthenticationSchemeOptions must be set");

        var validationResult = await context.Options.CredentialsValidator.ValidateAsync(context.Credentials);

        if (context.Principal == null)
            throw new InvalidOperationException("Credentials were successfully validated but Principal claim was not set");

        if (validationResult.IsValid)
            context.Success();
        else
            context.Fail(validationResult.ValidationData?.ToString() ?? "Invalid credentials");
    }
}

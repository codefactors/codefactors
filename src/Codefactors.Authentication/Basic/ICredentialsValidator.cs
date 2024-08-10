// Copyright (c) 2024, Codefactors Ltd.
//
// Codefactors Ltd licenses this file to you under the following license(s):
//
//   * The MIT License, see https://opensource.org/license/mit/

namespace Codefactors.Authentication.Basic;

/// <summary>
/// Inaterface for types that validate basic authentication credentials.
/// </summary>
public interface ICredentialsValidator
{
    /// <summary>
    /// Validates the supplied credentials.
    /// </summary>
    /// <param name="credentials">Basic authentication credentials to be validated.</param>
    /// <returns><see cref="IValidationResult"/> with IsValid set to true if the credentials are valid.</returns>
    Task<IValidationResult> ValidateAsync(BasicAuthenticationCredentials credentials);
}
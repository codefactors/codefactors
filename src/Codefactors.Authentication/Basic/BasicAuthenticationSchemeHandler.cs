// Copyright (c) 2024, Codefactors Ltd.
//
// Codefactors Ltd licenses this file to you under the following license(s):
//
//   * The MIT License, see https://opensource.org/license/mit/

using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;
using System.Text.Encodings.Web;

namespace Codefactors.Authentication.Basic;

/// <summary>
/// Basic authentication scheme handler.
/// </summary>
public class BasicAuthenticationSchemeHandler : AuthenticationHandler<BasicAuthenticationSchemeOptions>
{
    private const string _emptyCredentialsText = "Failed to authenticate with basic authentication; reason: header had no credentials";

    /// <summary>
    /// Gets or sets the <see cref="BasicAuthenticationSchemeEvents"/>.
    /// </summary>
    /// <remarks>
    /// The handler calls methods on the events which give the application control at certain points where processing is occurring.
    /// If it is not provided a default instance is supplied which does nothing when the methods are called.
    /// </remarks>
    public new BasicAuthenticationSchemeEvents Events
    {
        get => (BasicAuthenticationSchemeEvents)base.Events!;
        set => base.Events = value;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="BasicAuthenticationSchemeHandler"/> class.
    /// </summary>
    /// <param name="options">The monitor for the options instance.</param>
    /// <param name="loggerFactory">The <see cref="ILoggerFactory"/>.</param>
    /// <param name="encoder">The <see cref="UrlEncoder"/>.</param>
    public BasicAuthenticationSchemeHandler(
        IOptionsMonitor<BasicAuthenticationSchemeOptions> options,
        ILoggerFactory loggerFactory,
        UrlEncoder encoder)
        : base(options, loggerFactory, encoder)
    {
    }

    /// <summary>
    /// Method that handles authentication.
    /// </summary>
    /// <returns>The <see cref="AuthenticateResult"/>.</returns>
    /// <exception cref="InvalidOperationException">Thrown if the <see cref="ICredentialsValidator"/> fails
    /// to set the Principal claim.</exception>
    protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        var authorizationHeader = Request.Headers.Authorization.ToString();

        if (string.IsNullOrEmpty(authorizationHeader) ||
            !authorizationHeader.StartsWith(AuthorizationSchemes.Basic, StringComparison.OrdinalIgnoreCase))
            return AuthenticateResult.NoResult();

        if (authorizationHeader.TrimEnd() == AuthorizationSchemes.Basic.TrimEnd())
        {
            Logger.LogWarning(_emptyCredentialsText);

            return AuthenticateResult.Fail(_emptyCredentialsText);
        }

        try
        {
            var credentials = new BasicAuthenticationCredentials(authorizationHeader.Substring(AuthorizationSchemes.Basic.Length).Trim());

            var validateCredentialsContext = new ValidateCredentialsContext(Context, Scheme, Options, credentials);

            await Events.ValidateCredentialsAsync(validateCredentialsContext);

            if (validateCredentialsContext.Result != null)
            {
                if (validateCredentialsContext.Result.Succeeded)
                    return AuthenticateResult.Success(new AuthenticationTicket(validateCredentialsContext.Principal!, Scheme.Name));

                if (validateCredentialsContext.Result.Failure != null)
                    return AuthenticateResult.Fail(validateCredentialsContext.Result.Failure);
            }

            return AuthenticateResult.NoResult();
        }
        catch (ArgumentException ex)
        {
            Logger.LogWarning(ex, "Failed to authenticate with basic authentication; reason: {message}", ex.Message);

            return AuthenticateResult.Fail(ex.Message);
        }
        catch (InvalidOperationException ex)
        {
            Logger.LogError(ex, "Invalid operation when attempting basic authentication; details: {message}", ex.Message);

            return AuthenticateResult.Fail(ex.Message);
        }
    }
}
// Copyright (c) 2024, Codefactors Ltd.
//
// Codefactors Ltd licenses this file to you under the following license(s):
//
//   * The MIT License, see https://opensource.org/license/mit/

using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;
using System.Text;
using System.Text.Encodings.Web;

namespace Codefactors.Authentication.Basic;

/// <summary>
/// Basic authentication scheme handler.
/// </summary>
public class BasicAuthenticationSchemeHandler : AuthenticationHandler<BasicAuthenticationSchemeOptions>
{
    private const string _Scheme = "basic";
    private const string _BearerScheme = "bearer ";
    private const string _DigestScheme = "digest ";
    private const string _ApikeyScheme = "apikey ";

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
    /// <param name="logger">The <see cref="ILoggerFactory"/>.</param>
    /// <param name="encoder">The <see cref="System.Text.Encodings.Web.UrlEncoder"/>.</param>
    public BasicAuthenticationSchemeHandler(
        IOptionsMonitor<BasicAuthenticationSchemeOptions> options,
        ILoggerFactory logger,
        UrlEncoder encoder)
        : base(options, logger, encoder)
    {
    }

    /// <summary>
    /// Method that handles authentication.
    /// </summary>
    /// <returns>The <see cref="AuthenticateResult"/>.</returns>
    /// <exception cref="Exception">Thrown.</exception>
    /// <exception cref="NotImplementedException">Thrown if.</exception>
    protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        var authorizationHeader = Request.Headers.Authorization.ToString();

        if (string.IsNullOrEmpty(authorizationHeader) ||
            !authorizationHeader.StartsWith(AuthorizationSchemes.Basic, StringComparison.OrdinalIgnoreCase))
            return AuthenticateResult.NoResult();

        if (authorizationHeader == _Scheme.Trim())
        {
            const string noCredentialsMessage = "Authorization scheme was Basic but the header had no credentials.";
            Logger.LogInformation(noCredentialsMessage);
            return AuthenticateResult.Fail(noCredentialsMessage);
        }

        string encodedCredentials = authorizationHeader.Substring(_Scheme.Length).Trim();

        // string decodedCredentials = string.Empty;
        byte[] base64DecodedCredentials;
        try
        {
            base64DecodedCredentials = Convert.FromBase64String(encodedCredentials);
        }
        catch (FormatException)
        {
            const string failedToDecodeCredentials = "Cannot convert credentials from Base64.";
            Logger.LogInformation(failedToDecodeCredentials);
            return AuthenticateResult.Fail(failedToDecodeCredentials);
        }

        var utf8ValidatingEncoding = new UTF8Encoding(false, true);
        var decodedCredentials = utf8ValidatingEncoding.GetString(base64DecodedCredentials);

        var parts = decodedCredentials.Split(':');

        if (parts.Length != 2)
        {
            throw new Exception("Bad");
        }

        var validateCredentialsContext = new ValidateCredentialsContext(Context, Scheme, Options, parts);

        await Events.ValidateCredentials(validateCredentialsContext);

        if (validateCredentialsContext.Result != null &&
            validateCredentialsContext.Result.Succeeded)
        {
            var ticket = new AuthenticationTicket(validateCredentialsContext.Principal ?? throw new Exception(), Scheme.Name);
            return AuthenticateResult.Success(ticket);
        }

        if (validateCredentialsContext.Result != null &&
            validateCredentialsContext.Result.Failure != null)
        {
            return AuthenticateResult.Fail(validateCredentialsContext.Result.Failure);
        }

        return AuthenticateResult.NoResult();
        throw new NotImplementedException();
    }
}
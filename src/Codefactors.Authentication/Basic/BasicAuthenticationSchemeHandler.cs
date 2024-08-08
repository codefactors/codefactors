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

public class BasicAuthenticationSchemeHandler : AuthenticationHandler<BasicAuthenticationSchemeOptions>
{
    private const string _Scheme = "Basic";
    public new BasicAuthenticationSchemeEvents Events
    {
        get => (BasicAuthenticationSchemeEvents)base.Events!;
        set => base.Events = value;
    }

    public BasicAuthenticationSchemeHandler(
        IOptionsMonitor<BasicAuthenticationSchemeOptions> options,
        ILoggerFactory logger,
        UrlEncoder encoder)
        : base(options, logger, encoder)
    {
    }

    protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        string authorizationHeader = Request.Headers["Authorization"];
        if (string.IsNullOrEmpty(authorizationHeader))
        {
            return AuthenticateResult.NoResult();
        }
        if (_Scheme == authorizationHeader)
        {
            const string noCredentialsMessage = "Authorization scheme was Basic but the header had no credentials.";
            Logger.LogInformation(noCredentialsMessage);
            return AuthenticateResult.Fail(noCredentialsMessage);
        }
        string encodedCredentials = authorizationHeader.Substring(_Scheme.Length).Trim();
        //string decodedCredentials = string.Empty;
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
            var ticket = new AuthenticationTicket(validateCredentialsContext.Principal, Scheme.Name);
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
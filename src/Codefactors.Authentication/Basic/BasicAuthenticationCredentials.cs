// Copyright (c) 2024, Codefactors Ltd.
//
// Codefactors Ltd licenses this file to you under the following license(s):
//
//   * The MIT License, see https://opensource.org/license/mit/

using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using System.Text;

namespace Codefactors.Authentication.Basic;

/// <summary>
/// Basic authentication credentials.
/// </summary>
public class BasicAuthenticationCredentials
{
    private static readonly UTF8Encoding _utf8ValidatingEncoding = new UTF8Encoding(false, true);

    /// <summary>
    /// Gets the username.
    /// </summary>
    public string Username { get; init; }

    /// <summary>
    /// Gets the password.
    /// </summary>
    public string Password { get; init; }

    /// <summary>
    /// Initialises a new instance of <see cref="BasicAuthenticationCredentials"/>.
    /// </summary>
    /// <param name="base64Credentials">Base-64 encoded credentials string.</param>
    /// <exception cref="ArgumentException">Thrown if the supplied string cannot be base-64 decoded,
    /// or if the decoded value is empty.</exception>
    public BasicAuthenticationCredentials(string base64Credentials)
    {
        byte[] base64DecodedCredentials;

        try
        {
            base64DecodedCredentials = Convert.FromBase64String(base64Credentials);
        }
        catch (FormatException ex)
        {
            throw new ArgumentException("Unable to decode credentials; not a valid base-64 encoded value", nameof(base64Credentials), ex);
        }

        var decodedCredentials = _utf8ValidatingEncoding.GetString(base64DecodedCredentials);

        var parts = decodedCredentials.Split(':', 2, StringSplitOptions.RemoveEmptyEntries);

        if (parts.Length < 1 || parts.Length > 2)
            throw new ArgumentException("Unable to decode any values from the supplied credentials string.", nameof(base64Credentials));

        Username = parts[0];
        Password = parts.Length == 2 ? parts[1] : string.Empty;
    }
}
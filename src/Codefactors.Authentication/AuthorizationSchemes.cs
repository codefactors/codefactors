// Copyright (c) 2024, Codefactors Ltd.
//
// Codefactors Ltd licenses this file to you under the following license(s):
//
//   * The MIT License, see https://opensource.org/license/mit/

namespace Codefactors.Authentication;

/// <summary>
/// Static class providing access to the auth-scheme part of the HTTP Authorization header.
/// </summary>
/// <remarks>Note that scheme names here are all lower case in line with RFC 7235 Section 2.1, which
/// states that the auth-scheme part of an HTTP Authorization header is case-insensitive.</remarks>
public static class AuthorizationSchemes
{
    /// <summary>Basic authentication.</summary>
    public const string Basic = "basic ";

    /// <summary>Bearer authentication.</summary>
    public const string Bearer = "bearer ";

    /// <summary>Digest authentication.</summary>
    public const string DigestScheme = "digest ";

    /// <summary>Apikey authentication.</summary>
    public const string ApikeyScheme = "apikey ";


}

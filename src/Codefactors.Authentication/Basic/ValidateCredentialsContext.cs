// Copyright (c) 2024, Codefactors Ltd.
//
// Codefactors Ltd licenses this file to you under the following license(s):
//
//   * The MIT License, see https://opensource.org/license/mit/

using Microsoft.AspNetCore.Authentication;

namespace Codefactors.Authentication.Basic;

public class ValidateCredentialsContext(
    HttpContext context,
    AuthenticationScheme scheme,
    BasicAuthenticationSchemeOptions options,
    string[] credentials) : ResultContext<BasicAuthenticationSchemeOptions>(context, scheme, options)
{
    public string[] Credentials { get; } = credentials;
}
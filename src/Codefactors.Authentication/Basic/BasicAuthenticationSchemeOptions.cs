// Copyright (c) 2024, Codefactors Ltd.
//
// Codefactors Ltd licenses this file to you under the following license(s):
//
//   * The MIT License, see https://opensource.org/license/mit/

using Microsoft.AspNetCore.Authentication;

namespace Codefactors.Authentication.Basic;

public class BasicAuthenticationSchemeOptions : AuthenticationSchemeOptions
{
    public new BasicAuthenticationSchemeEvents Events
    {
        get => (BasicAuthenticationSchemeEvents)base.Events!;
        set => base.Events = value;
    }
}

// Copyright (c) 2024, Codefactors Ltd.
//
// Codefactors Ltd licenses this file to you under the following license(s):
//
//   * The MIT License, see https://opensource.org/license/mit/

namespace Codefactors.Authentication.Basic;

public class BasicAuthenticationSchemeEvents
{
    public Func<ValidateCredentialsContext, Task> OnValidateCredentials { get; set; } = default!;

    public virtual Task ValidateCredentials(ValidateCredentialsContext context) => OnValidateCredentials(context);
}

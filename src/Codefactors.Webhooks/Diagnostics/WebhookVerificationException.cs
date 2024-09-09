// Copyright (c) 2024, Codefactors Ltd.
//
// Codefactors Ltd licenses this file to you under the following license(s):
//
//   * The MIT License, see https://opensource.org/license/mit/

// Portions Copyright (c) 2023 Svix (https://www.svix.com) used under MIT licence,
// see https://github.com/standard-webhooks/standard-webhooks/blob/main/libraries/LICENSE.

namespace Codefactors.Webhooks.Diagnostics;

public class WebhookVerificationException : Exception
{
    public WebhookVerificationException() : base() { }
    public WebhookVerificationException(string message) : base(message) { }
    public WebhookVerificationException(string message, Exception inner) : base(message, inner) { }
}
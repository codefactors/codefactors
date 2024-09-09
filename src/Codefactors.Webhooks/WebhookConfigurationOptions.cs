// Copyright (c) 2024, Codefactors Ltd.
//
// Codefactors Ltd licenses this file to you under the following license(s):
//
//   * The MIT License, see https://opensource.org/license/mit/

namespace Codefactors.Webhooks;

public class WebhookConfigurationOptions
{
    public string IdHeaderKey { get; init; } = default!;

    public string SignatureHeaderKey { get; init; } = default!;

    public string TimestampHeaderKey { get; init; } = default!;
}

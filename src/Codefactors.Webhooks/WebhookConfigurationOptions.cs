// Copyright (c) 2024, Codefactors Ltd.
//
// Codefactors Ltd licenses this file to you under the following license(s):
//
//   * The MIT License, see https://opensource.org/license/mit/

namespace Codefactors.Webhooks;

public class WebhookConfigurationOptions
{
    private static WebhookConfigurationOptions _standard = new WebhookConfigurationOptions
    {
        IdHeaderKey = "webhook-id",
        SignatureHeaderKey = "webhook-signature",
        TimestampHeaderKey = "webhook-timestamp"
    };

    private static WebhookConfigurationOptions _svix = new WebhookConfigurationOptions
    {
        IdHeaderKey = "Svix-Id",
        SignatureHeaderKey = "Svix-Signature",
        TimestampHeaderKey = "Svix-Timestamp"
    };

    public string IdHeaderKey { get; init; } = default!;

    public string SignatureHeaderKey { get; init; } = default!;

    public string TimestampHeaderKey { get; init; } = default!;

    public static WebhookConfigurationOptions StandardWebhooks => _standard;

    public static WebhookConfigurationOptions SvixWebhooks => _svix;
}
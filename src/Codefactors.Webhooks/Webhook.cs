// Copyright (c) 2024, Codefactors Ltd.
//
// Codefactors Ltd licenses this file to you under the following license(s):
//
//   * The MIT License, see https://opensource.org/license/mit/

// Portions Copyright (c) 2023 Svix (https://www.svix.com) used under MIT licence,
// see https://github.com/standard-webhooks/standard-webhooks/blob/main/libraries/LICENSE.

using Codefactors.Webhooks.Diagnostics;
using System.Net;
using System.Security.Cryptography;
using System.Text;

namespace Codefactors.Webhooks;

public sealed class Webhook
{
    private const int TOLERANCE_IN_SECONDS = 60 * 5;

    private static readonly UTF8Encoding SafeUTF8Encoding = new UTF8Encoding(false, true);

    private readonly string _idHeaderKey;
    private readonly string _signatureHeaderKey;
    private readonly string _timestampHeaderKey;

    private static string _prefix = "whsec_";

    private byte[] _key;

    public Webhook(string key, WebhookConfigurationOptions options)
        : this(Convert.FromBase64String(key.StartsWith(_prefix) ? key.Substring(_prefix.Length) : key), options)
    {
    }

    public Webhook(byte[] key, WebhookConfigurationOptions options)
    {
        _key = key;

        _idHeaderKey = options.IdHeaderKey;
        _signatureHeaderKey = options.SignatureHeaderKey;
        _timestampHeaderKey = options.TimestampHeaderKey;
    }

    public void Verify(string payload, WebHeaderCollection headers)
    {
        string msgId = headers.Get(_idHeaderKey) ?? string.Empty;
        string msgSignature = headers.Get(_signatureHeaderKey) ?? string.Empty;
        string msgTimestamp = headers.Get(_timestampHeaderKey) ?? string.Empty;

        if (string.IsNullOrEmpty(msgId) || string.IsNullOrEmpty(msgSignature) || string.IsNullOrEmpty(msgTimestamp))
            throw new WebhookVerificationException("Missing Required Headers");

        var timestamp = Webhook.VerifyTimestamp(msgTimestamp);

        var signature = this.Sign(msgId, timestamp, payload);

        var expectedSignature = signature.Split(',')[1];
        var passedSignatures = msgSignature.Split(' ');

        foreach (string versionedSignature in passedSignatures)
        {
            var parts = versionedSignature.Split(',');

            if (parts.Length < 2)
                throw new WebhookVerificationException("Invalid Signature Headers");

            var version = parts[0];
            var passedSignature = parts[1];

            if (version != "v1")
                continue;

            if (Utils.SecureCompare(expectedSignature, passedSignature))
                return;
        }

        throw new WebhookVerificationException("No matching signature found");
    }

    private static DateTimeOffset VerifyTimestamp(string timestampHeader)
    {
        DateTimeOffset timestamp;

        var now = DateTimeOffset.UtcNow;

        try
        {
            var timestampInt = long.Parse(timestampHeader);

            timestamp = DateTimeOffset.FromUnixTimeSeconds(timestampInt);
        }
        catch
        {
            throw new WebhookVerificationException("Invalid Signature Headers");
        }

        if (timestamp < now.AddSeconds(-1 * TOLERANCE_IN_SECONDS))
            throw new WebhookVerificationException("Message timestamp too old");

        if (timestamp > now.AddSeconds(TOLERANCE_IN_SECONDS))
            throw new WebhookVerificationException("Message timestamp too new");

        return timestamp;
    }

    public string Sign(string msgId, DateTimeOffset timestamp, string payload)
    {
        var toSign = $"{msgId}.{timestamp.ToUnixTimeSeconds().ToString()}.{payload}";
        var toSignBytes = SafeUTF8Encoding.GetBytes(toSign);

        using (var hmac = new HMACSHA256(this._key))
        {
            var hash = hmac.ComputeHash(toSignBytes);

            var signature = Convert.ToBase64String(hash);

            return $"v1,{signature}";
        }
    }
}
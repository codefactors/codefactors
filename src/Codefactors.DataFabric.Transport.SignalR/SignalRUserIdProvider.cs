// Copyright (c) 2024, Codefactors Ltd.
//
// Codefactors Ltd licenses this file to you under the following license(s):
//
//   * The MIT License, see https://opensource.org/license/mit/

using Microsoft.AspNetCore.SignalR;
using System.Security.Claims;

namespace Codefactors.DataFabric.Transport.SignalR;

public class SignalRUserIdProvider(ISubscriptionKeyGenerator subscriptionKeyGenerator) : IUserIdProvider
{
    private readonly ISubscriptionKeyGenerator _subscriptionKeyGenerator = subscriptionKeyGenerator;

    public string GetUserId(HubConnectionContext connection) =>
        _subscriptionKeyGenerator is IClaimsBasedSubscriptionKeyGenerator generator &&
        connection.User?.Claims is IEnumerable<Claim> claims ?
            generator.GenerateKey(claims) :
            string.Empty;
}

// Copyright (c) 2024, Codefactors Ltd.
//
// Codefactors Ltd licenses this file to you under the following license(s):
//
//   * The MIT License, see https://opensource.org/license/mit/

using System.Security.Claims;

namespace Codefactors.DataFabric.Subscriptions;

/// <summary>
/// Interface for generating subscription keys based on claims rather than request context.
/// </summary>
public interface IClaimsBasedSubscriptionKeyGenerator
{
    string GenerateKey(IEnumerable<Claim> claims);
}

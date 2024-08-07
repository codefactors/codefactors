// Copyright (c) 2024, Codefactors Ltd.
//
// Codefactors Ltd licenses this file to you under the following license(s):
//
//   * The MIT License, see https://opensource.org/license/mit/

namespace Codefactors.DataFabric.Subscriptions;

/// <summary>
/// Interface for entities that generate subscription keys.
/// </summary>
public interface ISubscriptionKeyGenerator
{
    /// <summary>
    /// Generates the subscription key for the specified request context.
    /// </summary>
    /// <param name="requestContext">Request context.</param>
    /// <returns>Subscription key.</returns>
    string GenerateKey(IRequestContext requestContext);
}

// Copyright (c) 2024, Codefactors Ltd.
//
// Codefactors Ltd licenses this file to you under the following license(s):
//
//   * The MIT License, see https://opensource.org/license/mit/

using Codefactors.DataFabric.Diagnostics;
using Codefactors.DataFabric.Reflection;
using static Codefactors.DataFabric.Subscriptions.SubscriptionPathUtils;

namespace Codefactors.DataFabric.Subscriptions;

/// <summary>
/// Subscription path matcher for matching paths against available subscriptions.
/// </summary>
/// <param name="subscriptionTree">Subscription tree to search for path matches.</param>
public class SubscriptionMatcher(SubscriptionTree subscriptionTree)
{
    private readonly SubscriptionTree _subscriptionTree = subscriptionTree;

    /// <summary>
    /// Matches a path against the available subscriptions.
    /// </summary>
    /// <param name="path">Subscription path.</param>
    /// <returns>Invocation helper for the specified subcription path.</returns>
    /// <exception cref="SubscriptionException">Thrown if no match is found.</exception>
    public InvocationHelper Match(string path)
    {
        try
        {
            var parameters = new Dictionary<string, string>();

            var segments = Split(path);

            SubscriptionTreeNode? currentNode = _subscriptionTree;

            for (int i = 0; i < segments.Length; i++)
            {
                var isLastSegment = i == segments.Length - 1;

                if (currentNode.TryGetValue(segments[i], out var segmentNode))
                {
                    // Are we at the end of the path?
                    if (isLastSegment)
                        return MakeInvocationData(segmentNode, parameters);

                    // No - move to the next segment
                    currentNode = segmentNode;

                    continue;
                }
                // Otherwise, check for a placeholder
                else if (currentNode.TryGetPlaceholderNode(out segmentNode))
                {
                    var (key, value) = MakeParameter(segmentNode, segments[i]);

                    parameters.Add(key, value);

                    // Are we at the end of the path?
                    if (isLastSegment)
                        return MakeInvocationData(segmentNode, parameters);

                    // No - move to the next segment
                    currentNode = segmentNode;

                    continue;
                }

                // No matches thus far; stop search and throw exception (below)
                break;
            }
        }
        // NB Also catches ArgumentNullException
        catch (ArgumentException ex)
        {
            throw new SubscriptionException(SubscriptionErrorType.InvalidArgument, ex.Message, ex);
        }

        throw new SubscriptionException(SubscriptionErrorType.PathNotFound, "Unable to match path against available subscriptions");
    }

    private static (string key, string value) MakeParameter(
        in SubscriptionTreeNode node,
        in string segment) =>
        (node.Value[1..^1], segment);

    private static InvocationHelper MakeInvocationData(
        in SubscriptionTreeNode node,
        in Dictionary<string, string> parameters) =>
        new InvocationHelper(
            node.DataSource ??
                throw new SubscriptionException(SubscriptionErrorType.Fatal, "Unable to resolve subscription error; internal configuration error"),
            parameters);
}
// Copyright (c) 2024, Codefactors Ltd.
//
// Codefactors Ltd licenses this file to you under the following license(s):
//
//   * The MIT License, see https://opensource.org/license/mit/

using static Codefactors.DataFabric.Subscriptions.SubscriptionPathUtils;

namespace Codefactors.DataFabric.Subscriptions;

/// <summary>
/// Entity that represents a tree of subscriptions keyed on subscription path.
/// </summary>
public class SubscriptionTree : SubscriptionTreeNode
{
    /// <summary>
    /// Initialises a new instance of the SubscriptionTree class, as the root node, with a pseudo-value of '/'.
    /// </summary>
    public SubscriptionTree()
        : base(new string(PathSeparator, 1))
    {
    }

    /// <summary>
    /// Registers a data source against a subscription path.
    /// </summary>
    /// <param name="path">Subscription path.</param>
    /// <param name="dataSource">Data source.</param>
    /// <exception cref="ArgumentNullException">Thrown if the supplied data source is null.</exception>
    /// <exception cref="ArgumentException">Thrown if a subscription path already exists, or if it contains
    /// empty segments.</exception>
    public void RegisterSubscriptionPath(string path, ISubscriptionDataSource dataSource)
    {
        if (dataSource == null)
            throw new ArgumentNullException(nameof(dataSource), "Subscription data source cannot be null");

        if (path.Contains(DoublePathSeparator))
            throw new ArgumentException("Subscription path cannot contain empty segments", nameof(path));

        var segments = Split(path, true);

        // Lowercase all segments except placeholders
        segments = segments.Select(s => IsPlaceholderSegment(s) ? s : s.ToLowerInvariant()).ToArray();

        // See if we already have this in the root
        if (TryGetValue(segments[0], out var segmentNode))
        {
            if (segments.Length == 1)
            {
                if (segmentNode.DataSource != null)
                    throw new ArgumentException("Subscription path already exists", nameof(path));

                segmentNode.Update(dataSource);
            }
            else
            {
                segmentNode.Add(segments.Skip(1), dataSource);
            }
        }
        // Otherwise, add it to the root
        else
        {
            Add(segments, dataSource);
        }
    }
}
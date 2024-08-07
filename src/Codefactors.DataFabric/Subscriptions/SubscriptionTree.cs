// Copyright (c) 2024, Codefactors Ltd.
//
// Codefactors Ltd licenses this file to you under the following license(s):
//
//   * The MIT License, see https://opensource.org/license/mit/

namespace Codefactors.DataFabric.Subscriptions;

public class SubscriptionTree : SubscriptionTreeNode
{
    // Initialises a new instance of the SubscriptionTree class, as the root node, with a pseudo-value of '/'.
    public SubscriptionTree()
        : base(new string(PathSeparator, 1))
    {
    }

    public void RegisterSubscriptionPath(string path, IEntityProvider entityProvider)
    {
        if (entityProvider == null)
            throw new ArgumentNullException(nameof(entityProvider), "Entity provider cannot be null");

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
                if (segmentNode.EntityProvider != null)
                    throw new ArgumentException("Subscription path already exists", nameof(path));

                segmentNode.Update(entityProvider);
            }
            else
            {
                segmentNode.Add(segments.Skip(1), entityProvider);
            }
        }
        // Otherwise, add it to the root
        else
        {
            Add(segments, entityProvider);
        }
    }
}
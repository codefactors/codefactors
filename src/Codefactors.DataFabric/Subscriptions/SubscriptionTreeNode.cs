// Copyright (c) 2024, Codefactors Ltd.
//
// Codefactors Ltd licenses this file to you under the following license(s):
//
//   * The MIT License, see https://opensource.org/license/mit/

using System.Diagnostics.CodeAnalysis;

namespace Codefactors.DataFabric.Subscriptions;

public class SubscriptionTreeNode
{
    private readonly HashSet<SubscriptionTreeNode> _children = [];

    internal HashSet<SubscriptionTreeNode> Children => _children;

    public string Value { get; }

    public bool IsPlaceholder => IsPlaceholderSegment(Value);

    public IEntityProvider? EntityProvider { get; private set; }

    public SubscriptionTreeNode(string value, IEntityProvider? entityProvider = null)
    {
        Value = value;
        EntityProvider = entityProvider;
    }

    public void Update(IEntityProvider entityProvider)
    {
        EntityProvider = entityProvider;
    }

    public bool TryGetValue(string value, [NotNullWhen(true)] out SubscriptionTreeNode? segmentNode) =>
        (segmentNode = _children.FirstOrDefault(c => c.Value == value)) != null;

    public SubscriptionTreeNode? Add(IEnumerable<string> segments, IEntityProvider entityProvider)
    {
        if (!segments.Any())
            throw new ArgumentException("Segments cannot be empty", nameof(segments));

        var firstSegment = segments.First();

        if (IsPlaceholderSegment(firstSegment) &&
            HasPlaceholderNode() &&
            !_children.Any(node => node.Value == firstSegment))
            throw new ArgumentException($"Unexpected placeholder value '{firstSegment}'; placeholders must be consistent for a given path", nameof(segments));

        var nodeFound = TryGetValue(firstSegment, out var segmentNode);

        if (segments.Count() == 1)
        {
            if (nodeFound)
            {
                if (segmentNode!.EntityProvider != null)
                    throw new ArgumentException("Subscription path already exists", nameof(segments));

                segmentNode.Update(entityProvider);
            }
            else
            {
                _children.Add(segmentNode = new SubscriptionTreeNode(segments.First(), entityProvider));
            }
        }
        else
        {
            if (nodeFound)
            {
                segmentNode!.Add(segments.Skip(1), entityProvider);
            }
            else
            {
                _children.Add(segmentNode = new SubscriptionTreeNode(segments.First()));

                segmentNode.Add(segments.Skip(1), entityProvider);
            }
        }

        return segmentNode;
    }

    public bool TryGetPlaceholderNode([NotNullWhen(true)] out SubscriptionTreeNode? subscriptionTreeNode) =>
        (subscriptionTreeNode = _children.FirstOrDefault(c => c.IsPlaceholder)) != null;

    public bool HasPlaceholderNode() => _children.Any(c => c.IsPlaceholder);
}
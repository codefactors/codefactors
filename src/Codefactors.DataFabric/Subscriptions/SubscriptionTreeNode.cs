// Copyright (c) 2024, Codefactors Ltd.
//
// Codefactors Ltd licenses this file to you under the following license(s):
//
//   * The MIT License, see https://opensource.org/license/mit/

using System.Diagnostics.CodeAnalysis;
using static Codefactors.DataFabric.Subscriptions.SubscriptionPathUtils;

namespace Codefactors.DataFabric.Subscriptions;

/// <summary>
/// Represents a node in a subscription tree.
/// </summary>
public class SubscriptionTreeNode
{
    private readonly HashSet<SubscriptionTreeNode> _children = [];

    /// <summary>
    /// Gets the children of the node.
    /// </summary>
    internal HashSet<SubscriptionTreeNode> Children => _children;

    /// <summary>
    /// Gets the value (path segment) of the node.
    /// </summary>
    public string Value { get; }

    /// <summary>
    /// Gets a value indicating whether the node is a placeholder.
    /// </summary>
    public bool IsPlaceholder => IsPlaceholderSegment(Value);

    /// <summary>
    /// Gets the data source for the node.
    /// </summary>
    public ISubscriptionDataSource? DataSource { get; private set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="SubscriptionTreeNode"/> class.
    /// </summary>
    /// <param name="value">Value (path segment) of the node.</param>
    /// <param name="dataSource">Data source for the node. Optional; null indicates this is NOT an end node.</param>
    public SubscriptionTreeNode(string value, ISubscriptionDataSource? dataSource = null)
    {
        Value = value;
        DataSource = dataSource;
    }

    /// <summary>
    /// Updaes the data source for the node.
    /// </summary>
    /// <param name="dataSource">Subscription data source.</param>
    public void Update(ISubscriptionDataSource dataSource)
    {
        DataSource = dataSource;
    }

    /// <summary>
    /// Tries to get the child node that matches the value (path segment) of the node.
    /// </summary>
    /// <param name="value">Target value (path segment).</param>
    /// <param name="segmentNode">Node that matches the value (path segment), or null if no match.</param>
    /// <returns>True if a valid match was found, false otherwise.</returns>
    public bool TryGetValue(string value, [NotNullWhen(true)] out SubscriptionTreeNode? segmentNode) =>
        (segmentNode = _children.FirstOrDefault(c => c.Value == value)) != null;

    /// <summary>
    /// Adds one or more path segments to the node.
    /// </summary>
    /// <param name="segments">Path segments.</param>
    /// <param name="dataSource">Data source that corresponds to the path segment.</param>
    /// <returns>Subscription tree node to which the supplied segments have been added.</returns>
    /// <exception cref="ArgumentException">Thrown if a subscription path already exists, or if an invalid segment
    /// path is supplied.</exception>
    public SubscriptionTreeNode? Add(IEnumerable<string> segments, ISubscriptionDataSource dataSource)
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
                if (segmentNode!.DataSource != null)
                    throw new ArgumentException("Subscription path already exists", nameof(segments));

                segmentNode.Update(dataSource);
            }
            else
            {
                _children.Add(segmentNode = new SubscriptionTreeNode(segments.First(), dataSource));
            }
        }
        else
        {
            if (nodeFound)
            {
                segmentNode!.Add(segments.Skip(1), dataSource);
            }
            else
            {
                _children.Add(segmentNode = new SubscriptionTreeNode(segments.First()));

                segmentNode.Add(segments.Skip(1), dataSource);
            }
        }

        return segmentNode;
    }

    /// <summary>
    /// Tries to get the placeholder node for this current node.
    /// </summary>
    /// <param name="subscriptionTreeNode">Placeholder subscription tree node if the node is a placeholder; null otherwise.</param>
    /// <returns>True if this node is a placeholder node, false otherwise.</returns>
    public bool TryGetPlaceholderNode([NotNullWhen(true)] out SubscriptionTreeNode? subscriptionTreeNode) =>
        (subscriptionTreeNode = _children.FirstOrDefault(c => c.IsPlaceholder)) != null;

    /// <summary>
    /// Gets a value indicating whether the node has a placeholder node.
    /// </summary>
    /// <returns>True if any of the nodes children are placeholders.</returns>
    public bool HasPlaceholderNode() => _children.Any(c => c.IsPlaceholder);
}
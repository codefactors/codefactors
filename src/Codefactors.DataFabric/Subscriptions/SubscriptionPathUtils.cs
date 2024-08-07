// Copyright (c) 2024, Codefactors Ltd.
//
// Codefactors Ltd licenses this file to you under the following license(s):
//
//   * The MIT License, see https://opensource.org/license/mit/

namespace Codefactors.DataFabric.Subscriptions;

/// <summary>
/// Static class that provides utility methods for working with subscription paths.
/// </summary>
public static class SubscriptionPathUtils
{
    /// <summary>Path element separator character.</summary>
    public const char PathSeparator = '/';

    /// <summary>Open placeholder character.</summary>
    public const char OpenPlaceholder = '{';

    /// <summary>Close placeholder character.</summary>
    public const char ClosePlaceholder = '}';

    /// <summary>Invalid double placeholder string.</summary>
    public const string DoublePathSeparator = "//";

    /// <summary>Invalid empty placeholder string.</summary>
    public const string EmptyPlaceholder = "{}";

    /// <summary>
    /// Splits the supplied string into path segments.
    /// </summary>
    /// <param name="path">Subscription path to be split.</param>
    /// <param name="registering">Set to true if the path is being registered; false otherwise.</param>
    /// <returns>Array of path segments, as strings.</returns>
    /// <exception cref="ArgumentNullException">Thrown if the path supplied is empty or null.</exception>
    /// <exception cref="ArgumentException">Thrown if the path starts with a placeholder or if a placeholder
    /// is ill-formed.</exception>
    public static string[] Split(string path, bool registering = false)
    {
        if (string.IsNullOrWhiteSpace(path))
            throw new ArgumentNullException(nameof(path), "Subscription path cannot be null or empty");

        if (path.Contains(DoublePathSeparator))
            throw new ArgumentException("Subscription path cannot contain empty segments", nameof(path));

        var segments = path.Trim(PathSeparator)
            .Split(PathSeparator);

        if (segments.Length == 0)
            throw new ArgumentException("Subscription path cannot be empty", nameof(path));

        if (registering)
        {
            if (IsPlaceholderSegment(segments[0]))
                throw new ArgumentException("Subscription path cannot start with a placeholder", nameof(path));

            if (segments.Any(s => IsIllFormedPlaceholder(s)))
                throw new ArgumentException("Subscription path contains ill-formed placeholders", nameof(path));
        }
        else
        {
            if (segments.Any(s => IsPlaceholderSegment(s)) || IsIllFormedPlaceholder(path))
                throw new ArgumentException("Invalid subscription path; path cannot contain '{' or '}''", nameof(path));
        }

        // Lowercase all segments except placeholders
        segments = segments.Select(s => IsPlaceholderSegment(s) ? s : s.ToLowerInvariant()).ToArray();

        return segments;
    }

    /// <summary>
    /// Gets a value indicating whether the supplied segment is a placeholder.
    /// </summary>
    /// <param name="segment">Path segment.</param>
    /// <returns>True if the path segment is a placeholder, false otherwise.</returns>
    public static bool IsPlaceholderSegment(string segment) =>
        segment.Length > 2 && segment[0] == OpenPlaceholder && segment[^1] == ClosePlaceholder;

    /// <summary>
    /// Tests the supplied segment for ill-formed placeholders.
    /// </summary>
    /// <param name="segment">Path segment.</param>
    /// <returns>True if the segment contains an invalid placeholder, false otherwise.</returns>
    public static bool IsIllFormedPlaceholder(string segment)
    {
        var openPlacoeholderCount = segment.Count(c => c == OpenPlaceholder);
        var closePlaceholderCount = segment.Count(c => c == ClosePlaceholder);

        if (openPlacoeholderCount == 0 && closePlaceholderCount == 0)
            return false;

        if (openPlacoeholderCount > 1 || closePlaceholderCount > 1 || segment.Length == 2)
            return true;

        if (segment[0] != OpenPlaceholder || segment[^1] != ClosePlaceholder)
            return true;

        return false;
    }
}
// Copyright (c) 2024, Codefactors Ltd.
//
// Codefactors Ltd licenses this file to you under the following license(s):
//
//   * The MIT License, see https://opensource.org/license/mit/

namespace Codefactors.DataFabric.Subscriptions;

public static class PathUtils
{
    public const char PathSeparator = '/';
    public const char OpenPlaceholder = '{';
    public const char ClosePlaceholder = '}';
    public const string DoublePathSeparator = "//";
    public const string EmptyPlaceholder = "{}";

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

            if (segments.Any(s => IsIllformedPlaceholder(s)))
                throw new ArgumentException("Subscription path contains ill-formed placeholders", nameof(path));
        }
        else
        {
            if (segments.Any(s => IsPlaceholderSegment(s)) || IsIllformedPlaceholder(path))
                throw new ArgumentException("Invalid subscription path; path cannot contain '{' or '}''", nameof(path));
        }

        // Lowercase all segments except placeholders
        segments = segments.Select(s => IsPlaceholderSegment(s) ? s : s.ToLowerInvariant()).ToArray();

        return segments;
    }

    public static bool IsPlaceholderSegment(string segment) =>
        segment.Length > 2 && segment[0] == OpenPlaceholder && segment[^1] == ClosePlaceholder;

    public static bool IsIllformedPlaceholder(string segment)
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

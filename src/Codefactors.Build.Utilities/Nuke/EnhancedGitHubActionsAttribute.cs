// Copyright (c) 2024, Codefactors Ltd.
//
// Codefactors Ltd licenses this file to you under the following license(s):
//
//   * The MIT License, see https://opensource.org/license/mit/

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

using JetBrains.Annotations;
using Nuke.Common.CI.GitHubActions;
using Nuke.Common.CI.GitHubActions.Configuration;
using Nuke.Common.Execution;
using Nuke.Common.IO;
using Nuke.Common.Tooling;
using Nuke.Common.Utilities;
using Nuke.Common.Utilities.Collections;

namespace Codefactors.Build.Utilities.Nuke;

[PublicAPI]
[AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
public class EnhancedGitHubActionsAttribute(string name, GitHubActionsImage image, params GitHubActionsImage[] images)
    : GitHubActionsAttribute(name, image, images)
{
    private GitHubActionsSubmodules? _submodules;
    private bool? _lfs;
    private uint? _fetchDepth;

    public string[] Services { get; set; } = [];

    public new GitHubActionsSubmodules Submodules
    {
        set => _submodules = value;
        get => throw new NotSupportedException();
    }

    public new bool Lfs
    {
        set => _lfs = value;
        get => throw new NotSupportedException();
    }

    public new uint FetchDepth
    {
        set => _fetchDepth = value;
        get => throw new NotSupportedException();
    }

    protected override GitHubActionsJob GetJobs(GitHubActionsImage image, IReadOnlyCollection<ExecutableTarget> relevantTargets)
    {
        return new EnhancedGitHubActionsJob
        {
            Name = image.GetValue().Replace(".", "_"),
            Steps = GetSteps(image, relevantTargets).ToArray(),
            Services = Services,
            Image = image,
            TimeoutMinutes = TimeoutMinutes,
            ConcurrencyGroup = JobConcurrencyGroup,
            ConcurrencyCancelInProgress = JobConcurrencyCancelInProgress
        };
    }

    private IEnumerable<GitHubActionsStep> GetSteps(GitHubActionsImage _, IReadOnlyCollection<ExecutableTarget> relevantTargets)
    {
        yield return new GitHubActionsCheckoutStep
        {
            Submodules = _submodules,
            Lfs = _lfs,
            FetchDepth = _fetchDepth
        };

        if (CacheKeyFiles.Any())
        {
            yield return new GitHubActionsCacheStep
            {
                IncludePatterns = CacheIncludePatterns,
                ExcludePatterns = CacheExcludePatterns,
                KeyFiles = CacheKeyFiles
            };
        }

        yield return new GitHubActionsRunStep
        {
            BuildCmdPath = BuildCmdPath,
            InvokedTargets = InvokedTargets,
            Imports = GetImports().ToDictionary(x => x.Key, x => x.Value)
        };

        if (PublishArtifacts)
        {
            var artifacts = relevantTargets
                .SelectMany(x => x.ArtifactProducts)
                .Select(x => (AbsolutePath)x)
                // TODO: https://github.com/actions/upload-artifact/issues/11
                .Select(x => x.DescendantsAndSelf(y => y.Parent).FirstOrDefault(y => !y.ToString().ContainsOrdinalIgnoreCase("*")))
                .Distinct().ToList();

            foreach (var artifact in artifacts)
            {
                yield return new GitHubActionsArtifactStep
                {
                    Name = artifact?.ToString().TrimStart(artifact.Parent.ToString()).TrimStart('/', '\\'),
                    Path = Build.RootDirectory.GetUnixRelativePathTo(artifact),
                    Condition = PublishCondition
                };
            }
        }
    }
}

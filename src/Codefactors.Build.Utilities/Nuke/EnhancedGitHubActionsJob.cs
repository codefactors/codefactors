﻿// Copyright (c) 2024, Codefactors Ltd.
//
// Codefactors Ltd licenses this file to you under the following license(s):
//
//   * The MIT License, see https://opensource.org/license/mit/

/* IMPORTANT:
 * 
 * This class overrides functionality in the GitHubActionsJob class in Nuke.Common.  It should be kept
 * in sync with the original class in Nuke.Common.  The original class is located at:
 * https://github.com/nuke-build/nuke/blob/master/source/Nuke.Common/CI/GitHubActions/Configuration/GitHubActionsJob.cs
 * 
 * 24.10.2024 Updated to 8.1.2
 */

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

using Nuke.Common.CI.GitHubActions.Configuration;
using Nuke.Common.Tooling;
using Nuke.Common.Utilities;
using Nuke.Common.Utilities.Collections;

namespace Codefactors.Build.Utilities.Nuke;

public class EnhancedGitHubActionsJob : GitHubActionsJob
{
    public string[] Services { get; set; } = [];

    public override void Write(CustomFileWriter writer)
    {
        writer.WriteLine($"{Name}:");

        using (writer.Indent())
        {
            writer.WriteLine($"name: {Name}");
            writer.WriteLine($"runs-on: {Image.GetValue()}");

            if (Services.Length > 0)
            {
                writer.WriteLine("services:");

                using (writer.Indent())
                {
                    GitHubActionsServices.WriteYaml(Services[0], writer.WriteLine, writer.Indent);
                }
            }

            if (TimeoutMinutes > 0)
            {
                writer.WriteLine($"timeout-minutes: {TimeoutMinutes}");
            }

            if (!ConcurrencyGroup.IsNullOrWhiteSpace() || ConcurrencyCancelInProgress)
            {
                writer.WriteLine("concurrency:");
                using (writer.Indent())
                {
                    var group = ConcurrencyGroup;
                    if (group.IsNullOrWhiteSpace())
                    {
                        // create a default value that only cancels in-progress runs of the same workflow
                        // we don't fall back to github.ref which would disable multiple runs in main/master which is usually what is wanted
                        group = "${{ github.workflow }} @ ${{ github.event.pull_request.head.label || github.head_ref || github.run_id }}";
                    }

                    writer.WriteLine($"group: {group}");
                    if (ConcurrencyCancelInProgress)
                    {
                        writer.WriteLine("cancel-in-progress: true");
                    }
                }
            }

            if (!EnvironmentName.IsNullOrWhiteSpace())
            {
                if (EnvironmentUrl.IsNullOrWhiteSpace())
                {
                    writer.WriteLine($"environment: {EnvironmentName}");
                }
                else
                {
                    writer.WriteLine("environment:");
                    using (writer.Indent())
                    {
                        writer.WriteLine($"name: {EnvironmentName}");
                        writer.WriteLine($"url: {EnvironmentUrl}");
                    }
                }
            }

            writer.WriteLine("steps:");
            using (writer.Indent())
            {
                Steps.ForEach(x => x.Write(writer));
            }
        }
    }
}

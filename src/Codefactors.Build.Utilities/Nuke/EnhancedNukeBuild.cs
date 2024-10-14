// Copyright (c) 2024, Codefactors Ltd.
//
// Codefactors Ltd licenses this file to you under the following license(s):
//
//   * The MIT License, see https://opensource.org/license/mit/

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

using Nuke.Common;
using Nuke.Common.IO;
using Nuke.Common.Tools.Slack;
using System.IO.Compression;
using System.Text;
using static Nuke.Common.Tools.Slack.SlackTasks;

namespace Codefactors.Build.Utilities.Nuke;

public abstract class EnhancedNukeBuild : NukeBuild
{
    protected readonly StringBuilder _updateText = new StringBuilder();

    [Parameter]
    [Secret]
    protected readonly string SlackWebhook = default!;

    protected abstract string ProjectTitle { get; }

    protected override void OnBuildCreated()
    {
        AddNotification($"Building *{ProjectTitle}*...");

        base.OnBuildCreated();
    }

    protected override void OnTargetFailed(string target)
    {
        AddNotification($" • {target} failed");

        base.OnTargetFailed(target);
    }

    protected override void OnTargetSucceeded(string target)
    {
        AddNotification($" • {target} succeeded");

        base.OnTargetSucceeded(target);
    }

    protected override void OnBuildFinished()
    {
        AddNotification("Completed");

        NotifyBuildUpdate(_updateText.ToString());

        base.OnBuildFinished();
    }

    protected void NotifyBuildUpdate(string message)
    {
        if (SlackWebhook != null)
        {
            SendSlackMessage(_ => _
                    .SetText($"{DateTime.Now}: {message}"),
                $"https://hooks.slack.com/services/{SlackWebhook}");
        }
    }

    protected AbsolutePath CreateZipDeployment(in AbsolutePath artifactsDirectory, in AbsolutePath deploymentDirectory)
    {
        var zipFile = deploymentDirectory / "deployment.zip";

        if (File.Exists(zipFile))
            File.Delete(zipFile);

        if (!Directory.Exists(deploymentDirectory))
            Directory.CreateDirectory(deploymentDirectory);

        ZipFile.CreateFromDirectory(artifactsDirectory, zipFile);

        return zipFile;
    }

    protected void AddNotification(in string message) =>
        _updateText.AppendLine(message);
}
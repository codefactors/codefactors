// Copyright (c) 2024, Codefactors Ltd.
//
// Codefactors Ltd licenses this file to you under the following license(s):
//
//   * The MIT License, see https://opensource.org/license/mit/

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

using System.Text;

namespace Codefactors.Build.Utilities.Azure;

public class AppServiceDeployment
{
    private readonly Serilog.ILogger _logger;

    public AppServiceDeployment(in Serilog.ILogger logger)
    {
        _logger = logger;
    }

    /// <summary>
    /// Deploys the specified zip file to the specified App Service.
    /// </summary>
    /// <param name="zipFilePath">Path to zip file.</param>
    /// <param name="appServiceName">Either just the app service name, e.g., my-service, or where the URL contains a hash, then
    /// the full scm URL without https://, e.g., my-service-f8hqgcfibjckgqb1.scm.uksouth-01.azurewebsites.net</param>
    /// <param name="username">Deploy username.</param>
    /// <param name="password">Deploy password.</param>
    /// <returns>Task.</returns>
    /// <exception cref="InvalidOperationException">Thrown if the deployment fails.</exception>
    /// <remarks>App service name, username and password can be obtained from the profile.publishsettings file downloadable
    /// from the Azure portal.</remarks>
    public async Task DeployAsync(
        /* in */ string zipFilePath,
        /* in */ string appServiceName,
        /* in */ string username,
        /* in */ string password)
    {
        var base64Auth = Convert.ToBase64String(Encoding.Default.GetBytes($"{username}:{password}"));

        byte[] fileContents = File.ReadAllBytes(zipFilePath);

        using var memStream = new MemoryStream(fileContents);

        memStream.Position = 0;

        var content = new StreamContent(memStream);

        var httpClient = new HttpClient();

        httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Basic", base64Auth);

        var requestUrl = appServiceName.Contains(".scm.", StringComparison.OrdinalIgnoreCase) ?
            $"https://{appServiceName.Trim('/')}/api/zipdeploy" :
            $"https://{appServiceName}.scm.azurewebsites.net/api/zipdeploy";

        _logger.Information("Deploying {bytes} bytes to {url}", fileContents.Length, requestUrl);

        var response = await httpClient.PostAsync(requestUrl, content);

        if (!response.IsSuccessStatusCode)
            throw new InvalidOperationException(await response.Content.ReadAsStringAsync());
    }
}
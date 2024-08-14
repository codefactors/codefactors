// Copyright (c) 2024, Codefactors Ltd.
//
// Codefactors Ltd licenses this file to you under the following license(s):
//
//   * The MIT License, see https://opensource.org/license/mit/

namespace Codefactors.DataFabric;

/// <summary>
/// Provides access to the data fabric configuration values.
/// </summary>
public static class DataFabricConfiguration
{
    private const string DataFabricSection = "DataFabric";
    private const string SignalRHubPath = "SignalRHubPath";

    /// <summary>
    /// Gets the hub path for the SignalR transport.
    /// </summary>
    /// <param name="configuration"><see cref="ConfigurationManager"/> that provides access to the configuration data.</param>
    /// <returns>SignalR hub path (URL) if available.</returns>
    /// <exception cref="InvalidOperationException">Thrown if the configuration data is missing or invalid.</exception>
    public static string GetSignalRHubPath(ConfigurationManager configuration) =>
        configuration[$"{DataFabricSection}:{SignalRHubPath}"] ??
            throw new InvalidOperationException($"Configuration must include a '{DataFabricSection}' section, and within that section, a valid value for '{SignalRHubPath}'");
}

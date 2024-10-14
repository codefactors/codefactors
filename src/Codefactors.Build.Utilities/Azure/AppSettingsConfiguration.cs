// Copyright (c) 2024, Codefactors Ltd.
//
// Codefactors Ltd licenses this file to you under the following license(s):
//
//   * The MIT License, see https://opensource.org/license/mit/

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

using Nuke.Common.IO;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace Codefactors.Build.Utilities.Azure;

public class AppSettingsConfiguration : IDisposable
{
    private readonly AbsolutePath _configFile;
    private readonly Serilog.ILogger _logger;
    private readonly JsonNode _jsonContent;
    private bool _disposed;

    public AppSettingsConfiguration(in AbsolutePath configFile, in Serilog.ILogger logger)
    {
        _configFile = configFile;
        _logger = logger;

        if (!File.Exists(configFile))
            throw new FileNotFoundException(configFile);

        var content = File.ReadAllText(configFile);

        _jsonContent = JsonNode.Parse(content, new JsonNodeOptions { PropertyNameCaseInsensitive = true }) ??
            throw new InvalidOperationException($"Unable to parse configuration file '{configFile}'");
    }

    public void UpdateValue(in string section, in string key, in string value)
    {
        if (section.Contains('@'))
        {
            UpdateNestedValue(section, key, value);

            return;
        }

        var jsonSection = (section.Contains('/') ? GetConfigNode(section) : _jsonContent[section]) ??
            throw new ArgumentException($"Invalid section: '{section}'", nameof(section));

        UpdateNode(section, key, value, jsonSection);
    }

    private void UpdateNestedValue(in string section, in string key, in string value)
    {
        var parts = section.Split('/');

        var node = _jsonContent;

        for (var i = 0; i < parts.Length; i++)
        {
            if (parts[i].Contains('@'))
            {
                if (node is not JsonArray ja)
                    throw new ArgumentException($"Invalid configuration path: '{section}'", nameof(section));

                var nestedParts = parts[i].Trim('@').Split('=');

                node = ja.FirstOrDefault(n => n?[nestedParts[0]]?.GetValue<string>() == nestedParts[1]) ??
                    throw new ArgumentException($"Invalid configuration path: '{section} = no match for '{parts[i]}'", nameof(section));
            }
            else
            {
                node = node[parts[i]] ?? throw new ArgumentException($"Invalid configuration path: '{section}'", nameof(section));
            }
        }

        UpdateNode(section, key, value, node);
    }

    private void UpdateNode(string section, string key, string value, JsonNode node)
    {
        if (key.EndsWith("[]", StringComparison.OrdinalIgnoreCase))
        {
            var thisKey = key[..^2].Trim();

            var targetNode = node[thisKey];

            if (targetNode is not JsonArray ja)
                throw new ArgumentException($"Invalid configuration path: '{thisKey}' is not an array", nameof(key));

            ja.Clear();

            var values = value.Split(',');

            foreach (var v in values)
                ja.Add(v.Trim());
        }
        else
        {
            node[key] = value;
        }

        _logger.Information($"Updated {_configFile} entry {section} / {key}", _configFile, section, key);
    }

    private JsonNode GetConfigNode(in string configPath)
    {
        var pathParts = configPath.Split('/');

        JsonNode node = _jsonContent;

        foreach (var part in pathParts)
        {
            node = node[part] ??
                throw new ArgumentException($"Invalid configuration path: '{configPath}'", nameof(configPath));
        }

        return node;
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!_disposed)
        {
            if (disposing)
            {
                File.WriteAllText(_configFile, _jsonContent.ToJsonString(new JsonSerializerOptions { WriteIndented = true, TypeInfoResolver = JsonSerializerOptions.Default.TypeInfoResolver }));
            }

            _disposed = true;
        }
    }

    public void Dispose()
    {
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }
}

// Copyright (c) 2024, Codefactors Ltd.
//
// Codefactors Ltd licenses this file to you under the following license(s):
//
//   * The MIT License, see https://opensource.org/license/mit/

namespace Codefactors.Build.Utilities.Nuke;

public static class GitHubActionsServices
{
    public const string SqlServerService =
        """
        mssql:
          image: mcr.microsoft.com/mssql/server:2019-latest
          env:
            SA_PASSWORD: ${{ secrets.MSSQL_SA_PASSWORD }}
            ACCEPT_EULA: 'Y'
          ports:
            - 1433:1433
        """;

    public static void WriteYaml(string serviceText, Action<string> writer, Func<IDisposable> indentFunc)
    {
        var nl = serviceText.Contains("\r\n") ? "\r\n" : "\n";

        var lines = serviceText.Split(nl, StringSplitOptions.RemoveEmptyEntries);

        for (var i = 0; i < lines.Length; i++)
            WriteLineIndented(lines[i], writer, indentFunc);
    }

    private static void WriteLineIndented(string line, Action<string> writer, Func<IDisposable> indentFunc)
    {
        if (line.StartsWith("  ", StringComparison.OrdinalIgnoreCase))
        {
            using (indentFunc())
            {
                WriteLineIndented(line.Remove(0, 2), writer, indentFunc);
            }
        }
        else
            writer(line);
    }
}
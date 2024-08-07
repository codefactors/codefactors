// Copyright (c) 2024, Codefactors Ltd.
//
// Codefactors Ltd licenses this file to you under the following license(s):
//
//   * The MIT License, see https://opensource.org/license/mit/

using Microsoft.Extensions.Logging;
using Xunit.Abstractions;

namespace Codefactors.DataFabric.Tests;

public class TestOutputLogger<T> : ILogger<T>
{
    private readonly ITestOutputHelper _outputHelper;

    public TestOutputLogger(ITestOutputHelper outputHelper)
    {
        _outputHelper = outputHelper;
    }

    public bool IsEnabled(LogLevel logLevel)
    {
        return logLevel >= LogLevel.Warning;  // return true to enable all log levels
    }

    public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception, string> formatter)
    {
        _outputHelper.WriteLine(formatter(state, exception!));
    }

    IDisposable? ILogger.BeginScope<TState>(TState state)
    {
        throw new NotImplementedException();
    }
}
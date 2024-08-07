// Copyright (c) 2024, Codefactors Ltd.
//
// Codefactors Ltd licenses this file to you under the following license(s):
//
//   * The MIT License, see https://opensource.org/license/mit/

namespace Codefactors.DataFabric.Tests;

internal class TestEntityObject(Action<string> reporter)
{
    private readonly Action<string> _reporter = reporter;

    public Task<object> GetEmployers()
    {
        _reporter("GetEmployers");

        return Task.FromResult(GetResult("some employers"));
    }

    public Task<object> GetEmployees(Guid employerId)
    {
        _reporter("GetEmployees");
        return Task.FromResult(GetResult("some employers"));
    }

    public Task<object> GetEmployer(Guid employerId)
    {
        _reporter($"GetEmployer: {employerId}");
        return Task.FromResult(GetResult("some employer"));
    }

    public Task<object> GetEmployee(Guid employerId, Guid employeeId)
    {
        _reporter($"GetEmployee: {employeeId} for employer {employerId}");
        return Task.FromResult(GetResult("some employee"));
    }

    private object GetResult(string text) => text;
}
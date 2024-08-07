// Copyright (c) 2024, Codefactors Ltd.
//
// Codefactors Ltd licenses this file to you under the following license(s):
//
//   * The MIT License, see https://opensource.org/license/mit/

using FluentAssertions;

namespace Codefactors.DataFabric.Tests;

public partial class SubscriptionTreeTests
{
    [Fact]
    public void TestRegister()
    {
        var entityProviders = TestEntityProvider.MakeEntityProviders((text) => { });

        var tree = new SubscriptionTree();
        tree.RegisterSubscriptionPath("/employers", entityProviders[0]);

        tree.Children.Count.Should().Be(1);
        tree.Children.First().Value.Should().Be("employers");
        tree.Children.First().EntityProvider.Should().BeEquivalentTo(entityProviders[0]);

        Action act = () => tree.RegisterSubscriptionPath("/employers", entityProviders[0]);

        act.Should().Throw<ArgumentException>()
            .WithMessage("Subscription path already exists (Parameter 'path')");

        tree = new SubscriptionTree();

        tree.RegisterSubscriptionPath("/employers/{employerId}", entityProviders[1]);

        tree.Children.Count.Should().Be(1);
        tree.Children.First().Value.Should().Be("employers");
        tree.Children.First().EntityProvider.Should().BeNull();

        tree.Children.First().Children.Count.Should().Be(1);
        tree.Children.First().Children.First().Value.Should().Be("{employerId}");
        tree.Children.First().Children.First().EntityProvider.Should().BeEquivalentTo(entityProviders[1]);

        tree = new SubscriptionTree();

        tree.RegisterSubscriptionPath("/employers/{employerId}/employees", entityProviders[2]);

        tree.Children.Count.Should().Be(1);
        tree.Children.First().Value.Should().Be("employers");
        tree.Children.First().EntityProvider.Should().BeNull();

        tree.Children.First().Children.Count.Should().Be(1);
        tree.Children.First().Children.First().Value.Should().Be("{employerId}");
        tree.Children.First().Children.First().EntityProvider.Should().BeNull();

        tree.Children.First().Children.First().Children.Count.Should().Be(1);
        tree.Children.First().Children.First().Children.First().Value.Should().Be("employees");
        tree.Children.First().Children.First().Children.First().EntityProvider.Should().BeEquivalentTo(entityProviders[2]);

        tree.RegisterSubscriptionPath("/employers/{employerId}/payruns", entityProviders[3]);

        tree.Children.Count.Should().Be(1);
        tree.Children.First().Value.Should().Be("employers");
        tree.Children.First().EntityProvider.Should().BeNull();

        tree.Children.First().Children.Count.Should().Be(1);
        tree.Children.First().Children.First().Value.Should().Be("{employerId}");
        tree.Children.First().Children.First().EntityProvider.Should().BeNull();

        tree.Children.First().Children.First().Children.Count.Should().Be(2);
        tree.Children.First().Children.First().Children.Any(c => c.Value == "employees").Should().BeTrue();
        tree.Children.First().Children.First().Children.First(c => c.Value == "employees").EntityProvider.Should().BeEquivalentTo(entityProviders[2]);
        tree.Children.First().Children.First().Children.Any(c => c.Value == "payruns").Should().BeTrue();
        tree.Children.First().Children.First().Children.First(c => c.Value == "payruns").EntityProvider.Should().BeEquivalentTo(entityProviders[3]);
    }

    [Fact]
    public void TestRegisterEmpty()
    {
        TestEntityProvider? rootEntityProvider = null;

        var tree = new SubscriptionTree();

        Action act = () => tree.RegisterSubscriptionPath("/employers", rootEntityProvider!);

        act.Should().Throw<ArgumentException>()
            .WithMessage("Entity provider cannot be null (Parameter 'entityProvider')");
    }

    [Fact]
    public void TestRegisterBadPaths()
    {
        var entityProviders = TestEntityProvider.MakeEntityProviders((text) => { });

        var tree = new SubscriptionTree();

        Action act = () => tree.RegisterSubscriptionPath("/{hampsterId}/employers/", entityProviders[0]);

        act.Should().Throw<ArgumentException>()
            .WithMessage("Subscription path cannot start with a placeholder (Parameter 'path')");

        act = () => tree.RegisterSubscriptionPath("//Employees", entityProviders[0]);

        act.Should().Throw<ArgumentException>()
            .WithMessage("Subscription path cannot contain empty segments (Parameter 'path')");

        act = () => tree.RegisterSubscriptionPath("/Employers//Employees", entityProviders[0]);

        act.Should().Throw<ArgumentException>()
            .WithMessage("Subscription path cannot contain empty segments (Parameter 'path')");
    }

    [Fact]
    public void TestRegisterBadPlaceholders()
    {
        var entityProviders = TestEntityProvider.MakeEntityProviders((text) => { });

        var tree = new SubscriptionTree();

        string[] badPaths =
        [
            "/employers/{/employees",
            "/employers/}/employees",
            "/employers/{employees",
            "/employers/employees}",
            "/employers/{employeeId{x}}/foo",
            "/employers/}weasel{",
            "/employers/{{",
            "/employers/}}"
        ];

        foreach (var path in badPaths)
        {
            Action act = () => tree.RegisterSubscriptionPath(path, entityProviders[0]);

            act.Should().Throw<ArgumentException>()
                .WithMessage("Subscription path contains ill-formed placeholders (Parameter 'path')");
        }

        tree.RegisterSubscriptionPath("/employers/{employeeId}/foo", entityProviders[0]);

        Action act2 = () => tree.RegisterSubscriptionPath("/employers/{workerId}/foo2", entityProviders[0]);

        act2.Should().Throw<ArgumentException>()
            .WithMessage("Unexpected placeholder value '{workerId}'; placeholders must be consistent for a given path (Parameter 'segments')");
    }
}
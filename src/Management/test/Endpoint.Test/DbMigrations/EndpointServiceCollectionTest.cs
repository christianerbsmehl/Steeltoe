// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the Apache 2.0 License.
// See the LICENSE file in the project root for more information.

using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Steeltoe.Management.Endpoint.Test;
using Xunit;

namespace Steeltoe.Management.Endpoint.DbMigrations.Test;

public class EndpointServiceCollectionTest : BaseTest
{
    [Fact]
    public void AddEntityFrameworkActuator_ThrowsOnNulls()
    {
        const ServiceCollection nullServices = null;
        var services = new ServiceCollection();

        Action action1 = () => nullServices.AddDbMigrationsActuator();
        Action action2 = () => services.AddDbMigrationsActuator();

        action1.Should().ThrowExactly<ArgumentNullException>().Where(exception => exception.ParamName == "services");
        action2.Should().ThrowExactly<InvalidOperationException>();
    }

    [Fact]
    public void AddEntityFrameworkActuator_AddsCorrectServices()
    {
        var services = new ServiceCollection();

        var appSettings = new Dictionary<string, string>
        {
            ["management:endpoints:enabled"] = "false",
            ["management:endpoints:path"] = "/cloudfoundryapplication"
        };

        var configurationBuilder = new ConfigurationBuilder();
        configurationBuilder.AddInMemoryCollection(appSettings);
        IConfigurationRoot configurationRoot = configurationBuilder.Build();
        services.AddSingleton<IConfiguration>(configurationRoot);

        services.AddDbMigrationsActuator(configurationRoot);

        ServiceProvider serviceProvider = services.BuildServiceProvider();
        var options = serviceProvider.GetService<IDbMigrationsOptions>();
        options.Should().NotBeNull();
        var ep = serviceProvider.GetService<DbMigrationsEndpoint>();
        ep.Should().NotBeNull();
    }
}
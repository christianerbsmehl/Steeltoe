// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the Apache 2.0 License.
// See the LICENSE file in the project root for more information.

using Microsoft.Extensions.Configuration;
using Xunit;

namespace Steeltoe.Management.Endpoint.Test;

public class ExposureTest
{
    [Fact]
    public void ExposureReturnsDefaults()
    {
        var appSettings = new Dictionary<string, string>();
        IConfigurationRoot configurationRoot = new ConfigurationBuilder().AddInMemoryCollection(appSettings).Build();

        var exp = new Exposure(configurationRoot);

        Assert.Contains("health", exp.Include);
        Assert.Contains("info", exp.Include);
        Assert.Null(exp.Exclude);
    }

    [Fact]
    public void ExposureBindsToSteeltoeSettings()
    {
        var appSettings = new Dictionary<string, string>
        {
            ["management:endpoints:actuator:exposure:include:0"] = "httptrace",
            ["management:endpoints:actuator:exposure:include:1"] = "dbmigrations",
            ["management:endpoints:actuator:exposure:exclude:0"] = "trace",
            ["management:endpoints:actuator:exposure:exclude:1"] = "env"
        };

        IConfigurationRoot configurationRoot = new ConfigurationBuilder().AddInMemoryCollection(appSettings).Build();

        var exp = new Exposure(configurationRoot);

        Assert.Contains("httptrace", exp.Include);
        Assert.Contains("dbmigrations", exp.Include);
        Assert.Contains("trace", exp.Exclude);
        Assert.Contains("env", exp.Exclude);
    }

    [Fact]
    public void ExposureBindsToSpringSettings()
    {
        var appSettings = new Dictionary<string, string>
        {
            ["management:endpoints:web:exposure:include"] = "heapdump,env",
            ["management:endpoints:web:exposure:exclude"] = "dbmigrations,info"
        };

        IConfigurationRoot configurationRoot = new ConfigurationBuilder().AddInMemoryCollection(appSettings).Build();

        var exp = new Exposure(configurationRoot);

        Assert.Contains("heapdump", exp.Include);
        Assert.Contains("env", exp.Include);
        Assert.Contains("dbmigrations", exp.Exclude);
        Assert.Contains("info", exp.Exclude);
    }

    [Fact]
    public void ExposureDoesNotThrowOnInvalidSpringSettings()
    {
        var appSettings = new Dictionary<string, string>
        {
            ["management:endpoints:web:exposure:include"] = "heapdump;env"
        };

        IConfigurationRoot configurationRoot = new ConfigurationBuilder().AddInMemoryCollection(appSettings).Build();

        var exp = new Exposure(configurationRoot);

        Assert.Contains("heapdump;env", exp.Include);
        Assert.Null(exp.Exclude);
    }
}
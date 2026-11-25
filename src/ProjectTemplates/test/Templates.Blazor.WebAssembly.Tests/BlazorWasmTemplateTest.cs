// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.AspNetCore.InternalTesting;
using Newtonsoft.Json.Linq;
using Templates.Test.Helpers;
using Xunit;
using Xunit.Abstractions;
using Xunit.Sdk;

namespace Templates.Blazor.Test;

#pragma warning disable xUnit1041 // Fixture arguments to test classes must have fixture sources

public class BlazorWasmTemplateTest : BlazorTemplateTest
{
    public BlazorWasmTemplateTest(ProjectFactoryFixture projectFactory)
        : base(projectFactory) { }

    public override string ProjectType { get; } = "blazorwasm";

    [Theory]
    [InlineData(false, true)]
    [InlineData(true, true)]
    [InlineData(false, false)]
    [InlineData(true, false)]
    public async Task BlazorWasmStandalonePublishHonorsStartupScriptFingerprinting(bool noBuild, bool fingerprint)
    {
        var project = await CreateBuildPublishAsync(onlyCreate: true);
        var fingerprintArgument = $"-p:BlazorFingerprintBlazorJs={fingerprint}";
        if (noBuild)
        {
            await project.RunDotNetBuildAsync(additionalArgs: $"-p:Configuration=Release {fingerprintArgument}");
        }

        await project.RunDotNetPublishAsync(
            additionalArgs: noBuild ? $"--no-build -p:BuildProjectReferences=false {fingerprintArgument}" : fingerprintArgument,
            noRestore: noBuild);

        var wwwroot = Path.Combine(project.TemplatePublishDir, "wwwroot");
        var html = File.ReadAllText(Path.Combine(wwwroot, "index.html"));
        var script = Regex.Match(html, fingerprint
            ? @"src=""(?<path>_framework/blazor\.webassembly\.[a-z0-9]{10}\.js)"""
            : @"src=""(?<path>_framework/blazor\.webassembly\.js)""");
        Assert.True(script.Success, $"Unexpected startup script reference in published HTML: {html}");
        Assert.DoesNotContain("#[.{fingerprint}]", html);
        var scriptPath = script.Groups["path"].Value;
        Assert.True(File.Exists(Path.Combine(wwwroot, scriptPath)), $"Missing published startup script: {scriptPath}");

        if (fingerprint)
        {
            var importMapJson = Regex.Match(html, "<script type=\"importmap\">(?<map>.*?)</script>", RegexOptions.Singleline).Groups["map"].Value;
            using var importMapDocument = JsonDocument.Parse(importMapJson);
            var importMap = importMapDocument.RootElement;
            Assert.Equal($"./{scriptPath}", importMap.GetProperty("imports").GetProperty("./_framework/blazor.webassembly.js").GetString());
            Assert.StartsWith("sha256-", importMap.GetProperty("integrity").GetProperty($"./{scriptPath}").GetString());
        }
    }

    [Fact]
    public async Task BlazorWasmStandaloneTemplateCanCreateBuildPublish()
    {
        var project = await CreateBuildPublishAsync();

        // The service worker assets manifest isn't generated for non-PWA projects
        var publishDir = Path.Combine(project.TemplatePublishDir, "wwwroot");
        Assert.False(File.Exists(Path.Combine(publishDir, "service-worker-assets.js")), "Non-PWA templates should not produce service-worker-assets.js");
    }

    [Fact]
    public async Task BlazorWasmStandaloneTemplateEmptyCanCreateBuildPublish()
    {
        var project = await CreateBuildPublishAsync(args: new[] { ArgConstants.Empty }); ;

        // The service worker assets manifest isn't generated for non-PWA projects
        var publishDir = Path.Combine(project.TemplatePublishDir, "wwwroot");
        Assert.False(File.Exists(Path.Combine(publishDir, "service-worker-assets.js")), "Non-PWA templates should not produce service-worker-assets.js");
    }

    [Fact]
    public async Task BlazorWasmStandaloneTemplateNoHttpsCanCreateBuildPublish()
    {
        var project = await CreateBuildPublishAsync(args: new[] { ArgConstants.NoHttps });

        // The service worker assets manifest isn't generated for non-PWA projects
        var publishDir = Path.Combine(project.TemplatePublishDir, "wwwroot");
        Assert.False(File.Exists(Path.Combine(publishDir, "service-worker-assets.js")), "Non-PWA templates should not produce service-worker-assets.js");
    }

    [Fact]
    public async Task BlazorWasmStandaloneTemplateNoHttpsEmptyCanCreateBuildPublish()
    {
        var project = await CreateBuildPublishAsync(args: new[] { ArgConstants.NoHttps, ArgConstants.Empty });

        // The service worker assets manifest isn't generated for non-PWA projects
        var publishDir = Path.Combine(project.TemplatePublishDir, "wwwroot");
        Assert.False(File.Exists(Path.Combine(publishDir, "service-worker-assets.js")), "Non-PWA templates should not produce service-worker-assets.js");
    }

    [Fact]
    public Task BlazorWasmStandalonePwaTemplateCanCreateBuildPublish()
        => CreateBuildPublishAsync(args: new[] { ArgConstants.Pwa });

    [Fact]
    public Task BlazorWasmStandalonePwaEmptyTemplateCanCreateBuildPublish()
        => CreateBuildPublishAsync(args: new[] { ArgConstants.Pwa, ArgConstants.Empty });

    [Fact]
    public Task BlazorWasmStandalonePwaTemplateNoHttpsCanCreateBuildPublish()
        => CreateBuildPublishAsync(args: new[] { ArgConstants.Pwa, ArgConstants.NoHttps });

    [Fact]
    public Task BlazorWasmStandalonePwaEmptyTemplateNoHttpsCanCreateBuildPublish()
        => CreateBuildPublishAsync(args: new[] { ArgConstants.Pwa, ArgConstants.NoHttps, ArgConstants.Empty });
}

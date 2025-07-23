# xUnit v3 Testing Framework Setup Guide

**Document**: xUnit v3 Testing Framework Configuration
**Version**: 1.0
**Created**: 2025-07-23
**Purpose**: Comprehensive guide for migrating to xUnit v3 with 80%+ coverage targets

---

## 📋 Executive Summary

This document provides complete setup instructions for migrating the claude dotnet project from xUnit v2.9.3 to xUnit v3 (preview 0.7.2) with comprehensive testing strategies to achieve 80%+ unit test coverage and 70%+ integration test coverage.

---

## 🎯 Testing Objectives

- **Unit Test Coverage**: >80% with xUnit v3 and FluentAssertions
- **Integration Test Coverage**: >70% with Testcontainers and API testing
- **Performance Tests**: BenchmarkDotNet with regression detection
- **Compatibility Tests**: NPM feature parity validation
- **Cross-Platform**: Test on Windows, macOS, and Ubuntu
- **CI/CD Integration**: GitHub Actions with SonarCloud reporting

---

## 📦 Package Updates Required

### Directory.Packages.props Updates

```xml
<!-- Testing Framework Updates -->
<PackageVersion Include="xunit" Version="3.0.0-preview.7.2" />
<PackageVersion Include="xunit.runner.visualstudio" Version="3.0.0-preview.7.3" />
<PackageVersion Include="xunit.runner.console" Version="3.0.0-preview.7.2" />
<PackageVersion Include="xunit.v3.assert" Version="3.0.0-preview.7.2" />
<PackageVersion Include="xunit.v3.core" Version="3.0.0-preview.7.2" />

<!-- Test Utilities (keep current versions) -->
<PackageVersion Include="Microsoft.NET.Test.Sdk" Version="17.10.0" />
<PackageVersion Include="FluentAssertions" Version="6.12.0" />
<PackageVersion Include="Moq" Version="4.20.70" />
<PackageVersion Include="System.IO.Abstractions.TestingHelpers" Version="21.0.2" />
<PackageVersion Include="Testcontainers" Version="3.9.0" />
<PackageVersion Include="Microsoft.AspNetCore.Mvc.Testing" Version="9.0.0" />

<!-- Coverage Tools -->
<PackageVersion Include="coverlet.collector" Version="6.0.2" />
<PackageVersion Include="coverlet.msbuild" Version="6.0.2" />
<PackageVersion Include="ReportGenerator" Version="5.3.4" />

<!-- System.CommandLine Update -->
<PackageVersion Include="System.CommandLine" Version="2.0.0-beta4.24324.3" />
<PackageVersion Include="System.CommandLine.Hosting" Version="2.0.0-beta4.24324.3" />
```

---

## 🏗️ Test Project Structure

### Unit Tests Structure
```
tests/UnitTests/
├── Commands/
│   ├── AnalyzeCommandTests.cs
│   ├── ImplementCommandTests.cs
│   └── InteractiveCommandTests.cs
├── Services/
│   ├── FileSystemServiceTests.cs
│   ├── ConfigurationServiceTests.cs
│   └── AIProviderTests.cs
├── Tools/
│   ├── FileOperationToolsTests.cs
│   ├── SearchToolsTests.cs
│   └── ExecutionToolsTests.cs
├── MCP/
│   ├── MCPServerTests.cs
│   └── ProtocolTests.cs
├── Common/
│   ├── TestBase.cs
│   ├── TestFixtures.cs
│   └── TestHelpers.cs
└── GlobalUsings.cs
```

### Integration Tests Structure
```
tests/IntegrationTests/
├── CLI/
│   ├── CommandIntegrationTests.cs
│   └── EndToEndScenarioTests.cs
├── NPM/
│   ├── CompatibilityTests.cs
│   └── FeatureParityTests.cs
├── MCP/
│   ├── ServerIntegrationTests.cs
│   └── ProtocolIntegrationTests.cs
├── Performance/
│   ├── StartupPerformanceTests.cs
│   └── MemoryUsageTests.cs
├── CrossPlatform/
│   ├── WindowsTests.cs
│   ├── MacOSTests.cs
│   └── LinuxTests.cs
└── TestInfrastructure/
    ├── IntegrationTestBase.cs
    ├── TestContainersFixture.cs
    └── WebApplicationFactoryFixture.cs
```

---

## 🔧 xUnit v3 Migration Guide

### 1. Key Changes from v2 to v3

```csharp
// xUnit v2 (old)
[Fact]
public void TestMethod()
{
    Assert.Equal(expected, actual);
}

// xUnit v3 (new)
[Fact]
public async Task TestMethod()
{
    await Assert.That(() => actual == expected);
}

// xUnit v3 improved async support
[Fact]
public async Task AsyncTestWithCancellation(CancellationToken cancellationToken)
{
    var result = await service.ExecuteAsync(cancellationToken);
    await Assert.That(() => result.IsSuccess);
}
```

### 2. Test Base Classes

```csharp
// tests/UnitTests/Common/TestBase.cs
using Xunit.v3;

public abstract class TestBase : IAsyncLifetime
{
    protected IServiceProvider ServiceProvider { get; private set; }
    protected ITestOutputHelper Output { get; }

    protected TestBase(ITestOutputHelper output)
    {
        Output = output;
    }

    public virtual async Task InitializeAsync()
    {
        var services = new ServiceCollection();
        ConfigureServices(services);
        ServiceProvider = services.BuildServiceProvider();
        await Task.CompletedTask;
    }

    public virtual async Task DisposeAsync()
    {
        if (ServiceProvider is IAsyncDisposable asyncDisposable)
            await asyncDisposable.DisposeAsync();
    }

    protected virtual void ConfigureServices(IServiceCollection services)
    {
        services.AddLogging(builder => builder.AddXUnit(Output));
        services.AddSingleton<IFileSystem, MockFileSystem>();
    }
}
```

### 3. Test Categories and Traits

```csharp
[Trait("Category", "Unit")]
[Trait("Component", "Commands")]
public class AnalyzeCommandTests : TestBase
{
    [Fact]
    [Trait("Priority", "Critical")]
    public async Task Execute_WithValidInput_ReturnsSuccess()
    {
        // Arrange
        var command = new AnalyzeCommand();
        
        // Act
        var result = await command.ExecuteAsync(context);
        
        // Assert
        await Assert.That(() => result == 0);
        result.Should().Be(0);
    }
}
```

---

## 📊 Coverage Configuration

### 1. Coverage Collection Setup

```xml
<!-- In test project files -->
<PropertyGroup>
  <CollectCoverage>true</CollectCoverage>
  <CoverletOutputFormat>opencover,cobertura,json</CoverletOutputFormat>
  <CoverletOutput>./TestResults/</CoverletOutput>
  <ExcludeByAttribute>GeneratedCodeAttribute,ExcludeFromCodeCoverageAttribute</ExcludeByAttribute>
  <ExcludeByFile>**/Program.cs,**/*.Designer.cs</ExcludeByFile>
  <Threshold>80</Threshold>
  <ThresholdType>line,branch,method</ThresholdType>
  <ThresholdStat>total</ThresholdStat>
</PropertyGroup>
```

### 2. Global Coverage Settings

```json
// coverlet.runsettings.json
{
  "coverlet": {
    "excludes": [
      "[xunit.*]*",
      "[*]*.Program",
      "[*]*.Startup",
      "[*.Tests]*",
      "[*.IntegrationTests]*"
    ],
    "includeTestAssembly": false,
    "skipAutoProps": true,
    "mergeWith": "./TestResults/coverage.json"
  }
}
```

### 3. SonarCloud Integration

```yaml
# .github/workflows/dotnet.yml updates
- name: Run Tests with Coverage
  run: |
    dotnet test \
      --no-build \
      --verbosity normal \
      --collect:"XPlat Code Coverage" \
      --results-directory ./TestResults \
      -- DataCollectionRunSettings.DataCollectors.DataCollector.Configuration.Format=opencover

- name: Convert Coverage to SonarCloud Format
  run: |
    dotnet tool install --global dotnet-reportgenerator-globaltool
    reportgenerator \
      -reports:./TestResults/**/coverage.opencover.xml \
      -targetdir:./TestResults/CoverageReport \
      -reporttypes:SonarQube
```

---

## 🧪 Testing Patterns & Best Practices

### 1. Command Testing Pattern

```csharp
public class AnalyzeCommandTests : TestBase
{
    private readonly Mock<IFileSystemService> _fileSystemMock;
    private readonly Mock<IAIProvider> _aiProviderMock;
    
    [Fact]
    public async Task Analyze_ProjectScope_ProcessesAllFiles()
    {
        // Arrange
        var files = new[] { "file1.cs", "file2.cs" };
        _fileSystemMock.Setup(x => x.EnumerateAsync(It.IsAny<string>(), "*.cs", default))
            .Returns(files.ToAsyncEnumerable());
        
        var command = new AnalyzeCommand(_fileSystemMock.Object, _aiProviderMock.Object)
        {
            Scope = "project",
            Focus = "performance"
        };
        
        // Act
        var result = await command.ExecuteAsync(new InvocationContext());
        
        // Assert
        result.Should().Be(0);
        _fileSystemMock.Verify(x => x.EnumerateAsync(It.IsAny<string>(), "*.cs", default), Times.Once);
        _aiProviderMock.Verify(x => x.SendMessageAsync(It.IsAny<AIRequest>(), default), Times.Exactly(files.Length));
    }
}
```

### 2. Tool Testing Pattern

```csharp
public class FileOperationToolTests : TestBase
{
    [Theory]
    [InlineData("test.txt", "content")]
    [InlineData("path/to/file.cs", "class Test { }")]
    public async Task WriteTool_ValidPath_WritesContent(string path, string content)
    {
        // Arrange
        var fileSystem = new MockFileSystem();
        var tool = new WriteTool(fileSystem);
        var request = new ToolRequest { Path = path, Content = content };
        
        // Act
        var result = await tool.ExecuteAsync(request, CancellationToken.None);
        
        // Assert
        result.Should().BeOfType<SuccessResult>();
        fileSystem.File.Exists(path).Should().BeTrue();
        fileSystem.File.ReadAllText(path).Should().Be(content);
    }
}
```

### 3. Integration Testing Pattern

```csharp
public class CLIIntegrationTests : IntegrationTestBase
{
    [Fact]
    public async Task CLI_AnalyzeCommand_EndToEnd()
    {
        // Arrange
        await using var container = new ContainerBuilder()
            .WithImage("mcr.microsoft.com/dotnet/sdk:9.0")
            .WithCommand("claude", "analyze", "--scope", "project")
            .Build();
        
        // Act
        await container.StartAsync();
        var exitCode = await container.GetExitCodeAsync();
        var output = await container.GetLogsAsync();
        
        // Assert
        exitCode.Should().Be(0);
        output.Should().Contain("Analysis complete");
    }
}
```

---

## 🔄 CI/CD Integration Updates

### GitHub Actions Workflow Updates

```yaml
# .github/workflows/dotnet.yml
jobs:
  test:
    strategy:
      matrix:
        os: [ubuntu-latest, windows-latest, macos-latest]
        include:
          - os: ubuntu-latest
            codecov_os: linux
          - os: windows-latest
            codecov_os: windows
          - os: macos-latest
            codecov_os: macos
    
    steps:
    - name: Setup .NET 9.0
      uses: actions/setup-dotnet@v4
      with:
        dotnet-version: 9.0.x
    
    - name: Run Unit Tests
      run: |
        dotnet test tests/UnitTests/Claude.UnitTests.csproj \
          --configuration Release \
          --logger "trx;LogFileName=unit-tests.trx" \
          --logger "console;verbosity=detailed" \
          --collect:"XPlat Code Coverage" \
          --results-directory ./TestResults/Unit \
          -- DataCollectionRunSettings.DataCollectors.DataCollector.Configuration.Format=opencover,cobertura
    
    - name: Run Integration Tests
      run: |
        dotnet test tests/IntegrationTests/Claude.IntegrationTests.csproj \
          --configuration Release \
          --logger "trx;LogFileName=integration-tests.trx" \
          --collect:"XPlat Code Coverage" \
          --results-directory ./TestResults/Integration
    
    - name: Generate Coverage Report
      run: |
        dotnet tool install --global dotnet-reportgenerator-globaltool
        reportgenerator \
          -reports:"./TestResults/**/coverage.cobertura.xml" \
          -targetdir:"./TestResults/CoverageReport" \
          -reporttypes:"Html;Cobertura;SonarQube;Badges"
    
    - name: Check Coverage Thresholds
      run: |
        dotnet tool install --global dotnet-coverage
        dotnet-coverage merge ./TestResults/**/coverage.cobertura.xml \
          --output ./TestResults/merged-coverage.xml \
          --output-format cobertura
        
        # Fail if coverage < 80%
        coverage=$(xmllint --xpath "string(//coverage/@line-rate)" ./TestResults/merged-coverage.xml)
        if (( $(echo "$coverage < 0.80" | bc -l) )); then
          echo "Coverage ${coverage} is below 80% threshold"
          exit 1
        fi
```

---

## 📈 Test Implementation Priorities

### Phase 1: Core Command Tests (Week 1)
1. AnalyzeCommand unit tests
2. ImplementCommand unit tests
3. InteractiveCommand unit tests
4. Command integration tests

### Phase 2: Service Layer Tests (Week 2)
1. FileSystemService tests
2. ConfigurationService tests
3. AIProvider tests with mocking
4. Service integration tests

### Phase 3: Tool System Tests (Week 3)
1. File operation tools
2. Search and discovery tools
3. Execution tools
4. Tool orchestration tests

### Phase 4: Advanced Testing (Week 4)
1. MCP protocol tests
2. NPM compatibility tests
3. Performance benchmarks
4. Cross-platform validation

---

## 🎯 Success Metrics

- **Unit Test Coverage**: ≥80% line, branch, and method coverage
- **Integration Test Coverage**: ≥70% scenario coverage
- **Test Execution Time**: <2 minutes for unit tests, <5 minutes for integration
- **Cross-Platform Success**: 100% pass rate on Windows, macOS, Ubuntu
- **Performance Regression**: <5% variance in benchmark results
- **NPM Compatibility**: 100% feature parity validation

---

**Next Steps**:
1. Update Directory.Packages.props with xUnit v3 packages
2. Create test base classes and infrastructure
3. Implement first set of command unit tests
4. Update CI/CD pipeline for coverage reporting
5. Create test documentation and guidelines

**Document Status**: Complete
**Implementation Ready**: Yes
**Estimated Implementation Time**: 4 weeks
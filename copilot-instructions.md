# GitHub Copilot Instructions for claude dotnet

This file provides comprehensive instructions for GitHub Copilot to perform optimally on the **claude dotnet** project - a high-performance .NET 9.0 reimplementation of Anthropic's Claude Code CLI.

## 🎯 Project Overview

**claude dotnet** is a complete reimplementation of Anthropic's Claude Code CLI using C# 12 and .NET 9.0, distributed as a .NET Global Tool. The project aims to deliver 2-5x performance improvement while maintaining 100% feature parity with the original Node.js implementation.

### Key Technologies
- **Runtime**: .NET 9.0
- **Language**: C# 12 with nullable reference types
- **CLI Framework**: System.CommandLine v2.0.0-beta4
- **Hosting**: Microsoft.Extensions.Hosting
- **DI Container**: Microsoft.Extensions.DependencyInjection
- **Testing**: xUnit v3 with 80%+ coverage target
- **Package Management**: Directory.Packages.props (centralized)

## 🏗️ Architecture Patterns

### Core Architectural Patterns
- **Command Pattern**: System.CommandLine integration with structured command processing
- **Strategy Pattern**: Tool-based operation handling with dynamic discovery
- **Plugin Architecture**: MEF-based extensibility for MCP servers
- **Factory Pattern**: AI provider and tool instantiation with DI
- **REPL Pattern**: Interactive conversational interface as primary mode
- **Repository Pattern**: Configuration and state management

### Project Structure
```
src/Claude/                 # Main CLI application
├── Program.cs              # Entry point with DI configuration
├── Commands/               # Command implementations
├── Services/               # Core service interfaces and implementations
├── Tools/                  # Tool system (Read, Write, Edit, etc.)
├── MCP/                    # Model Context Protocol implementation
├── AI/                     # AI provider abstractions
└── Configuration/          # Multi-layer configuration system

tests/
├── UnitTests/              # Unit tests with 80%+ coverage
└── IntegrationTests/       # End-to-end testing
```

## 🎨 Code Style Guidelines

### Naming Conventions
```csharp
// Interfaces: IPascalCase
public interface IFileSystemService { }

// Classes: PascalCase
public class FileSystemService : IFileSystemService { }

// Methods: PascalCase with Async suffix for async methods
public async Task<string> ReadFileAsync(string path, CancellationToken cancellationToken = default) { }

// Properties: PascalCase
public string FileName { get; set; }

// Fields: _camelCase (private), PascalCase (public)
private readonly ILogger<FileSystemService> _logger;

// Constants: UPPER_CASE
private const string DEFAULT_CONFIG_FILE = "config.json";
```

### Code Organization
```csharp
public class ExampleService : IExampleService
{
    // Fields (private readonly first)
    private readonly ILogger<ExampleService> _logger;
    private readonly IConfiguration _configuration;
    
    // Constructor
    public ExampleService(ILogger<ExampleService> logger, IConfiguration configuration)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
    }
    
    // Properties
    public string Name { get; set; } = string.Empty;
    
    // Public methods
    public async Task<Result> ExecuteAsync(CancellationToken cancellationToken = default)
    {
        // Implementation
    }
    
    // Private methods
    private void ValidateInput(string input)
    {
        // Implementation
    }
}
```

### Error Handling Patterns
```csharp
// Use Result pattern for operations that can fail
public record Result<T>(bool IsSuccess, T? Value, string? Error);

// Comprehensive exception handling with logging
try
{
    var result = await operation.ExecuteAsync(cancellationToken);
    _logger.LogInformation("Operation completed successfully");
    return result;
}
catch (OperationCanceledException)
{
    _logger.LogInformation("Operation was cancelled");
    throw;
}
catch (Exception ex)
{
    _logger.LogError(ex, "Operation failed with error: {Error}", ex.Message);
    return Result<T>.Failure($"Operation failed: {ex.Message}");
}
```

## 🔧 Service Implementation Patterns

### Service Interface Design
```csharp
public interface IFileSystemService
{
    Task<string> ReadFileAsync(string path, CancellationToken cancellationToken = default);
    Task WriteFileAsync(string path, string content, CancellationToken cancellationToken = default);
    Task<bool> ExistsAsync(string path, CancellationToken cancellationToken = default);
    Task<FileInfo> GetFileInfoAsync(string path, CancellationToken cancellationToken = default);
    IAsyncEnumerable<FileSystemEntry> EnumerateAsync(string path, string pattern, CancellationToken cancellationToken = default);
}
```

### Dependency Injection Registration
```csharp
// In Program.cs CreateHostBuilder method
services.AddSingleton<IFileSystemService, FileSystemService>();
services.AddSingleton<IConfigurationService, ConfigurationService>();
services.AddTransient<IToolRegistry, ToolRegistry>();
services.AddScoped<IREPLService, REPLService>();
```

### Configuration Service Pattern
```csharp
public interface IConfigurationService
{
    T GetValue<T>(string key, T defaultValue = default!);
    Task SetValueAsync<T>(string key, T value, CancellationToken cancellationToken = default);
    Task<bool> HasKeyAsync(string key, CancellationToken cancellationToken = default);
    Task SaveAsync(CancellationToken cancellationToken = default);
}
```

## 🛠️ Tool System Implementation

### Tool Interface Pattern
```csharp
public interface ITool
{
    string Name { get; }
    string Description { get; }
    ToolCapabilities Capabilities { get; }
    Task<ToolResult> ExecuteAsync(ToolRequest request, CancellationToken cancellationToken = default);
}

public abstract record ToolResult;
public record SuccessResult(string Output, Dictionary<string, object>? Metadata = null) : ToolResult;
public record ErrorResult(string Error, Exception? Exception = null) : ToolResult;

public record ToolRequest(
    string Operation,
    Dictionary<string, object> Parameters,
    PermissionContext Permissions);
```

### Tool Implementation Example
```csharp
public class ReadFileTool : ITool
{
    private readonly IFileSystemService _fileSystem;
    private readonly ILogger<ReadFileTool> _logger;
    
    public string Name => "read_file";
    public string Description => "Read contents of a file";
    public ToolCapabilities Capabilities => ToolCapabilities.FileRead;
    
    public ReadFileTool(IFileSystemService fileSystem, ILogger<ReadFileTool> logger)
    {
        _fileSystem = fileSystem ?? throw new ArgumentNullException(nameof(fileSystem));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }
    
    public async Task<ToolResult> ExecuteAsync(ToolRequest request, CancellationToken cancellationToken = default)
    {
        try
        {
            if (!request.Parameters.TryGetValue("path", out var pathObj) || pathObj is not string path)
            {
                return new ErrorResult("Missing or invalid 'path' parameter");
            }
            
            var content = await _fileSystem.ReadFileAsync(path, cancellationToken);
            _logger.LogInformation("Successfully read file: {Path}", path);
            
            return new SuccessResult(content, new Dictionary<string, object>
            {
                ["file_path"] = path,
                ["content_length"] = content.Length
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to read file: {Path}", request.Parameters.GetValueOrDefault("path"));
            return new ErrorResult($"Failed to read file: {ex.Message}", ex);
        }
    }
}
```

## 🎮 Command Implementation Patterns

### System.CommandLine Command Pattern
```csharp
[Command("analyze")]
public class AnalyzeCommand : ICommand
{
    private readonly IFileSystemService _fileSystem;
    private readonly ILogger<AnalyzeCommand> _logger;
    
    [Option("--scope", Description = "Analysis scope (file|module|project|system)")]
    public string Scope { get; set; } = "project";
    
    [Option("--focus", Description = "Focus area (performance|security|quality|architecture)")]
    public string? Focus { get; set; }
    
    [Option("--output", Description = "Output format (text|json|markdown)")]
    public OutputFormat Output { get; set; } = OutputFormat.Text;
    
    [Argument("files", Description = "Files to analyze")]
    public string[] Files { get; set; } = Array.Empty<string>();
    
    public AnalyzeCommand(IFileSystemService fileSystem, ILogger<AnalyzeCommand> logger)
    {
        _fileSystem = fileSystem ?? throw new ArgumentNullException(nameof(fileSystem));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }
    
    public async Task<int> InvokeAsync(InvocationContext context)
    {
        var cancellationToken = context.GetCancellationToken();
        
        try
        {
            _logger.LogInformation("Starting analysis with scope: {Scope}, focus: {Focus}", Scope, Focus);
            
            // Implementation logic here
            
            return 0; // Success
        }
        catch (OperationCanceledException)
        {
            _logger.LogInformation("Analysis was cancelled");
            return 1;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Analysis failed");
            return 1;
        }
    }
}
```

## 🧪 Testing Patterns

### Unit Test Structure
```csharp
public class FileSystemServiceTests
{
    private readonly IFileSystemService _fileSystemService;
    private readonly ILogger<FileSystemService> _logger;
    
    public FileSystemServiceTests()
    {
        _logger = Substitute.For<ILogger<FileSystemService>>();
        _fileSystemService = new FileSystemService(_logger);
    }
    
    [Fact]
    public async Task ReadFileAsync_WithValidPath_ReturnsContent()
    {
        // Arrange
        var path = "test.txt";
        var expectedContent = "Hello, World!";
        
        // Act
        var result = await _fileSystemService.ReadFileAsync(path);
        
        // Assert
        result.Should().Be(expectedContent);
    }
    
    [Theory]
    [InlineData("")]
    [InlineData(null)]
    [InlineData("   ")]
    public async Task ReadFileAsync_WithInvalidPath_ThrowsArgumentException(string invalidPath)
    {
        // Act & Assert
        await _fileSystemService.Invoking(x => x.ReadFileAsync(invalidPath))
            .Should().ThrowAsync<ArgumentException>();
    }
}
```

### Integration Test Pattern
```csharp
public class AnalyzeCommandIntegrationTests : IClassFixture<TestWebApplicationFactory>
{
    private readonly TestWebApplicationFactory _factory;
    
    public AnalyzeCommandIntegrationTests(TestWebApplicationFactory factory)
    {
        _factory = factory;
    }
    
    [Fact]
    public async Task AnalyzeCommand_WithProjectScope_ReturnsAnalysis()
    {
        // Arrange
        using var scope = _factory.Services.CreateScope();
        var command = scope.ServiceProvider.GetRequiredService<AnalyzeCommand>();
        
        // Act
        var result = await command.InvokeAsync(new InvocationContext());
        
        // Assert
        result.Should().Be(0);
    }
}
```

## 🔄 REPL Implementation Patterns

### REPL Service Interface
```csharp
public interface IREPLService
{
    Task StartAsync(CancellationToken cancellationToken = default);
    Task<REPLResponse> ProcessInputAsync(string input, REPLContext context, CancellationToken cancellationToken = default);
    Task<REPLSession> CreateSessionAsync(string? workingDirectory = null);
    Task<bool> ResumeSessionAsync(string sessionId, CancellationToken cancellationToken = default);
}

public record REPLResponse(
    string Output,
    REPLContext UpdatedContext,
    bool ShouldExit = false);

public record REPLContext(
    string SessionId,
    string WorkingDirectory,
    Dictionary<string, object> Variables,
    List<ConversationTurn> History);
```

### Session Management Pattern
```csharp
public class REPLSession
{
    public string Id { get; init; } = Guid.NewGuid().ToString();
    public DateTime Created { get; init; } = DateTime.UtcNow;
    public DateTime LastAccessed { get; set; } = DateTime.UtcNow;
    public string WorkingDirectory { get; set; } = Environment.CurrentDirectory;
    public List<ConversationTurn> Conversation { get; init; } = new();
    public Dictionary<string, object> Context { get; init; } = new();
    public PermissionSettings Permissions { get; set; } = new();
}
```

## 🔐 Security and Permissions

### Permission System Pattern
```csharp
public record PermissionSettings(
    HashSet<string> AllowedPaths,
    HashSet<string> DeniedPaths,
    HashSet<string> AllowedCommands,
    HashSet<string> DeniedCommands,
    bool AllowNetworkAccess = false,
    bool AllowFileWrite = true,
    bool AllowFileRead = true);

public interface IPermissionService
{
    Task<bool> CanAccessPathAsync(string path, FileSystemPermission permission, CancellationToken cancellationToken = default);
    Task<bool> CanExecuteCommandAsync(string command, CancellationToken cancellationToken = default);
    Task<PermissionSettings> GetPermissionsAsync(string context, CancellationToken cancellationToken = default);
}
```

## 📝 Documentation Standards

### XML Documentation
```csharp
/// <summary>
/// Provides file system operations with permission validation and logging.
/// </summary>
/// <remarks>
/// This service implements the <see cref="IFileSystemService"/> interface and provides
/// secure file system access with comprehensive logging and error handling.
/// </remarks>
public class FileSystemService : IFileSystemService
{
    /// <summary>
    /// Reads the contents of a file asynchronously.
    /// </summary>
    /// <param name="path">The path to the file to read.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>A task that represents the asynchronous read operation. The task result contains the file contents.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="path"/> is null.</exception>
    /// <exception cref="FileNotFoundException">Thrown when the file does not exist.</exception>
    /// <exception cref="UnauthorizedAccessException">Thrown when access to the file is denied.</exception>
    public async Task<string> ReadFileAsync(string path, CancellationToken cancellationToken = default)
    {
        // Implementation
    }
}
```

## 🚀 Performance Guidelines

### Async/Await Best Practices
```csharp
// Use ConfigureAwait(false) for library code
var result = await operation.ExecuteAsync().ConfigureAwait(false);

// Use CancellationToken for all async operations
public async Task<T> ProcessAsync<T>(T input, CancellationToken cancellationToken = default)
{
    cancellationToken.ThrowIfCancellationRequested();
    // Implementation
}

// Use IAsyncEnumerable for streaming operations
public async IAsyncEnumerable<FileInfo> EnumerateFilesAsync(
    string path, 
    [EnumeratorCancellation] CancellationToken cancellationToken = default)
{
    await foreach (var file in GetFilesAsync(path).WithCancellation(cancellationToken))
    {
        yield return file;
    }
}
```

### Memory Management
```csharp
// Use ArrayPool for large arrays
private static readonly ArrayPool<byte> s_arrayPool = ArrayPool<byte>.Shared;

public async Task ProcessLargeDataAsync(Stream stream)
{
    var buffer = s_arrayPool.Rent(4096);
    try
    {
        // Use buffer
    }
    finally
    {
        s_arrayPool.Return(buffer);
    }
}

// Use Span<T> and Memory<T> for efficient memory operations
public void ProcessData(ReadOnlySpan<char> data)
{
    // Efficient string processing without allocations
}
```

## 🎯 AI Integration Patterns

### AI Provider Interface
```csharp
public interface IAIProvider
{
    string Name { get; }
    string Version { get; }
    ProviderCapabilities Capabilities { get; }
    
    Task<AIResponse> SendMessageAsync(AIRequest request, CancellationToken cancellationToken = default);
    IAsyncEnumerable<AIStreamResponse> SendMessageStreamAsync(AIRequest request, CancellationToken cancellationToken = default);
}

public record AIRequest(
    string Message,
    List<AIMessage> History,
    Dictionary<string, object> Parameters,
    string? SystemPrompt = null);

public record AIResponse(
    string Content,
    AIUsage Usage,
    Dictionary<string, object> Metadata);
```

## 📦 Package and Deployment

### Global Tool Configuration
```xml
<PropertyGroup>
    <OutputType>Exe</OutputType>
    <IsPackable>true</IsPackable>
    <PackAsTool>true</PackAsTool>
    <ToolCommandName>claude</ToolCommandName>
    <PackageId>claude</PackageId>
    <PackageTags>ai;cli;claude;anthropic;dotnet-tool</PackageTags>
    <PackageLicenseExpression>MIT</PackageLicenseExpression>
</PropertyGroup>
```

## 🎯 Key Implementation Priorities

1. **Performance First**: Always consider performance implications, use async/await properly
2. **Type Safety**: Leverage C# 12 nullable reference types and pattern matching
3. **Testability**: Design for testability with dependency injection and interfaces
4. **Error Handling**: Comprehensive error handling with proper logging
5. **Documentation**: Complete XML documentation for all public APIs
6. **Security**: Implement proper permission validation and secure credential handling
7. **Cross-Platform**: Ensure compatibility across Windows, macOS, and Linux

## 📝 Git Commit Message Guidelines

When writing git commit messages, be descriptive and comprehensive like Trae AI. Follow these patterns:

### Commit Message Structure
```
<type>(<scope>): <subject>

<body>

<footer>
```

### Commit Types
- **feat**: New feature implementation
- **fix**: Bug fix or issue resolution
- **refactor**: Code refactoring without functional changes
- **perf**: Performance improvements
- **test**: Adding or updating tests
- **docs**: Documentation updates
- **style**: Code style changes (formatting, etc.)
- **chore**: Maintenance tasks, dependency updates
- **ci**: CI/CD pipeline changes
- **build**: Build system or external dependency changes

### Descriptive Examples

**Feature Implementation:**
```
feat(tools): implement ReadFileTool with permission validation

- Add ReadFileTool class implementing ITool interface
- Integrate file system permission checking with IPermissionService
- Include comprehensive error handling for file access scenarios
- Add structured logging for file read operations
- Support cancellation token for async operations
- Include metadata in ToolResult for file size and path information

Resolves: #123
Performance: Optimized for large file reading with streaming
Security: Validates file access permissions before reading
```

**Bug Fix:**
```
fix(repl): resolve session persistence issue in interactive mode

- Fix session state not being saved between REPL interactions
- Correct async/await pattern in SessionManager.SaveSessionAsync
- Add proper exception handling for session serialization failures
- Implement retry logic for transient storage failures
- Update session timestamp on each interaction

Issue: Sessions were lost when CLI was restarted
Root Cause: Missing await in session save operation
Impact: Users can now resume conversations across CLI restarts
Testing: Added integration tests for session persistence scenarios
```

**Refactoring:**
```
refactor(services): extract configuration management into dedicated service

- Move configuration logic from Program.cs to ConfigurationService
- Implement IConfigurationService interface with async methods
- Add multi-layer configuration support (user, project, environment)
- Introduce configuration validation and schema checking
- Update dependency injection registration for new service structure

Benefits:
- Improved separation of concerns
- Better testability with interface abstraction
- Enhanced configuration validation
- Simplified Program.cs startup logic
- Consistent async patterns throughout configuration handling
```

**Performance Improvement:**
```
perf(tools): optimize file enumeration with IAsyncEnumerable

- Replace synchronous file enumeration with async streaming
- Implement IAsyncEnumerable<FileSystemEntry> for large directories
- Add cancellation token support for long-running operations
- Use ArrayPool<byte> for buffer management in file operations
- Optimize memory usage with Span<T> for string operations

Performance Impact:
- 60% reduction in memory usage for large directory scans
- 3x faster enumeration for directories with 10k+ files
- Improved responsiveness with cancellation support
- Reduced GC pressure through buffer pooling

Benchmarks:
- Before: 2.3s, 150MB memory for 50k files
- After: 0.8s, 60MB memory for 50k files
```

### Key Principles for Descriptive Commits

1. **Explain the What and Why**: Don't just state what changed, explain why it was necessary
2. **Include Impact**: Describe how the change affects users, performance, or functionality
3. **Add Context**: Reference issues, performance metrics, or architectural decisions
4. **List Technical Details**: Include specific implementation details for complex changes
5. **Mention Testing**: Describe what tests were added or updated
6. **Note Breaking Changes**: Clearly mark any breaking changes in the footer
7. **Include Metrics**: Add performance numbers, test coverage changes, or other measurable impacts

### Multi-line Body Guidelines

- Use bullet points for multiple changes
- Include "Before/After" scenarios for significant changes
- Add performance metrics when relevant
- Reference related issues or pull requests
- Explain architectural decisions
- Note any breaking changes or migration requirements
- Include testing strategy and coverage information

### Footer Information

- **Resolves**: Link to resolved issues
- **Breaking Change**: Note any breaking changes
- **Performance**: Include performance impact
- **Security**: Note security implications
- **Testing**: Describe test coverage changes
- **Documentation**: Reference updated documentation

This approach ensures commit messages are informative, searchable, and provide valuable context for future developers and code reviews.

## 🔍 Common Patterns to Follow

- Use `Result<T>` pattern for operations that can fail
- Implement proper cancellation token support
- Use structured logging with Serilog
- Follow async/await best practices
- Implement comprehensive error handling
- Use dependency injection for all services
- Write unit tests for all business logic
- Use integration tests for end-to-end scenarios
- Follow SOLID principles
- Implement proper resource disposal with `using` statements

This instruction set will help GitHub Copilot generate code that aligns with the project's architecture, coding standards, and performance requirements while maintaining consistency with the existing codebase.
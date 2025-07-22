# Contributing to claude dotnet

First off, thank you for considering contributing to **claude dotnet**! 🎉

It's people like you who make claude dotnet such a great tool for the .NET community. This document provides guidelines and information for contributing to the project.

## 📋 Table of Contents

- [Code of Conduct](#code-of-conduct)
- [Getting Started](#getting-started)
- [Development Setup](#development-setup)
- [Making Changes](#making-changes)
- [Submitting Changes](#submitting-changes)
- [Code Style](#code-style)
- [Testing](#testing)
- [Documentation](#documentation)
- [Community](#community)

## 🤝 Code of Conduct

This project and everyone participating in it is governed by our Code of Conduct. By participating, you are expected to uphold this code. Please report unacceptable behavior to [opensource@wangkanai.com](mailto:opensource@wangkanai.com).

### Our Pledge

We are committed to making participation in our project a harassment-free experience for everyone, regardless of age, body size, disability, ethnicity, gender identity and expression, level of experience, nationality, personal appearance, race, religion, or sexual identity and orientation.

## 🚀 Getting Started

### Prerequisites

Before you begin, ensure you have the following installed:

- [.NET 9.0 SDK](https://dotnet.microsoft.com/download/dotnet/9.0) (latest version)
- [Git](https://git-scm.com/) for version control
- [Visual Studio 2024](https://visualstudio.microsoft.com/) or [JetBrains Rider](https://www.jetbrains.com/rider/) (recommended)
- A [GitHub account](https://github.com/join)

### Fork and Clone

1. **Fork the repository** on GitHub
2. **Clone your fork** locally:
   ```bash
   git clone https://github.com/YOUR-USERNAME/claude.git
   cd claude
   ```
3. **Add the original repository** as upstream:
   ```bash
   git remote add upstream https://github.com/wangkanai/claude.git
   ```

## 🛠️ Development Setup

### Initial Setup

1. **Restore dependencies**:
   ```bash
   dotnet restore
   ```

2. **Build the solution**:
   ```bash
   dotnet build
   ```

3. **Run tests** to ensure everything works:
   ```bash
   dotnet test
   ```

4. **Install development tools** (optional but recommended):
   ```bash
   # Install EF Core tools
   dotnet tool install --global dotnet-ef
   
   # Install code coverage tools
   dotnet tool install --global dotnet-reportgenerator-globaltool
   ```

### Running the Application

```bash
# Run from source
dotnet run --project src/Claude.Cli -- --help

# Install local build as global tool
dotnet pack src/Claude.Cli/Claude.Cli.csproj -c Release
dotnet tool install -g --add-source ./src/Claude.Cli/bin/Release claude-dotnet

# Test the installed tool
claude --version
```

## 🔧 Making Changes

### Before You Start

1. **Check existing issues** - look for existing issues or discussions related to your contribution
2. **Create an issue** if one doesn't exist to discuss your proposed changes
3. **Get agreement** on the approach before spending significant time on implementation

### Branching Strategy

1. **Create a feature branch** from main:
   ```bash
   git checkout main
   git pull upstream main
   git checkout -b feature/your-feature-name
   ```

2. **Branch naming conventions**:
   - `feature/feature-name` - New features
   - `fix/bug-description` - Bug fixes
   - `docs/documentation-update` - Documentation changes
   - `refactor/component-name` - Code refactoring
   - `test/test-improvement` - Test improvements

### Development Guidelines

#### Code Organization

```
src/
├── Claude.Cli/              # Main CLI application
│   ├── Commands/            # CLI command implementations
│   ├── Services/            # Core business services
│   ├── Tools/               # Tool implementations
│   ├── Configuration/       # Configuration management
│   ├── Extensions/          # Extension methods
│   └── Program.cs           # Application entry point
tests/
├── Claude.Cli.Tests/        # Unit tests
└── Claude.Cli.IntegrationTests/ # Integration tests
```

#### Architectural Principles

- **SOLID Principles** - Follow single responsibility, open/closed, Liskov substitution, interface segregation, and dependency inversion principles
- **Clean Architecture** - Maintain clear separation between UI, business logic, and infrastructure
- **Dependency Injection** - Use Microsoft.Extensions.DependencyInjection for IoC
- **Async/Await** - Use async programming patterns for I/O operations
- **Configuration** - Support multiple configuration sources (files, environment, CLI)

#### Performance Considerations

- **Memory Management** - Be mindful of memory allocation and disposal
- **Async Operations** - Use async/await for I/O bound operations
- **Caching** - Implement appropriate caching strategies
- **Resource Cleanup** - Properly dispose of resources using `using` statements or `IDisposable`

## 📝 Submitting Changes

### Pull Request Process

1. **Update your branch** with the latest changes:
   ```bash
   git checkout main
   git pull upstream main
   git checkout feature/your-feature-name
   git merge main
   ```

2. **Commit your changes** with clear, descriptive messages:
   ```bash
   git add .
   git commit -m "feat: add new analyze command with performance focus"
   ```

3. **Push your branch**:
   ```bash
   git push origin feature/your-feature-name
   ```

4. **Create a Pull Request** on GitHub with:
   - Clear title and description
   - Reference to related issues
   - Screenshots or demos if applicable
   - Checklist completion (see template)

### Commit Message Format

Use [Conventional Commits](https://conventionalcommits.org/) format:

```
type(scope): description

[optional body]

[optional footer(s)]
```

**Types:**
- `feat` - New features
- `fix` - Bug fixes
- `docs` - Documentation changes
- `style` - Code style changes (formatting, etc.)
- `refactor` - Code refactoring
- `test` - Adding or updating tests
- `chore` - Maintenance tasks

**Examples:**
```
feat(cli): add interactive mode with command history
fix(tools): resolve file permission issue on Linux
docs(readme): update installation instructions
test(services): add unit tests for configuration service
```

### Pull Request Checklist

Before submitting your PR, ensure:

- [ ] Code compiles without warnings
- [ ] All tests pass (`dotnet test`)
- [ ] New features have corresponding tests
- [ ] Documentation is updated if needed
- [ ] Code follows project style guidelines
- [ ] Commit messages follow conventional format
- [ ] PR description clearly explains changes
- [ ] Breaking changes are clearly documented

## 🎨 Code Style

### C# Style Guidelines

We follow [Microsoft's C# Coding Conventions](https://docs.microsoft.com/en-us/dotnet/csharp/fundamentals/coding-style/coding-conventions) with some additional rules:

#### Naming Conventions

```csharp
// Interfaces: IPascalCase
public interface IFileSystemService { }

// Classes: PascalCase
public class FileSystemService : IFileSystemService { }

// Methods: PascalCase
public async Task<string> ReadFileAsync(string path) { }

// Properties: PascalCase
public string FileName { get; set; }

// Fields: _camelCase (private), PascalCase (public)
private readonly ILogger _logger;
public static readonly string DefaultPath = "~/.claude";

// Parameters: camelCase
public void ProcessFile(string fileName, bool overwrite = false) { }

// Local variables: camelCase
var configService = serviceProvider.GetRequiredService<IConfigurationService>();
```

#### Code Organization

```csharp
// File header (if needed)
// Copyright notice, etc.

// Using statements (alphabetical, System first)
using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Claude.Cli.Services;

// Namespace
namespace Claude.Cli.Commands;

/// <summary>
/// XML documentation for public types
/// </summary>
public class AnalyzeCommand
{
    // Constants
    private const int DefaultTimeout = 30000;
    
    // Static readonly fields
    private static readonly IReadOnlyList<string> SupportedExtensions = [".cs", ".js", ".ts"];
    
    // Private fields
    private readonly ILogger<AnalyzeCommand> _logger;
    private readonly IFileSystemService _fileService;
    
    // Constructor
    public AnalyzeCommand(ILogger<AnalyzeCommand> logger, IFileSystemService fileService)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _fileService = fileService ?? throw new ArgumentNullException(nameof(fileService));
    }
    
    // Properties
    public string Scope { get; set; } = "project";
    
    // Methods (public first, then private)
    public async Task<int> ExecuteAsync(CancellationToken cancellationToken = default)
    {
        // Implementation
    }
    
    private bool IsValidScope(string scope) => 
        scope is "file" or "module" or "project" or "system";
}
```

### EditorConfig

The project includes a `.editorconfig` file that enforces consistent formatting. Ensure your IDE respects these settings.

## 🧪 Testing

### Testing Strategy

- **Unit Tests** (90%+ coverage target) - Test individual components in isolation
- **Integration Tests** (80%+ coverage target) - Test component interactions
- **End-to-End Tests** - Test complete workflows
- **Performance Tests** - Ensure performance requirements are met

### Writing Tests

#### Unit Test Example

```csharp
public class FileSystemServiceTests
{
    private readonly Mock<IFileSystem> _mockFileSystem;
    private readonly FileSystemService _service;

    public FileSystemServiceTests()
    {
        _mockFileSystem = new Mock<IFileSystem>();
        _service = new FileSystemService(_mockFileSystem.Object);
    }

    [Fact]
    public async Task ReadFileAsync_ExistingFile_ReturnsContent()
    {
        // Arrange
        const string path = "test.txt";
        const string expectedContent = "Hello, World!";
        _mockFileSystem.Setup(fs => fs.File.ReadAllTextAsync(path, It.IsAny<CancellationToken>()))
                      .ReturnsAsync(expectedContent);

        // Act
        var result = await _service.ReadFileAsync(path);

        // Assert
        result.Should().Be(expectedContent);
        _mockFileSystem.Verify(fs => fs.File.ReadAllTextAsync(path, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData(null)]
    public async Task ReadFileAsync_InvalidPath_ThrowsArgumentException(string invalidPath)
    {
        // Act & Assert
        await _service.Invoking(s => s.ReadFileAsync(invalidPath))
                     .Should().ThrowAsync<ArgumentException>();
    }
}
```

#### Integration Test Example

```csharp
public class AnalyzeCommandIntegrationTests : IClassFixture<TestWebApplicationFactory>
{
    private readonly TestWebApplicationFactory _factory;

    public AnalyzeCommandIntegrationTests(TestWebApplicationFactory factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task AnalyzeCommand_WithValidProject_ReturnsSuccessResult()
    {
        // Arrange
        using var client = _factory.CreateClient();
        var tempDir = Path.GetTempPath();
        
        // Create test files
        await File.WriteAllTextAsync(Path.Combine(tempDir, "test.cs"), "public class Test { }");
        
        // Act
        var result = await client.PostAsync("/analyze", new StringContent($"{{\"path\":\"{tempDir}\"}}", 
            Encoding.UTF8, "application/json"));
        
        // Assert
        result.StatusCode.Should().Be(HttpStatusCode.OK);
        var content = await result.Content.ReadAsStringAsync();
        content.Should().Contain("Analysis completed successfully");
    }
}
```

### Running Tests

```bash
# Run all tests
dotnet test

# Run tests with coverage
dotnet test --collect:"XPlat Code Coverage"

# Run specific test project
dotnet test tests/Claude.Cli.Tests

# Run tests with filter
dotnet test --filter "Category=Unit"

# Generate coverage report
reportgenerator -reports:"**/coverage.cobertura.xml" -targetdir:"coverage" -reporttypes:"Html"
```

## 📚 Documentation

### Types of Documentation

1. **Code Documentation** - XML comments for public APIs
2. **README Updates** - Keep README.md current with new features
3. **API Documentation** - Generated from XML comments
4. **Architecture Documentation** - High-level design documents
5. **User Guides** - Step-by-step usage instructions

### XML Documentation

```csharp
/// <summary>
/// Analyzes the specified files or directories for code quality issues.
/// </summary>
/// <param name="scope">The scope of analysis (file, module, project, or system).</param>
/// <param name="focus">The focus area for analysis (performance, security, quality, or architecture).</param>
/// <param name="files">The files to analyze. If empty, analyzes the current directory.</param>
/// <param name="cancellationToken">Cancellation token to cancel the operation.</param>
/// <returns>A task that represents the asynchronous analyze operation. The task result contains the analysis results.</returns>
/// <exception cref="ArgumentException">Thrown when scope or focus contains invalid values.</exception>
/// <exception cref="FileNotFoundException">Thrown when specified files do not exist.</exception>
public async Task<AnalysisResult> AnalyzeAsync(
    string scope = "project",
    string focus = null,
    IEnumerable<string> files = null,
    CancellationToken cancellationToken = default)
{
    // Implementation
}
```

## 💬 Community

### Communication Channels

- **GitHub Issues** - Bug reports and feature requests
- **GitHub Discussions** - General discussions and Q&A
- **Email** - [opensource@wangkanai.com](mailto:opensource@wangkanai.com)

### Getting Help

- Check existing issues and discussions
- Read the documentation thoroughly
- Ask specific, well-formed questions
- Provide context and examples when asking for help

### Reporting Issues

When reporting bugs, please include:

- **Environment details** (OS, .NET version, IDE)
- **Steps to reproduce** the issue
- **Expected behavior** vs actual behavior
- **Error messages** and stack traces
- **Sample code** or project if applicable

### Feature Requests

When requesting features:

- **Describe the problem** you're trying to solve
- **Explain the proposed solution** and alternatives considered
- **Provide use cases** and examples
- **Consider implementation complexity** and maintenance burden

## 🎉 Recognition

Contributors are recognized in several ways:

- **Contributors Section** in README.md
- **Changelog** entries for significant contributions
- **GitHub Releases** acknowledge contributors
- **Social Media** mentions for major contributions

## 📄 License

By contributing to claude dotnet, you agree that your contributions will be licensed under the MIT License. See the [LICENSE](LICENSE) file for details.

---

**Thank you for contributing to claude dotnet!** 🙏

Every contribution, no matter how small, helps make this project better for the entire .NET community.
# PLANNING.md - Claude .NET Project Planning Document

**READ THIS FIRST** - This document provides essential context for every new conversation about the claude dotnet
project.

---

## 🎯 Project Vision & Status

### **Project Identity**

- **Name**: claude dotnet - High-Performance .NET Global Tool
- **Version**: 2.0 (Enhanced Architecture)
- **Technology**: C# 12, .NET 9.0, System.CommandLine, xUnit v3
- **Purpose**: Complete reimplementation of Anthropic's Claude Code CLI as a .NET Global Tool

### **Current Status** ✅

**Enhanced Phase 1: Foundation** (36.4% Complete - 16/45 tasks)

- ✅ Repository setup with comprehensive documentation
- ✅ CLI implementation foundation with System.CommandLine
- ✅ xUnit v3 testing framework setup (80%+ coverage target)
- ✅ Directory.Packages.props centralized package management
- ✅ SESSION-STATE.md contributor workflow template
- ✅ Claude Code CLI official documentation research
- 🚧 **In Progress**: NPM package analysis automation setup

---

## 🏗️ Architecture & Key Decisions

### **Technology Stack** (ENHANCED v2.0)

```yaml
Runtime:            .NET 9.0
Language:           C# 12 with latest language features
CLI Framework:      System.CommandLine v2.0.0-beta4
Hosting:            Microsoft.Extensions.Hosting
DI Container:       Microsoft.Extensions.DependencyInjection
Configuration:      Microsoft.Extensions.Configuration (multi-layer)
Logging:            Microsoft.Extensions.Logging + Serilog
Testing:            xUnit v3 with 80%+ coverage target
Package Management: Directory.Packages.props (centralized)
Build System:       MSBuild with cross-platform publishing
NPM Analysis:       Automated @anthropic-ai/claude-code monitoring
```

### **Project Structure** (ESTABLISHED)

```
claude/
├── src/Claude/                 # Main CLI application
├── tests/UnitTests/            # Unit tests (xUnit v3)
├── tests/IntegrationTests/     # Integration tests
├── benchmark/                  # Performance benchmarks
├── docs/                       # Documentation
├── Directory.Packages.props    # Centralized package management
├── SESSION-STATE.md            # Contributor workflow
├── CLAUDE.md                   # Project guide
├── PRD.md                      # Product Requirements
├── PLANNING.md                 # This file
├── TASKS.md                    # Task management
└── [Standard .NET files]
```

### **Key Architectural Decisions**

1. **Simplified Naming**: "Claude" namespace (not "Claude.Cli")
2. **Global Tool**: Packaged as `claude` with `claude` command
3. **System.CommandLine**: Microsoft's official CLI library with full command mapping
4. **Clean Architecture**: Separation of concerns with DI
5. **Cross-Platform**: Windows, macOS, Linux + ARM64 support with installers
6. **Performance First**: 2-5x faster than Node.js version
7. **NPM Analysis**: Automated package analysis for feature parity
8. **SESSION-STATE.md**: Contributor workflow integration
9. **MCP Integration**: Full MCP server support (Context7, Sequential, Memory, Repomix)
10. **Testing Strategy**: xUnit v3 with 80%+ coverage requirements

---

## 🚀 Development Phases

### **Phase 1: Enhanced Foundation** (CURRENT - Weeks 1-4) - 36.4% Complete

**Priority**: Establish robust foundation with NPM analysis and testing infrastructure

- [x] System.CommandLine integration with official CLI command structure mapping
- [x] Multi-layer configuration management system (appsettings, user, project, environment)
- [x] xUnit v3 testing framework setup with 80%+ coverage targets
- [x] Directory.Packages.props centralized package management
- [x] SESSION-STATE.md contributor workflow integration
- [x] Claude Code CLI documentation research and analysis
- [ ] NPM package analysis automation setup with GitHub Actions
- [ ] Enhanced logging and error handling infrastructure
- [ ] Basic tool interface and registry pattern
- [ ] Core file operations (Read, Write, Edit)

### **Phase 2: NPM Analysis & CLI Mapping** (Weeks 5-7)

**Priority**: Complete feature parity with Node.js implementation

- [ ] Automated @anthropic-ai/claude-code package decompilation and analysis
- [ ] Complete CLI command structure mapping from Node.js to .NET
- [ ] Official documentation integration and validation
- [ ] Command compatibility verification and testing
- [ ] Performance benchmarking against original implementation

### **Phase 3: Enhanced Tool System** (Weeks 8-11)

**Priority**: Complete tool ecosystem with advanced features

- [ ] Complete tool interface and registry with dynamic discovery
- [ ] All core tools implementation (Read, Write, Edit, MultiEdit, Bash, Grep, Glob, etc.)
- [ ] Advanced permission system and access control with audit trails
- [ ] Tool orchestration and chaining with performance optimization
- [ ] Cross-platform compatibility testing and validation

### **Phase 4: AI Integration & MCP Protocol** (Weeks 12-15)

**Priority**: AI provider integration and MCP support

- [ ] Anthropic API client with streaming support and rate limiting
- [ ] Multi-provider architecture (Bedrock, Vertex) with failover
- [ ] OAuth 2.0 authentication flows and secure credential management
- [ ] JSON-RPC protocol implementation with full MCP compliance
- [ ] Plugin loading and discovery system with hot-reload capabilities
- [ ] Core MCP servers integration and testing

### **Phase 5: Advanced Features & Performance** (Weeks 16-18)

**Priority**: Enhanced functionality and optimization

- [ ] Git integration with comprehensive version control operations
- [ ] Web search capabilities and external service integration
- [ ] Session management and persistence with state recovery
- [ ] Performance optimizations and memory usage improvements
- [ ] Advanced debugging and profiling integration

### **Phase 6: Cross-Platform Publishing & Release** (Weeks 19-22)

**Priority**: Production readiness and distribution

- [ ] Cross-platform publishing with single-file executables
- [ ] Installer generation for multiple platforms and formats
- [ ] Comprehensive documentation and security audit
- [ ] Package distribution setup and automated CI/CD pipeline
- [ ] Community engagement and contributor onboarding

---

## 📊 Success Metrics

### **Technical Targets**

- **Performance**: 2-5x faster than Node.js Claude Code
- **Memory**: 30-50% reduction in memory footprint
- **Cold Start**: <500ms from command to first response
- **Test Coverage**: >80% unit, >70% integration (xUnit v3)
- **Quality Gate**: SonarCloud A rating
- **NPM Parity**: 100% feature compatibility with automated validation

### **Business Goals**

- **Downloads**: 10k+ within 6 months
- **Community**: Active contributor ecosystem with SESSION-STATE.md workflow
- **Adoption**: Top 3 AI CLI tools in .NET space
- **Enterprise**: 5+ enterprise customers
- **Cross-Platform**: Support for Windows, macOS, Linux with installers

---

## 🔧 Development Standards

### **Code Quality** (ENFORCED)

- **SOLID Principles**: Single responsibility, dependency inversion
- **Clean Architecture**: Clear layer separation
- **Async/Await**: Comprehensive async programming
- **Error Handling**: Graceful degradation with recovery
- **Security**: Secure credential storage, HTTPS-only
- **Testing**: xUnit v3 with 80%+ coverage requirement

### **Testing Strategy** (ENHANCED)

- **Unit Tests**: xUnit v3 + FluentAssertions (80%+ coverage)
- **Integration Tests**: API clients, file operations, MCP protocol
- **End-to-End Tests**: Complete workflows using Testcontainers
- **Performance Tests**: BenchmarkDotNet with regression detection
- **Security Tests**: SonarCloud and automated vulnerability scanning
- **Compatibility Tests**: Automated NPM package analysis validation

### **Git Workflow** (ESTABLISHED)

- **Commits**: Conventional commits (feat:, fix:, docs:, etc.)
- **Branches**: feature/*, fix/*, docs/* patterns
- **PRs**: Required for all changes with review
- **Tags**: Semantic versioning (v1.0.0, v1.1.0, etc.)
- **Automation**: GitHub Actions for CI/CD and NPM analysis

---

## 🎯 Current Context & Next Steps

### **Immediate Priorities** (Enhanced Phase 1)

1. **NPM Analysis Setup**: GitHub Actions automation for package monitoring
2. **Test Infrastructure**: Complete xUnit v3 setup with coverage reporting
3. **Package Management**: Finalize Directory.Packages.props configuration
4. **Documentation**: Update all docs to v2.0 enhanced architecture
5. **Tool Foundation**: Implement core file operation tools

### **Technical Debt to Avoid**

- ❌ Blocking synchronous operations
- ❌ Hard-coded file paths or configurations
- ❌ Missing cancellation token support
- ❌ Poor error handling or silent failures
- ❌ Tight coupling between layers
- ❌ Inadequate test coverage (<80%)
- ❌ Manual NPM compatibility checking

### **Key Resources**

- **Project Guide**: [`CLAUDE.md`](CLAUDE.md) - Complete technical specification
- **PRD**: [`PRD.md`](PRD.md) - Product requirements document
- **Tasks**: [`TASKS.md`](TASKS.md) - Active task tracking
- **Session State**: [`SESSION-STATE.md`](SESSION-STATE.md) - Contributor workflow
- **Usage Modes Planning**: [`docs/USAGE-MODES-PLANNING.md`](docs/USAGE-MODES-PLANNING.md) - Comprehensive usage modes analysis
- **Resource Allocation**: [`docs/RESOURCE-ALLOCATION.md`](docs/RESOURCE-ALLOCATION.md) - Detailed resource planning and timeline
- **Implementation Architecture**: [`docs/IMPLEMENTATION-ARCHITECTURE.md`](docs/IMPLEMENTATION-ARCHITECTURE.md) - Technical implementation methods
- **Repository**: https://github.com/wangkanai/claude

---

## 🔄 Project Continuity

### **For New Conversations**

1. **Read PLANNING.md** (this file) for full context
2. **Check TASKS.md** for current work status
3. **Review CLAUDE.md** for technical details
4. **Check SESSION-STATE.md** for contributor context
5. **Understand current phase** and priorities
6. **Follow established patterns** and standards

### **Context Preservation**

- **Always update TASKS.md** when work is completed
- **Add new tasks** as they're discovered
- **Document key decisions** in this file
- **Maintain git commit quality** for history
- **Update SESSION-STATE.md** for contributor handoffs
- **Track NPM analysis results** for feature parity

---

**Document Version**: 2.0 (Enhanced Architecture)
**Created**: 2025-07-22
**Last Updated**: 2025-07-23
**Current Phase**: Enhanced Phase 1 (Foundation) - 36.4% Complete
**Repository Status**: Active Development
**Next Milestone**: NPM Analysis Automation & Core Tools

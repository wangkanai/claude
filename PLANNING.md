# PLANNING.md - Claude .NET Project Planning Document

**READ THIS FIRST** - This document provides essential context for every new conversation about the claude dotnet
project.

---

## 🎯 Project Vision & Status

### **Project Identity**

- **Name**: claude dotnet - High-Performance .NET Global Tool
- **Version**: 1.0 (Development Phase)
- **Technology**: C# 12, .NET 9.0, System.CommandLine
- **Purpose**: Complete reimplementation of Anthropic's Claude Code CLI as a .NET Global Tool

### **Current Status** ✅

**Repository Setup Complete** (28/28 tasks completed - 2025-07-22)

- ✅ Repository boilerplate with comprehensive documentation
- ✅ Project structure with simplified "Claude" naming convention
- ✅ CI/CD pipeline and development standards established
- ✅ Git workflow with semantic commit conventions
- ✅ **Ready for Phase 1 Development**

---

## 🏗️ Architecture & Key Decisions

### **Technology Stack** (LOCKED)

```yaml
Runtime:       .NET 9.0
Language:      C# 12 with latest language features
CLI Framework: System.CommandLine
Hosting:       Microsoft.Extensions.Hosting
DI Container:  Microsoft.Extensions.DependencyInjection
Configuration: Microsoft.Extensions.Configuration
Logging:       Microsoft.Extensions.Logging (Serilog)
Testing:       xUnit, FluentAssertions, Testcontainers
```

### **Project Structure** (ESTABLISHED)

```
claude/
├── src/Claude/                 # Main CLI application
├── tests/Claude.Tests/         # Unit tests
├── tests/Claude.IntegrationTests/ # Integration tests
├── CLAUDE.md                   # Project guide
├── PRD.md                      # Product Requirements
├── PLANNING.md                 # This file
├── TASKS.md                    # Task management
└── [Standard .NET files]
```

### **Key Architectural Decisions**

1. **Simplified Naming**: "Claude" namespace (not "Claude.Cli")
2. **Global Tool**: Packaged as `claude-dotnet` with `claude` command
3. **System.CommandLine**: Microsoft's official CLI library
4. **Clean Architecture**: Separation of concerns with DI
5. **Cross-Platform**: Windows, macOS, Linux + ARM64 support
6. **Performance First**: 2-5x faster than Node.js version

---

## 🚀 Development Phases

### **Phase 1: Foundation** (CURRENT - Weeks 1-3)

**Priority**: Get basic CLI working with core file operations

- [ ] System.CommandLine integration with basic commands
- [ ] Configuration management system (appsettings, user, project)
- [ ] Core file operations (Read, Write, Edit)
- [ ] Logging and error handling infrastructure
- [ ] Basic tool interface and registry pattern

### **Phase 2: Tool System** (Weeks 4-7)

**Priority**: Complete tool ecosystem

- [ ] All core tools (Bash, Grep, Glob, TodoWrite, etc.)
- [ ] Permission system and access control
- [ ] Tool orchestration and chaining
- [ ] File system abstraction layer

### **Phase 3: AI Integration** (Weeks 8-10)

**Priority**: Anthropic API integration

- [ ] HTTP client with streaming support
- [ ] Multi-provider architecture setup
- [ ] OAuth 2.0 authentication flows
- [ ] Rate limiting and quota management

### **Phase 4: MCP Protocol** (Weeks 11-14)

**Priority**: Model Context Protocol support

- [ ] JSON-RPC protocol implementation
- [ ] Plugin loading and discovery system
- [ ] Core MCP servers integration

### **Phase 5: Advanced Features** (Weeks 15-17)

**Priority**: Enhanced functionality

- [ ] Git integration capabilities
- [ ] Web search and fetch tools
- [ ] Session management and persistence

### **Phase 6: Release Preparation** (Weeks 18-20)

**Priority**: Production readiness

- [ ] Comprehensive documentation
- [ ] Security audit and hardening
- [ ] Package distribution setup
- [ ] CI/CD pipeline completion

---

## 📊 Success Metrics

### **Technical Targets**

- **Performance**: 2-5x faster than Node.js Claude Code
- **Memory**: 30-50% reduction in memory footprint
- **Cold Start**: <500ms from command to first response
- **Test Coverage**: >90% unit, >80% integration
- **Quality Gate**: SonarCloud A rating

### **Business Goals**

- **Downloads**: 10k+ within 6 months
- **Community**: Active contributor ecosystem
- **Adoption**: Top 3 AI CLI tools in .NET space
- **Enterprise**: 5+ enterprise customers

---

## 🔧 Development Standards

### **Code Quality** (ENFORCED)

- **SOLID Principles**: Single responsibility, dependency inversion
- **Clean Architecture**: Clear layer separation
- **Async/Await**: Comprehensive async programming
- **Error Handling**: Graceful degradation with recovery
- **Security**: Secure credential storage, HTTPS-only

### **Testing Strategy** (REQUIRED)

- **Unit Tests**: xUnit + FluentAssertions (90%+ coverage)
- **Integration Tests**: Real API and file system testing
- **Performance Tests**: BenchmarkDotNet benchmarks
- **Security Tests**: Static analysis integration

### **Git Workflow** (ESTABLISHED)

- **Commits**: Conventional commits (feat:, fix:, docs:, etc.)
- **Branches**: feature/*, fix/*, docs/* patterns
- **PRs**: Required for all changes with review
- **Tags**: Semantic versioning (v1.0.0, v1.1.0, etc.)

---

## 🎯 Current Context & Next Steps

### **Immediate Priorities** (Phase 1 Start)

1. **System.CommandLine Setup**: Basic command structure and parsing
2. **DI Container**: Host builder with service registration
3. **File Operations**: Read, Write, Edit tool implementations
4. **Configuration**: Multi-layer config system
5. **Logging**: Structured logging with Serilog

### **Technical Debt to Avoid**

- ❌ Blocking synchronous operations
- ❌ Hard-coded file paths or configurations
- ❌ Missing cancellation token support
- ❌ Poor error handling or silent failures
- ❌ Tight coupling between layers

### **Key Resources**

- **Project Guide**: [`CLAUDE.md`](CLAUDE.md) - Complete technical specification
- **PRD**: [`PRD.md`](PRD.md) - Product requirements document
- **Tasks**: [`TASKS.md`](TASKS.md) - Active task tracking
- **Repository**: GitHub will be configured in Phase 1

---

## 🔄 Project Continuity

### **For New Conversations**

1. **Read PLANNING.md** (this file) for full context
2. **Check TASKS.md** for current work status
3. **Review CLAUDE.md** for technical details
4. **Understand current phase** and priorities
5. **Follow established patterns** and standards

### **Context Preservation**

- **Always update TASKS.md** when work is completed
- **Add new tasks** as they're discovered
- **Document key decisions** in this file
- **Maintain git commit quality** for history
- **Update status** as phases progress

---

**Document Version**: 1.0
**Created**: 2025-07-22
**Last Updated**: 2025-07-22
**Current Phase**: Phase 1 (Foundation)
**Repository Status**: Setup Complete, Development Ready
**Next Milestone**: Basic CLI + File Operations

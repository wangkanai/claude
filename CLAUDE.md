# CLAUDE.md - claude dotnet Project Guide

**Project**: claude dotnet - High-Performance .NET Global Tool
**Version**: 2.0 (Enhanced Architecture)
**Technology**: C# 12, .NET 9.0, System.CommandLine, xUnit v3

---

## 📋 Project Overview

**claude dotnet** is a complete reimplementation of Anthropic's Claude Code CLI as a .NET Global Tool, incorporating comprehensive research of the official CLI, automated NPM package analysis, and enterprise-grade architecture.

### **Vision**

Create the most performant, reliable, and developer-friendly AI-powered CLI tool for software development, leveraging .NET ecosystem while maintaining 100% feature parity with Node.js implementation.

### **Key Value Propositions**

-   **2-5x Performance** through compiled .NET code
-   **30-50% Lower Memory** with optimized GC
-   **100% Feature Parity** verified through NPM analysis
-   **Enhanced Type Safety** using C# 12 features
-   **Self-Contained** without Node.js dependency
-   **Superior DX** with advanced debugging/profiling

### **Success Metrics**

-   Performance: <500ms cold start, <200ms operations
-   Memory: <100MB baseline, <500MB under load
-   Quality: >80% test coverage, SonarCloud A rating
-   Adoption: 10k+ downloads within 6 months

---

## 🏗️ Architecture Overview

### **Core Technology Stack**

```yaml
Runtime: .NET 9.0
Language: C# 12
CLI Framework: System.CommandLine v2.0.0-beta4
Hosting: Microsoft.Extensions.Hosting
Testing: xUnit v3 (80%+ coverage)
Package Mgmt: Directory.Packages.props
NPM Analysis: Automated @anthropic-ai/claude-code monitoring
```

### **System Architecture**

```
┌─────────────────────────────────────────────────────────────┐
│                    claude dotnet CLI v2.0                   │
├─────────────────────────────────────────────────────────────┤
│ Command Layer    │ System.CommandLine + CLI Mapping         │
│ NPM Analysis     │ Automated Package Analysis & Detection   │
│ Tool Layer       │ Strategy Pattern + Tool Implementation   │
│ MCP Layer        │ JSON-RPC Protocol + Server Discovery     │
│ AI Provider      │ Multi-Provider (Anthropic/Bedrock/Vertex)│
│ Configuration    │ Multi-Layer Config System                │
│ File System      │ System.IO.Abstractions + Permissions     │
│ Testing          │ xUnit v3 + Testcontainers                │
│ Infrastructure   │ Cross-Platform + Single-File Publishing  │
└─────────────────────────────────────────────────────────────┘
```

---

## 🎯 Development Phases

### **Phase 1: Foundation (Weeks 1-4)** - Current

-   Directory.Packages.props centralized management
-   System.CommandLine integration
-   Multi-layer configuration system
-   xUnit v3 testing framework
-   NPM analysis automation setup
-   SESSION-STATE.md workflow

### **Phase 2: NPM Analysis (Weeks 5-7)**

-   Automated package decompilation
-   CLI command structure mapping
-   Compatibility verification
-   Performance benchmarking

### **Phase 3: Tool System (Weeks 8-11)**

-   Complete tool interface and registry
-   Core tools implementation
-   Permission system with audit
-   Cross-platform testing

### **Phase 4: AI Integration (Weeks 12-15)**

-   Anthropic API client with streaming
-   Multi-provider architecture
-   OAuth 2.0 authentication
-   JSON-RPC protocol implementation

### **Phase 5: Advanced Features (Weeks 16-18)**

-   Git integration
-   Web search capabilities
-   Session management
-   Performance optimizations

### **Phase 6: Publishing (Weeks 19-22)**

-   Cross-platform executables
-   Installer generation
-   Security audit
-   Community engagement

---

## 🛡️ Development Guidelines

### **Code Quality Standards**

-   Test Coverage: >80% unit, >70% integration
-   Performance: <500ms cold start, <2s complex ops
-   Memory: <100MB baseline, <500MB heavy load
-   Compatibility: 100% NPM feature parity
-   Security: Secure storage, HTTPS-only, audit trails

### **Best Practices**

-   SOLID Principles & Clean Architecture
-   Async/Await for all I/O operations
-   Multi-layer configuration
-   Cross-platform support (Windows/macOS/Linux)
-   Centralized package management
-   SESSION-STATE.md contributor workflow

---

## 🖼️ Image Processing Configuration

Optimized for Claude's efficiency:

-   **Supported**: SVG (vector graphics with text)
-   **Excluded**: PNG/JPG/JPEG/GIF/WEBP/BMP/TIFF/ICO (raster formats)
-   **Rationale**: Prioritize computational efficiency

---

## 🔧 Core Features

### **1. CLI Interface**

```csharp
[Command("analyze")]
public class AnalyzeCommand : ICommand
{
    [Option("--scope")] public string Scope { get; set; } = "project";
    [Option("--focus")] public string Focus { get; set; }
    [Option("--think")] public bool EnableDeepAnalysis { get; set; }
}
```

Features: Natural language processing, context awareness, slash commands support, NPM compatibility verification.

### **2. Tool System**

```csharp
public interface ITool
{
    string Name { get; }
    Task<ToolResult> ExecuteAsync(ToolRequest request, CancellationToken ct);
}
```

Core Tools: Read, Write, Edit, MultiEdit, Bash, Grep, Glob, TodoWrite, WebFetch, NpmAnalyze.

### **3. MCP Integration**

JSON-RPC protocol, MEF-based plugins, dynamic server discovery, graceful fallback handling.

### **4. AI Providers**

Multi-provider support (Anthropic, Bedrock, Vertex), OAuth 2.0, rate limiting, usage tracking.

### **5. File System**

Cross-platform handling, real-time change detection, permission validation, symbolic link support.

---

## 🛠️ MCP Workflow Integration

### **Essential MCP Servers**

**Tier 1 - Daily Use**:

-   **JetBrains**: IDE integration

-   **Sequential**: Architectural analysis
-   **Memory**: Decision tracking
-   **Repomix**: Codebase analysis

**Tier 2 - Feature Development**:

-   **Magic**: UI component generation
-   **Context7**: .NET library documentation

**Tier 3 - Testing**:

-   **Playwright**: E2E testing
-   **BrowserLoop**: Visual testing
-   **Fetch**: External docs

### **Quick Commands**

```bash
# Research patterns
mcp__context7__resolve-library-id --query "System.CommandLine"
mcp__context7__get-library-docs --library-id "system-commandline"

# Analyze problems
mcp__sequential-thinking__sequentialthinking --thought "Analysis" --totalThoughts 5

# Track decisions
mcp__memory__create_entities --entities '[{"name": "Decision"}]'

# Analyze code
mcp__repomix__pack_codebase --directory "." --compress true
```

---

## 🔒 Security & Performance

### **Security**

-   OS credential managers
-   HTTPS-only communication
-   Sandboxed code execution
-   Comprehensive audit trails
-   GDPR compliance

### **Performance Targets**

-   Cold Start: <500ms
-   Memory: <100MB baseline
-   Response: <200ms simple ops
-   Throughput: 100+ concurrent files
-   Scalability: 100k+ files, 1GB+ codebases

---

## 🚀 Installation & Usage

### **Installation**

```bash
# Global tool
dotnet tool install -g claude

# Platform-specific
winget install wangkanai.claude  # Windows
brew install claude               # macOS
curl -sSL https://github.com/wangkanai/claude/releases/latest/download/claude-linux-x64 -o claude
```

### **Usage**

```bash
# Natural language
claude "Analyze this code for performance issues"

# Commands
claude analyze --scope project --focus performance
claude implement "Add JWT authentication" --framework dotnet

# Configuration
claude config set api-key "key" --provider anthropic
```

---

## 📅 Development Progress

### **Current Status**: Phase 1 - 36.4% Complete (16/45 tasks)

**Completed**:

-   ✅ Repository setup with .NET Global Tool config
-   ✅ System.CommandLine integration
-   ✅ Dependency injection infrastructure
-   ✅ Multi-command support
-   ✅ NPM analysis design
-   ✅ xUnit v3 setup
-   ✅ Directory.Packages.props
-   ✅ SESSION-STATE.md workflow

**Next Priorities**:

1. NPM Analysis GitHub Actions
2. ITool interface implementation
3. Core file operations
4. Multi-layer configuration
5. Serilog logging

---

## 📚 Resources

### **Documentation**

-   API Documentation (100% XML coverage)
-   Architecture Decision Records
-   User/Developer Guides
-   NPM Analysis Reports
-   Performance Benchmarks

### **Community**

-   GitHub Repository & Issues
-   NuGet Package Distribution
-   Docker Hub Images
-   Discord/Discussions
-   Plugin Ecosystem

---

## 🤝 Contributing

-   Git flow with feature branches
-   Mandatory code review
-   Automated testing & quality gates
-   Semantic versioning
-   SESSION-STATE.md tracking

---

**Version**: 2.0 | **Updated**: 2025-07-23 | **Status**: Active Development  
**Repository**: https://github.com/wangkanai/claude  
**Phase**: Enhanced Phase 1 (36.4% Complete)  
**Next**: NPM Analysis Automation & Core Tool Implementation


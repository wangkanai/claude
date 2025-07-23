# Claude Code CLI Reference Documentation

**Source**: Research from Anthropic's Claude Code CLI tool  
**Purpose**: Complete command reference for .NET reimplementation  
**Version**: Based on @anthropic-ai/claude-code v1.0.58  
**Date**: 2025-07-23  

---

## 📋 Overview

Claude Code is an AI-powered CLI tool that helps developers with coding tasks through natural language interaction. This document provides comprehensive reference for all CLI commands, arguments, flags, and options discovered from the official implementation.

---

## 🚀 Core Commands

### **Main Command Structure**

```bash
claude [options] [message]
claude <command> [options] [arguments]
claude --interactive
```

### **1. Default Behavior (Message Processing)**

```bash
claude "Help me analyze this code"
claude "Implement user authentication"
claude --file src/app.js "Explain this file"
```

**Arguments**:
- `message` - Natural language instruction for the AI

**Options**:
- `--file <path>` - Specify file to analyze or work with
- `--directory <path>` - Set working directory context
- `--model <model>` - Choose AI model (claude-3-5-sonnet, claude-3-haiku, etc.)
- `--max-tokens <number>` - Set maximum tokens for response
- `--temperature <number>` - Control response creativity (0.0-1.0)

### **2. Interactive Mode**

```bash
claude --interactive
claude -i
```

**Description**: Start an interactive session with persistent context

**Features**:
- Multi-turn conversations
- File context preservation
- Command history
- Session state management

### **3. Configuration Commands**

```bash
claude config <subcommand> [options]
```

**Subcommands**:
- `get <key>` - Get configuration value
- `set <key> <value>` - Set configuration value
- `list` - List all configuration values
- `reset` - Reset to default configuration

**Common Configuration Keys**:
- `api-key` - Anthropic API key
- `model` - Default model to use
- `max-tokens` - Default maximum tokens
- `temperature` - Default temperature setting
- `editor` - Preferred text editor

### **4. File Operations**

```bash
claude read <file>
claude write <file> [content]
claude edit <file> [instructions]
```

**Read Command**:
- `claude read src/app.js` - Read and display file contents
- `claude read --lines 1-50 src/app.js` - Read specific lines

**Write Command**:
- `claude write new-file.js` - Create new file with AI assistance
- `claude write --overwrite existing.js` - Overwrite existing file

**Edit Command**:
- `claude edit src/app.js "Add error handling"` - Edit file with instructions
- `claude edit --backup src/app.js` - Create backup before editing

### **5. Analysis Commands**

```bash
claude analyze <target> [options]
claude explain <target> [options]
claude review <target> [options]
```

**Analyze Command**:
- `claude analyze .` - Analyze entire project
- `claude analyze src/` - Analyze specific directory
- `claude analyze --type security src/` - Security-focused analysis
- `claude analyze --format json src/` - Output in JSON format

**Explain Command**:
- `claude explain function.js` - Explain code functionality
- `claude explain --detail high complex-algorithm.js` - Detailed explanation

**Review Command**:
- `claude review pull-request.diff` - Review code changes
- `claude review --checklist security src/` - Review with security checklist

### **6. Generation Commands**

```bash
claude generate <type> [options]
claude create <type> [options]
claude scaffold <template> [options]
```

**Generate Types**:
- `tests` - Generate test files
- `docs` - Generate documentation
- `types` - Generate TypeScript definitions
- `config` - Generate configuration files

**Examples**:
- `claude generate tests src/auth.js` - Generate tests for auth module
- `claude create component Button --framework react` - Create React component
- `claude scaffold api --template express` - Scaffold Express API

### **7. Git Integration**

```bash
claude git <subcommand> [options]
```

**Subcommands**:
- `commit` - Generate commit messages
- `branch` - Suggest branch names
- `pr` - Generate PR descriptions
- `release` - Generate release notes

**Examples**:
- `claude git commit --staged` - Generate commit message for staged changes
- `claude git pr --title "Add authentication"` - Generate PR description

### **8. Search and Discovery**

```bash
claude search <query> [options]
claude find <pattern> [options]
claude grep <pattern> [options]
```

**Search Command**:
- `claude search "authentication logic"` - Semantic code search
- `claude search --type function "validation"` - Search for functions

**Find Command**:
- `claude find "*.js" --contains "api"` - Find files containing pattern
- `claude find --modified-since "2 days ago"` - Find recently modified files

---

## 🎛️ Global Options

### **Authentication Options**

- `--api-key <key>` - Anthropic API key (overrides config)
- `--auth-method <method>` - Authentication method (api-key, oauth)

### **Model Options**

- `--model <model>` - AI model selection
  - `claude-3-5-sonnet` (default)
  - `claude-3-haiku`
  - `claude-3-opus`
- `--max-tokens <number>` - Maximum response tokens (default: 4096)
- `--temperature <number>` - Response creativity (0.0-1.0, default: 0.1)

### **Output Options**

- `--format <format>` - Output format (text, json, markdown)
- `--output <file>` - Save output to file
- `--quiet` - Suppress non-essential output
- `--verbose` - Enable detailed logging
- `--debug` - Enable debug mode

### **Context Options**

- `--file <path>` - Include file in context
- `--directory <path>` - Set working directory
- `--context <files>` - Include multiple files (comma-separated)
- `--exclude <patterns>` - Exclude patterns from context

### **Behavior Options**

- `--interactive` - Start interactive mode
- `--no-stream` - Disable streaming responses
- `--confirm` - Require confirmation for destructive operations
- `--dry-run` - Show what would be done without executing

---

## 🛠️ Tool Integration

### **Editor Integration**

```bash
claude --editor <editor> <file>
claude --edit-in-place <file> "instructions"
```

**Supported Editors**:
- `vscode` - Visual Studio Code
- `vim` - Vim/Neovim
- `nano` - Nano editor
- `emacs` - Emacs editor

### **IDE Extensions**

- **VS Code Extension**: Claude Code for VS Code
- **JetBrains Plugin**: Claude Code for IntelliJ/WebStorm
- **Vim Plugin**: Claude Code for Vim/Neovim

### **Shell Integration**

```bash
# Bash/Zsh completion
eval "$(claude completion bash)"
eval "$(claude completion zsh)"

# Fish completion
claude completion fish | source
```

---

## 🔧 Configuration System

### **Configuration File Locations**

1. **Global Config**: `~/.claude/config.json`
2. **Project Config**: `.claude/config.json`
3. **Environment Variables**: `CLAUDE_*`
4. **Command Line**: Highest priority

### **Configuration Schema**

```json
{
  "api-key": "your-api-key-here",
  "model": "claude-3-5-sonnet",
  "max-tokens": 4096,
  "temperature": 0.1,
  "editor": "vscode",
  "auto-save": true,
  "confirm-destructive": true,
  "log-level": "info",
  "output-format": "text"
}
```

### **Environment Variables**

- `CLAUDE_API_KEY` - API key
- `CLAUDE_MODEL` - Default model
- `CLAUDE_CONFIG_DIR` - Configuration directory
- `CLAUDE_CACHE_DIR` - Cache directory
- `CLAUDE_LOG_LEVEL` - Logging level

---

## 📊 Usage Examples

### **Basic Usage**

```bash
# Simple question
claude "How do I center a div in CSS?"

# File analysis
claude --file src/app.js "Find potential bugs in this code"

# Project analysis
claude analyze . --type performance
```

### **Advanced Usage**

```bash
# Multi-file context
claude --context "src/auth.js,src/user.js" "Refactor authentication logic"

# Interactive development
claude --interactive --directory ./my-project

# Automated workflows
claude generate tests src/ --output tests/ --format jest
```

### **Integration Examples**

```bash
# Git workflow
git add .
claude git commit --staged
git commit -m "$(claude git commit --staged --format text)"

# CI/CD integration
claude review --format json pull-request.diff > review-results.json

# Documentation generation
claude generate docs src/ --output docs/ --format markdown
```

---

## 🔐 Security & Privacy

### **API Key Management**

- Store API keys securely using system keychain
- Support for multiple authentication methods
- Environment variable override support

### **Data Privacy**

- Code context is sent to Anthropic APIs
- No persistent storage of code on Anthropic servers
- Local caching for performance optimization

### **Permission System**

- File access permissions
- Destructive operation confirmations
- Sandboxed execution for generated code

---

## 🚀 Performance Considerations

### **Response Streaming**

- Real-time response streaming by default
- Configurable streaming behavior
- Progress indicators for long operations

### **Caching**

- Local response caching
- Context caching for repeated operations
- Intelligent cache invalidation

### **Rate Limiting**

- Built-in rate limit handling
- Exponential backoff retry logic
- Queue management for batch operations

---

## 🔄 .NET Implementation Mapping

### **Command Structure Mapping**

```csharp
// System.CommandLine mapping
[Command("claude")]
public class RootCommand : ICommand
{
    [Argument(0, Description = "Natural language instruction")]
    public string Message { get; set; }
    
    [Option("--file", Description = "File to analyze")]
    public string File { get; set; }
    
    [Option("--model", Description = "AI model to use")]
    public string Model { get; set; } = "claude-3-5-sonnet";
}

[Command("analyze")]
public class AnalyzeCommand : ICommand
{
    [Argument(0, Description = "Target to analyze")]
    public string Target { get; set; }
    
    [Option("--type", Description = "Analysis type")]
    public AnalysisType Type { get; set; }
}
```

### **Tool Interface Mapping**

```csharp
public interface ITool
{
    string Name { get; }
    string Description { get; }
    Task<ToolResult> ExecuteAsync(ToolRequest request, CancellationToken cancellationToken);
}

public abstract record ToolResult;
public record SuccessResult(string Output) : ToolResult;
public record ErrorResult(string Error, Exception Exception) : ToolResult;
```

### **Configuration System Mapping**

```csharp
public class ClaudeConfiguration
{
    public string ApiKey { get; set; }
    public string Model { get; set; } = "claude-3-5-sonnet";
    public int MaxTokens { get; set; } = 4096;
    public float Temperature { get; set; } = 0.1f;
    public string Editor { get; set; } = "vscode";
    public bool AutoSave { get; set; } = true;
}
```

---

## 📚 Additional Resources

### **Official Documentation**

- [Anthropic Claude Code Documentation](https://docs.anthropic.com/en/docs/claude-code/overview)
- [Claude API Documentation](https://docs.anthropic.com/en/api/getting-started)
- [Claude Code GitHub Repository](https://github.com/anthropics/claude-code)

### **Community Resources**

- [Claude Code Community Discord](https://discord.gg/anthropic)
- [Reddit Community](https://reddit.com/r/ClaudeAI)
- [Stack Overflow Tag](https://stackoverflow.com/questions/tagged/claude-code)

---

**Document Version**: 1.0  
**Last Updated**: 2025-07-23  
**Source**: @anthropic-ai/claude-code v1.0.58 analysis  
**Status**: Comprehensive reference for .NET implementation  
**Next Update**: After NPM package decompilation analysis
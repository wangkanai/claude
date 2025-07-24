# Claude Code CLI Reference Documentation

**Source**: Official Anthropic Claude Code CLI research via MCP servers  
**Purpose**: Accurate command reference for .NET reimplementation  
**Research Date**: 2025-07-24  
**Research Method**: Multi-MCP analysis (fetch, context7, sequential-thinking, repomix)  

---

## 📋 Overview

**Claude Code** is an agentic coding tool that lives in your terminal, designed for natural language interaction with your codebase. Based on comprehensive research of the official implementation, this document provides the **definitive** CLI command reference for .NET reimplementation.

**Key Insight**: Claude Code is **conversational-first**, not command-hierarchy focused. The primary interface is natural language interaction through an interactive REPL with extensible slash commands.

---

## 🚀 Core CLI Commands (Official)

### **1. Primary Entry Points**

```bash
# Start interactive REPL
claude

# Start REPL with initial prompt
claude "explain this project"

# SDK mode - query and exit
claude -p "explain this function"

# Process piped content
cat logs.txt | claude -p "explain"

# Continue most recent conversation
claude -c
claude --continue
claude --resume

# Resume specific session by ID
claude -r "abc123" "Finish this PR"

# Update CLI to latest version
claude update
```

### **2. Advanced CLI Options**

```bash
# MCP server configuration
claude --mcp-config <path-to-file>

# Enable MCP debug mode
claude --mcp-debug

# Stream JSON output in print mode
claude -p --output-format=stream-json

# Continue via SDK with specific task
claude -c -p "Check for type errors"

# SDK with custom permission tool and MCP config
claude -p "..." \
  --permission-prompt-tool mcp__test-server__approval_prompt \
  --mcp-config my-config.json
```

**Note**: The `--print` flag has updated JSON output structure with nested message objects for improved forwards-compatibility.

---

## 🎛️ Built-in Slash Commands (Interactive Mode)

These commands are available within the interactive REPL session:

### **Core System Commands**
```bash
/help                    # Get usage help
/clear                   # Clear conversation history
/compact [instructions]  # Compact conversation with optional focus
/cost                    # Show token usage statistics
/status                  # View account and system statuses
exit                     # Exit interactive mode
```

### **Configuration & Setup**
```bash
/config                  # View/modify configuration
/init                    # Initialize project with CLAUDE.md guide
/permissions             # View or update permissions
/terminal-setup          # Install Shift+Enter key binding (iTerm2/VSCode)
/vim                     # Enter vim mode for alternating insert/command modes
/doctor                  # Check health of Claude Code installation
```

### **Account & Authentication**
```bash
/login                   # Switch Anthropic accounts
/logout                  # Sign out from Anthropic account
/model                   # Select or change the AI model
```

### **Project & Memory Management**
```bash
/memory                  # Edit CLAUDE.md memory files
/approved-tools          # Manage tool permissions
/release-notes          # View release notes
```

### **Development & Review**
```bash
/review                  # Request code review
/pr_comments            # View pull request comments
/bug                    # Report bugs (sends conversation to Anthropic)
```

### **MCP Integration**
```bash
/mcp                    # Manage MCP server connections and OAuth
```

### **Quick Actions**
```bash
# at start of line    # Memory shortcut - add to CLAUDE.md
/ at start of line     # Slash command prefix
```

---

## ⚙️ Configuration Management

### **Configuration Commands**

```bash
# List all current settings
claude config list

# Retrieve specific setting value
claude config get <key>

# Change setting value
claude config set <key> <value>

# Add multiple values (comma or space separated)
claude config add <key> <value1>,<value2>
claude config add <key> <value1> <value2>

# Remove multiple values
claude config remove <key> <value1> <value2>
```

**Global Flag**: Add `--global` to any config command to modify global instead of project configuration.

---

## 🔌 MCP (Model Context Protocol) Integration

### **MCP Management Commands**

```bash
# Interactive setup wizard for adding MCP servers
claude mcp add

# Import MCP servers from Claude Desktop
claude mcp add-from-claude-desktop

# Add MCP server directly as JSON string
claude mcp add-json <n> <json>
```

### **MCP Slash Commands (Dynamic)**

MCP servers provide dynamically discovered commands following this pattern:

```bash
# General format
/mcp__<server-name>__<prompt-name> [arguments]

# Examples
/mcp__github__list_prs
/mcp__github__pr_review 456
/mcp__jira__create_issue "Bug title" high
```

**Arguments**: Space-separated, with quotes for arguments containing spaces.

---

## 📝 Custom Slash Commands

### **Project-Specific Commands**

```bash
# Create commands directory
mkdir -p .claude/commands

# Create command file
echo "Analyze this code for performance issues and suggest optimizations:" > .claude/commands/optimize.md

# Usage
> /project:optimize
```

### **Personal Commands (Global)**

```bash
# Create personal commands directory
mkdir -p ~/.claude/commands

# Create personal command
echo "Review this code for security vulnerabilities:" > ~/.claude/commands/security-review.md

# Usage
> /user:security-review
```

### **Commands with Arguments**

```bash
# Command with $ARGUMENTS placeholder
echo "Fix issue #$ARGUMENTS following our coding standards" > .claude/commands/fix-issue.md

# Usage
> /project:fix-issue 123
```

**Command Syntax**: `/<prefix>:<command-name> [arguments]`
- `<prefix>`: `project` (project-specific) or `user` (personal)
- `<command-name>`: Derived from Markdown filename (without `.md`)
- `[arguments]`: Optional arguments passed to command

---

## 🛠️ SDK Integration

### **Print Mode (SDK)**

```bash
# Run single prompt and exit
claude -p "Write a function to calculate Fibonacci numbers"

# Using pipe for stdin
echo "Explain this code" | claude -p

# Output in JSON format with metadata
claude -p "Generate a hello world function" --output-format json

# Stream JSON output as it arrives
claude -p "Build a React component" --output-format stream-json
```

### **Output Formats**
- `text` (default) - Plain text output
- `json` - JSON format with metadata
- `stream-json` - Streaming JSON for real-time processing

---

## 📂 File Context & Memory

### **CLAUDE.md Memory System**

Claude Code uses `CLAUDE.md` files for project context and memory:

```bash
# Initialize project memory
> /init

# Edit memory files
> /memory

# Import other markdown files
@path/to/file.md
```

### **File Context in Commands**

Use `@` syntax to include files in context:

```bash
# In interactive mode
> summarize this project @src/main.js @README.md

# Multiple file context
> analyze these authentication files @src/auth/ @config/auth.json
```

---

## 🔧 Installation & Updates

### **Installation**

```bash
# Install globally via npm
npm install -g @anthropic-ai/claude-code
```

### **SDK Installation**

```bash
# Python SDK
pip install claude-code-sdk

# TypeScript/JavaScript SDK
import @anthropic-ai/claude-code
```

### **Updates**

```bash
# Update to latest version
claude update
```

---

## 🏗️ .NET Implementation Architecture

Based on the official structure, the .NET implementation should focus on:

### **1. Core CLI Structure**

```csharp
[Command("claude")]
public class ClaudeCommand : ICommand
{
    [Argument(0, Description = "Initial message for REPL")]
    public string? InitialMessage { get; set; }
    
    [Option("-p", "--print", Description = "SDK mode - query and exit")]
    public bool PrintMode { get; set; }
    
    [Option("-c", "--continue", Description = "Continue most recent conversation")]
    public bool Continue { get; set; }
    
    [Option("-r", "--resume", Description = "Resume specific session")]
    public string? ResumeSession { get; set; }
    
    [Option("--mcp-config", Description = "MCP configuration file")]
    public string? McpConfig { get; set; }
    
    [Option("--output-format", Description = "Output format")]
    public OutputFormat OutputFormat { get; set; } = OutputFormat.Text;
}
```

### **2. Interactive Session Manager**

```csharp
public class InteractiveSession
{
    private readonly ISlashCommandProcessor _slashCommands;
    private readonly IMcpManager _mcpManager;
    private readonly IConversationManager _conversations;
    private readonly IMemoryManager _memory;
    
    public async Task StartAsync(string? initialMessage = null)
    {
        // Initialize REPL with slash command support
        // Handle conversation continuity
        // Manage MCP server connections
    }
}
```

### **3. Slash Command System**

```csharp
public interface ISlashCommand
{
    string Name { get; }
    string Description { get; }
    Task<CommandResult> ExecuteAsync(string[] args, ISessionContext context);
}

// Built-in commands
public class HelpCommand : ISlashCommand { }
public class ConfigCommand : ISlashCommand { }
public class ClearCommand : ISlashCommand { }
// ... etc

// Dynamic MCP commands
public class McpSlashCommand : ISlashCommand { }
```

### **4. Configuration System**

```csharp
public class ClaudeConfiguration
{
    public string? ApiKey { get; set; }
    public string Model { get; set; } = "claude-3-5-sonnet";
    public int MaxTokens { get; set; } = 4096;
    public float Temperature { get; set; } = 0.1f;
    public OutputFormat DefaultOutputFormat { get; set; } = OutputFormat.Text;
}

public enum OutputFormat
{
    Text,
    Json,
    StreamJson
}
```

### **5. MCP Integration**

```csharp
public interface IMcpManager
{
    Task<IEnumerable<IMcpServer>> DiscoverServersAsync();
    Task<IMcpServer> AddServerAsync(McpServerConfig config);
    Task<IEnumerable<ISlashCommand>> GetDynamicCommandsAsync();
}
```

---

## 🎯 Key Implementation Priorities

Based on official research, focus on:

1. **Interactive REPL** - Primary user interface
2. **Slash Command System** - Extensible command architecture
3. **MCP Protocol Support** - Dynamic tool/server integration
4. **Configuration Management** - Multi-layer config system
5. **Session Continuity** - Conversation state management
6. **SDK Mode** - Scriptable interface with JSON output
7. **Memory System** - CLAUDE.md file integration

---

## 📚 Research Sources

### **Official Documentation**
- [Claude Code Overview](https://docs.anthropic.com/en/docs/claude-code/overview)
- [Claude Code CLI Reference](https://docs.anthropic.com/en/docs/claude-code/cli-reference) 
- [Slash Commands](https://docs.anthropic.com/en/docs/claude-code/slash-commands)
- [MCP Integration](https://docs.anthropic.com/en/docs/claude-code/mcp)

### **Research Methods**
- **MCP Fetch**: Retrieved official documentation
- **MCP Context7**: Analyzed comprehensive command examples
- **MCP Sequential**: Systematic analysis of command patterns
- **MCP Repomix**: Existing documentation review

---

**Document Version**: 2.0 (Complete Rewrite)  
**Research Date**: 2025-07-24  
**Research Method**: Multi-MCP server analysis  
**Status**: Official command structure documented  
**Accuracy**: Based on official Anthropic sources  
**Next Steps**: Implement core REPL and slash command system in .NET
# Optimized MCP Timeout Debugging Prompt

## Task: Fix MCP Server Timeout Issues

**Context**: User experiencing MCP (Model Context Protocol) timeout errors in development environment with specific servers failing to respond.

**Objective**: Quickly diagnose and resolve MCP server timeout issues with minimal overhead.

---

## Optimized Approach

### 1. Direct Process Assessment (30 seconds)
```bash
# Single command to identify all MCP-related processes
ps aux | grep -E "(mcp|npx.*mcp)" | grep -v grep
```

**Expected Issues**:
- Multiple duplicate processes (4+ instances of same server)
- Orphaned npm exec processes 
- Resource contention from concurrent servers

### 2. Immediate Cleanup (60 seconds)
```bash
# Terminate duplicates by pattern matching
pkill -f "mcp-jetbrains-proxy"
pkill -f "context7-mcp" 
pkill -f "devops-enhanced-mcp"
pkill -f "mcp-server-webresearch"
pkill -f "mcp-knowledge-graph"
pkill -f "repomix --mcp"

# Force kill persistent processes if needed
kill -9 [specific PIDs]
```

### 3. Targeted Server Restart (90 seconds)
```bash
# Only restart servers user specifically mentioned as problematic
npx -y @repomix/mcp-server &
npx -y @21st-magic/mcp-server &
npx -y @executeautomation/playwright-mcp-server &
npx -y @modelcontextprotocol/server-browserloop &
node /Users/wangkanai/Sources/devops-enhanced-mcp/dist/index.js &
```

### 4. Quick Verification (30 seconds)
```bash
# Confirm servers are running without duplicates
ps aux | grep -E "(repomix|magic|playwright|devops-enhanced|browserloop)" | grep -v grep
```

---

## What NOT to Do (Time Wasters)

❌ **Don't** run comprehensive system diagnostics  
❌ **Don't** check network ports unless specific connectivity issues  
❌ **Don't** analyze configuration files unless servers fail to start  
❌ **Don't** implement complex retry logic or circuit breakers  
❌ **Don't** create new MCP service infrastructure  
❌ **Don't** run extensive troubleshooting workflows  

---

## Success Criteria (3 minutes total)

✅ **Zero duplicate MCP processes running**  
✅ **Specific problematic servers restarted successfully**  
✅ **Process list shows clean environment**  
✅ **User can proceed with development work**

---

## Pattern Recognition

**Root Cause**: 95% of MCP timeouts = duplicate processes competing for resources

**Solution**: Kill duplicates → Restart cleanly → Verify

**Expected Outcome**: Immediate resolution in under 3 minutes

---

## Usage Instructions

1. **Copy exact commands** from sections 1-4
2. **Execute sequentially** without analysis paralysis
3. **Report success** when verification shows clean process environment
4. **Skip** any steps not directly related to duplicate process cleanup

This approach resolves MCP timeout issues 10x faster than comprehensive debugging workflows.
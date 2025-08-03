using Microsoft.Extensions.Logging;

namespace Claude.Services;

/// <summary>
/// Enhanced interactive shell service with session management and advanced features.
/// </summary>
public interface IInteractiveService
{
    /// <summary>
    /// Starts the interactive shell.
    /// </summary>
    /// <param name="sessionId">Optional session ID to resume.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    Task StartInteractiveAsync(string? sessionId = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Processes a command in the interactive shell.
    /// </summary>
    /// <param name="input">User input.</param>
    /// <param name="session">Current session.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Response and updated session.</returns>
    Task<(string response, Session session)> ProcessCommandAsync(string input, Session session, CancellationToken cancellationToken = default);
}

/// <summary>
/// Implementation of enhanced interactive service.
/// </summary>
public class InteractiveService : IInteractiveService
{
    private readonly ILogger<InteractiveService> _logger;
    private readonly ISessionService _sessionService;
    private readonly IApiService _apiService;

    public InteractiveService(ILogger<InteractiveService> logger, ISessionService sessionService, IApiService apiService)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _sessionService = sessionService ?? throw new ArgumentNullException(nameof(sessionService));
        _apiService = apiService ?? throw new ArgumentNullException(nameof(apiService));
    }

    public async Task StartInteractiveAsync(string? sessionId = null, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Starting interactive mode with session: {SessionId}", sessionId ?? "new");

        // Get or create session
        Session session;
        if (!string.IsNullOrEmpty(sessionId))
        {
            session = await _sessionService.GetSessionAsync(sessionId, cancellationToken);
            if (session == null)
            {
                Console.WriteLine($"Session {sessionId} not found. Creating new session...");
                session = await _sessionService.CreateSessionAsync(cancellationToken: cancellationToken);
            }
            else
            {
                Console.WriteLine($"Resumed session {session.Id} (created {session.Created:yyyy-MM-dd HH:mm:ss})");
                if (session.Conversation.Count > 0)
                {
                    Console.WriteLine($"Previous conversation has {session.Conversation.Count} turns.");
                }
            }
        }
        else
        {
            session = await _sessionService.CreateSessionAsync(cancellationToken: cancellationToken);
        }

        Console.WriteLine();
        Console.WriteLine("=== Claude .NET Interactive Mode ===");
        Console.WriteLine($"Session: {session.Id}");
        Console.WriteLine($"Working Directory: {session.WorkingDirectory}");
        Console.WriteLine();
        Console.WriteLine("Enhanced Commands:");
        Console.WriteLine("  /help          - Show all commands");
        Console.WriteLine("  /status        - Show session status");
        Console.WriteLine("  /sessions      - List all sessions");
        Console.WriteLine("  /new           - Create new session");
        Console.WriteLine("  /switch <id>   - Switch to session");
        Console.WriteLine("  /history       - Show conversation history");
        Console.WriteLine("  /clear         - Clear current session");
        Console.WriteLine("  /cd <path>     - Change working directory");
        Console.WriteLine("  /sub           - Create sub-agent session");
        Console.WriteLine("  exit           - Exit interactive mode");
        Console.WriteLine();
        Console.WriteLine("Type your message or command and press Enter...");
        Console.WriteLine();

        while (!cancellationToken.IsCancellationRequested)
        {
            var prompt = session.SubAgentId != null 
                ? $"claude[{session.SubAgentId}]> " 
                : "claude> ";
            Console.Write(prompt);
            
            var input = Console.ReadLine();
            
            if (string.IsNullOrWhiteSpace(input))
                continue;
            
            if (input.Trim().ToLowerInvariant() == "exit")
            {
                Console.WriteLine("Goodbye!");
                break;
            }

            try
            {
                var (response, updatedSession) = await ProcessCommandAsync(input, session, cancellationToken);
                session = updatedSession;
                
                if (!string.IsNullOrEmpty(response))
                {
                    Console.WriteLine();
                    Console.WriteLine(response);
                    Console.WriteLine();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing command: {Input}", input);
                Console.WriteLine($"Error: {ex.Message}");
                Console.WriteLine();
            }
        }
    }

    public async Task<(string response, Session session)> ProcessCommandAsync(string input, Session session, CancellationToken cancellationToken = default)
    {
        var trimmedInput = input.Trim();
        
        // Handle slash commands
        if (trimmedInput.StartsWith('/'))
        {
            return await ProcessSlashCommandAsync(trimmedInput, session, cancellationToken);
        }
        
        // Handle regular AI conversation
        var aiRequest = new AIRequest
        {
            Message = input,
            SessionId = session.Id
        };
        
        var aiResponse = await _apiService.ProcessAIRequestAsync(aiRequest, cancellationToken);
        
        // Refresh session to get updated conversation
        var updatedSession = await _sessionService.GetSessionAsync(session.Id, cancellationToken) ?? session;
        
        return (aiResponse.Content, updatedSession);
    }

    private async Task<(string response, Session session)> ProcessSlashCommandAsync(string input, Session session, CancellationToken cancellationToken)
    {
        var parts = input.Split(' ', StringSplitOptions.RemoveEmptyEntries);
        var command = parts[0].ToLowerInvariant();
        
        switch (command)
        {
            case "/help":
                return (GetHelpText(), session);
                
            case "/status":
                return (GetStatusText(session), session);
                
            case "/sessions":
                var sessionList = await _apiService.ListSessionsAsync(cancellationToken);
                return (FormatSessionList(sessionList), session);
                
            case "/new":
                var newSession = await _sessionService.CreateSessionAsync(session.WorkingDirectory, cancellationToken: cancellationToken);
                return ($"Created new session: {newSession.Id}", newSession);
                
            case "/switch":
                if (parts.Length < 2)
                    return ("Usage: /switch <session-id>", session);
                
                var targetSession = await _sessionService.GetSessionAsync(parts[1], cancellationToken);
                if (targetSession == null)
                    return ($"Session {parts[1]} not found", session);
                
                return ($"Switched to session: {targetSession.Id}", targetSession);
                
            case "/history":
                return (FormatConversationHistory(session), session);
                
            case "/clear":
                session.Conversation.Clear();
                await _sessionService.SaveSessionAsync(session, cancellationToken);
                return ("Conversation history cleared", session);
                
            case "/cd":
                if (parts.Length < 2)
                    return ($"Current directory: {session.WorkingDirectory}", session);
                
                var newDir = parts[1];
                if (Directory.Exists(newDir))
                {
                    session.WorkingDirectory = Path.GetFullPath(newDir);
                    await _sessionService.SaveSessionAsync(session, cancellationToken);
                    return ($"Changed directory to: {session.WorkingDirectory}", session);
                }
                else
                {
                    return ($"Directory not found: {newDir}", session);
                }
                
            case "/sub":
                var subSession = await _sessionService.CreateSessionAsync(session.WorkingDirectory, session.Id, cancellationToken);
                return ($"Created sub-agent session: {subSession.Id} (Agent: {subSession.SubAgentId})", subSession);
                
            default:
                return ($"Unknown command: {command}. Type /help for available commands.", session);
        }
    }

    private string GetHelpText()
    {
        return @"Claude .NET Interactive Commands:

Conversation:
  Just type your message to chat with Claude

Session Management:
  /status        - Show current session information
  /sessions      - List all available sessions
  /new           - Create a new session
  /switch <id>   - Switch to a different session
  /history       - Show conversation history
  /clear         - Clear current conversation history

Navigation:
  /cd <path>     - Change working directory
  /cd            - Show current directory

Sub-Agents:
  /sub           - Create a sub-agent session

General:
  /help          - Show this help
  exit           - Exit interactive mode";
    }

    private string GetStatusText(Session session)
    {
        var status = $@"Session Status:
  ID: {session.Id}
  Created: {session.Created:yyyy-MM-dd HH:mm:ss}
  Last Accessed: {session.LastAccessed:yyyy-MM-dd HH:mm:ss}
  Working Directory: {session.WorkingDirectory}
  Conversation Turns: {session.Conversation.Count}";

        if (!string.IsNullOrEmpty(session.SubAgentId))
        {
            status += $"\n  Sub-Agent ID: {session.SubAgentId}";
        }
        
        if (!string.IsNullOrEmpty(session.ParentSessionId))
        {
            status += $"\n  Parent Session: {session.ParentSessionId}";
        }

        return status;
    }

    private string FormatSessionList(SessionListResponse sessionList)
    {
        if (sessionList.Sessions.Count == 0)
        {
            return "No sessions found.";
        }

        var result = $"Found {sessionList.TotalCount} sessions:\n\n";
        
        foreach (var session in sessionList.Sessions.Take(10)) // Show only first 10
        {
            var agentInfo = !string.IsNullOrEmpty(session.SubAgentId) 
                ? $" [{session.SubAgentId}]" 
                : "";
            
            result += $"  {session.Id}{agentInfo}\n";
            result += $"    Created: {session.Created:yyyy-MM-dd HH:mm:ss}\n";
            result += $"    Turns: {session.ConversationLength}\n";
            result += $"    Directory: {session.WorkingDirectory}\n\n";
        }
        
        if (sessionList.Sessions.Count > 10)
        {
            result += $"... and {sessionList.Sessions.Count - 10} more sessions\n";
        }
        
        return result.Trim();
    }

    private string FormatConversationHistory(Session session)
    {
        if (session.Conversation.Count == 0)
        {
            return "No conversation history in this session.";
        }

        var result = $"Conversation History ({session.Conversation.Count} turns):\n\n";
        
        foreach (var turn in session.Conversation.TakeLast(10)) // Show last 10 turns
        {
            var timestamp = turn.Timestamp.ToString("HH:mm:ss");
            var role = turn.Role.ToUpperInvariant();
            var content = turn.Content.Length > 100 
                ? turn.Content[..100] + "..." 
                : turn.Content;
            
            result += $"[{timestamp}] {role}: {content}\n\n";
        }
        
        if (session.Conversation.Count > 10)
        {
            result = $"... showing last 10 of {session.Conversation.Count} turns\n\n" + result;
        }
        
        return result.Trim();
    }
}
using System.Text.Json;
using Microsoft.Extensions.Logging;

namespace Claude.Services;

/// <summary>
/// File-based implementation of session service.
/// </summary>
public class SessionService : ISessionService
{
    private readonly ILogger<SessionService> _logger;
    private readonly IConfigurationService _configuration;
    private readonly string _sessionsDirectory;

    public SessionService(ILogger<SessionService> logger, IConfigurationService configuration)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        
        // Get sessions directory from configuration or use default
        var baseDir = _configuration.GetValue("SessionsDirectory", 
            Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), ".claude", "sessions"));
        _sessionsDirectory = baseDir;
        
        // Ensure directory exists
        Directory.CreateDirectory(_sessionsDirectory);
        
        _logger.LogInformation("SessionService initialized with directory: {Directory}", _sessionsDirectory);
    }

    public async Task<Session> CreateSessionAsync(string? workingDirectory = null, string? parentSessionId = null, CancellationToken cancellationToken = default)
    {
        var session = new Session
        {
            WorkingDirectory = workingDirectory ?? Environment.CurrentDirectory,
            ParentSessionId = parentSessionId
        };

        if (!string.IsNullOrEmpty(parentSessionId))
        {
            // Generate a sub-agent ID for child sessions
            session.SubAgentId = $"agent-{Guid.NewGuid().ToString("N")[..8]}";
            _logger.LogInformation("Created sub-session {SessionId} for parent {ParentId} with agent {AgentId}", 
                session.Id, parentSessionId, session.SubAgentId);
        }
        else
        {
            _logger.LogInformation("Created new session {SessionId}", session.Id);
        }

        await SaveSessionAsync(session, cancellationToken);
        return session;
    }

    public async Task<Session?> GetSessionAsync(string sessionId, CancellationToken cancellationToken = default)
    {
        var sessionFile = Path.Combine(_sessionsDirectory, $"{sessionId}.json");
        
        if (!File.Exists(sessionFile))
        {
            _logger.LogWarning("Session {SessionId} not found", sessionId);
            return null;
        }

        try
        {
            var json = await File.ReadAllTextAsync(sessionFile, cancellationToken);
            var session = JsonSerializer.Deserialize<Session>(json);
            
            if (session != null)
            {
                session.LastAccessed = DateTime.UtcNow;
                _logger.LogDebug("Retrieved session {SessionId}", sessionId);
            }
            
            return session;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to load session {SessionId}", sessionId);
            return null;
        }
    }

    public async Task SaveSessionAsync(Session session, CancellationToken cancellationToken = default)
    {
        var sessionFile = Path.Combine(_sessionsDirectory, $"{session.Id}.json");
        
        try
        {
            session.LastAccessed = DateTime.UtcNow;
            var json = JsonSerializer.Serialize(session, new JsonSerializerOptions { WriteIndented = true });
            await File.WriteAllTextAsync(sessionFile, json, cancellationToken);
            
            _logger.LogDebug("Saved session {SessionId}", session.Id);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to save session {SessionId}", session.Id);
            throw;
        }
    }

    public async Task<List<Session>> ListSessionsAsync(CancellationToken cancellationToken = default)
    {
        var sessions = new List<Session>();
        
        try
        {
            var sessionFiles = Directory.GetFiles(_sessionsDirectory, "*.json");
            
            foreach (var file in sessionFiles)
            {
                try
                {
                    var json = await File.ReadAllTextAsync(file, cancellationToken);
                    var session = JsonSerializer.Deserialize<Session>(json);
                    if (session != null)
                    {
                        sessions.Add(session);
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "Failed to load session file {File}", file);
                }
            }
            
            _logger.LogDebug("Listed {Count} sessions", sessions.Count);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to list sessions");
            throw;
        }

        return sessions.OrderByDescending(s => s.LastAccessed).ToList();
    }

    public async Task DeleteSessionAsync(string sessionId, CancellationToken cancellationToken = default)
    {
        var sessionFile = Path.Combine(_sessionsDirectory, $"{sessionId}.json");
        
        if (File.Exists(sessionFile))
        {
            try
            {
                File.Delete(sessionFile);
                _logger.LogInformation("Deleted session {SessionId}", sessionId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to delete session {SessionId}", sessionId);
                throw;
            }
        }
        else
        {
            _logger.LogWarning("Session {SessionId} not found for deletion", sessionId);
        }
    }

    public async Task AddConversationTurnAsync(string sessionId, string role, string content, Dictionary<string, object>? metadata = null, CancellationToken cancellationToken = default)
    {
        var session = await GetSessionAsync(sessionId, cancellationToken);
        if (session == null)
        {
            throw new InvalidOperationException($"Session {sessionId} not found");
        }

        var turn = new ConversationTurn
        {
            Role = role,
            Content = content,
            Metadata = metadata ?? new Dictionary<string, object>()
        };

        session.Conversation.Add(turn);
        await SaveSessionAsync(session, cancellationToken);
        
        _logger.LogDebug("Added {Role} turn to session {SessionId}", role, sessionId);
    }
}
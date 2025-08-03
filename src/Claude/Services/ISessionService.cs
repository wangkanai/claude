using System.Text.Json;

namespace Claude.Services;

/// <summary>
/// Represents a conversation session with AI agents.
/// </summary>
public class Session
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public DateTime Created { get; set; } = DateTime.UtcNow;
    public DateTime LastAccessed { get; set; } = DateTime.UtcNow;
    public string WorkingDirectory { get; set; } = Environment.CurrentDirectory;
    public List<ConversationTurn> Conversation { get; set; } = new();
    public Dictionary<string, object> Context { get; set; } = new();
    public string? SubAgentId { get; set; }
    public string? ParentSessionId { get; set; }
}

/// <summary>
/// Represents a single turn in a conversation.
/// </summary>
public class ConversationTurn
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    public string Role { get; set; } = string.Empty; // "user", "assistant", "system"
    public string Content { get; set; } = string.Empty;
    public Dictionary<string, object> Metadata { get; set; } = new();
}

/// <summary>
/// Service for managing conversational sessions with AI agents.
/// </summary>
public interface ISessionService
{
    /// <summary>
    /// Creates a new session.
    /// </summary>
    /// <param name="workingDirectory">Optional working directory for the session.</param>
    /// <param name="parentSessionId">Optional parent session ID for sub-agents.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The created session.</returns>
    Task<Session> CreateSessionAsync(string? workingDirectory = null, string? parentSessionId = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets a session by ID.
    /// </summary>
    /// <param name="sessionId">The session ID.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The session if found, null otherwise.</returns>
    Task<Session?> GetSessionAsync(string sessionId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Saves or updates a session.
    /// </summary>
    /// <param name="session">The session to save.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    Task SaveSessionAsync(Session session, CancellationToken cancellationToken = default);

    /// <summary>
    /// Lists all sessions.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>List of sessions.</returns>
    Task<List<Session>> ListSessionsAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Deletes a session.
    /// </summary>
    /// <param name="sessionId">The session ID to delete.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    Task DeleteSessionAsync(string sessionId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Adds a conversation turn to a session.
    /// </summary>
    /// <param name="sessionId">The session ID.</param>
    /// <param name="role">The role (user, assistant, system).</param>
    /// <param name="content">The content of the turn.</param>
    /// <param name="metadata">Optional metadata.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    Task AddConversationTurnAsync(string sessionId, string role, string content, Dictionary<string, object>? metadata = null, CancellationToken cancellationToken = default);
}
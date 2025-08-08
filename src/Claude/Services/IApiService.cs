using Microsoft.AspNetCore.Mvc;

namespace Claude.Services;

/// <summary>
/// Represents a request to the AI API.
/// </summary>
public class AIRequest
{
    public string Message { get; set; } = string.Empty;
    public string? SessionId { get; set; }
    public Dictionary<string, object>? Parameters { get; set; }
    public string? SystemPrompt { get; set; }
}

/// <summary>
/// Represents a response from the AI API.
/// </summary>
public class AIResponse
{
    public string Content { get; set; } = string.Empty;
    public string SessionId { get; set; } = string.Empty;
    public Dictionary<string, object> Metadata { get; set; } = new();
    public bool Success { get; set; } = true;
    public string? Error { get; set; }
}

/// <summary>
/// Represents a session list response.
/// </summary>
public class SessionListResponse
{
    public List<SessionSummary> Sessions { get; set; } = new();
    public int TotalCount { get; set; }
}

/// <summary>
/// Summary information about a session.
/// </summary>
public class SessionSummary
{
    public string Id { get; set; } = string.Empty;
    public DateTime Created { get; set; }
    public DateTime LastAccessed { get; set; }
    public string WorkingDirectory { get; set; } = string.Empty;
    public int ConversationLength { get; set; }
    public string? SubAgentId { get; set; }
    public string? ParentSessionId { get; set; }
}

/// <summary>
/// Service for handling API operations.
/// </summary>
public interface IApiService
{
    /// <summary>
    /// Processes an AI request.
    /// </summary>
    /// <param name="request">The AI request.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The AI response.</returns>
    Task<AIResponse> ProcessAIRequestAsync(AIRequest request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets session information.
    /// </summary>
    /// <param name="sessionId">The session ID.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Session information or null if not found.</returns>
    Task<Session?> GetSessionInfoAsync(string sessionId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Lists all sessions.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Session list response.</returns>
    Task<SessionListResponse> ListSessionsAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Creates a new session.
    /// </summary>
    /// <param name="workingDirectory">Optional working directory.</param>
    /// <param name="parentSessionId">Optional parent session ID.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The created session.</returns>
    Task<Session> CreateSessionAsync(string? workingDirectory = null, string? parentSessionId = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Deletes a session.
    /// </summary>
    /// <param name="sessionId">The session ID to delete.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    Task DeleteSessionAsync(string sessionId, CancellationToken cancellationToken = default);
}
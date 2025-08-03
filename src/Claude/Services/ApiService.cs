using Microsoft.Extensions.Logging;

namespace Claude.Services;

/// <summary>
/// Implementation of API service that coordinates with session service and simulates AI responses.
/// </summary>
public class ApiService : IApiService
{
    private readonly ILogger<ApiService> _logger;
    private readonly ISessionService _sessionService;

    public ApiService(ILogger<ApiService> logger, ISessionService sessionService)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _sessionService = sessionService ?? throw new ArgumentNullException(nameof(sessionService));
    }

    public async Task<AIResponse> ProcessAIRequestAsync(AIRequest request, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Processing AI request for session {SessionId}", request.SessionId);

            // Get or create session
            Session session;
            if (!string.IsNullOrEmpty(request.SessionId))
            {
                session = await _sessionService.GetSessionAsync(request.SessionId, cancellationToken) 
                    ?? await _sessionService.CreateSessionAsync(cancellationToken: cancellationToken);
            }
            else
            {
                session = await _sessionService.CreateSessionAsync(cancellationToken: cancellationToken);
            }

            // Add user message to conversation
            await _sessionService.AddConversationTurnAsync(session.Id, "user", request.Message, 
                request.Parameters, cancellationToken);

            // Simulate AI processing - in a real implementation, this would call Claude API
            var aiResponse = await SimulateAIResponseAsync(request, session, cancellationToken);

            // Add AI response to conversation
            await _sessionService.AddConversationTurnAsync(session.Id, "assistant", aiResponse, 
                cancellationToken: cancellationToken);

            return new AIResponse
            {
                Content = aiResponse,
                SessionId = session.Id,
                Success = true,
                Metadata = new Dictionary<string, object>
                {
                    ["processing_time_ms"] = 100, // Simulated
                    ["conversation_length"] = session.Conversation.Count,
                    ["working_directory"] = session.WorkingDirectory
                }
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to process AI request");
            return new AIResponse
            {
                Success = false,
                Error = $"Failed to process request: {ex.Message}"
            };
        }
    }

    private async Task<string> SimulateAIResponseAsync(AIRequest request, Session session, CancellationToken cancellationToken)
    {
        // Simulate processing delay
        await Task.Delay(100, cancellationToken);

        // Simple response simulation based on common patterns
        var message = request.Message.ToLowerInvariant();
        
        if (message.Contains("analyze"))
        {
            return $"I'll analyze the code in your working directory: {session.WorkingDirectory}. " +
                   "This is a simulated response. In the full implementation, I would perform actual code analysis.";
        }
        
        if (message.Contains("implement") || message.Contains("create"))
        {
            return "I'll help you implement that feature. This is a simulated response. " +
                   "In the full implementation, I would generate actual code and files.";
        }
        
        if (message.Contains("help") || message.Contains("?"))
        {
            return "I'm Claude, your AI coding assistant. I can help you analyze code, implement features, " +
                   "debug issues, and answer programming questions. What would you like to work on?";
        }
        
        if (message.Contains("session") || message.Contains("status"))
        {
            return $"Current session: {session.Id}\n" +
                   $"Working directory: {session.WorkingDirectory}\n" +
                   $"Conversation turns: {session.Conversation.Count}\n" +
                   $"Created: {session.Created:yyyy-MM-dd HH:mm:ss}";
        }

        // Default response
        return $"I understand you said: \"{request.Message}\". " +
               "This is a simulated response from the Claude .NET implementation. " +
               "In the full version, I would provide intelligent assistance based on your request.";
    }

    public async Task<Session?> GetSessionInfoAsync(string sessionId, CancellationToken cancellationToken = default)
    {
        return await _sessionService.GetSessionAsync(sessionId, cancellationToken);
    }

    public async Task<SessionListResponse> ListSessionsAsync(CancellationToken cancellationToken = default)
    {
        var sessions = await _sessionService.ListSessionsAsync(cancellationToken);
        
        return new SessionListResponse
        {
            Sessions = sessions.Select(s => new SessionSummary
            {
                Id = s.Id,
                Created = s.Created,
                LastAccessed = s.LastAccessed,
                WorkingDirectory = s.WorkingDirectory,
                ConversationLength = s.Conversation.Count,
                SubAgentId = s.SubAgentId,
                ParentSessionId = s.ParentSessionId
            }).ToList(),
            TotalCount = sessions.Count
        };
    }

    public async Task<Session> CreateSessionAsync(string? workingDirectory = null, string? parentSessionId = null, CancellationToken cancellationToken = default)
    {
        return await _sessionService.CreateSessionAsync(workingDirectory, parentSessionId, cancellationToken);
    }

    public async Task DeleteSessionAsync(string sessionId, CancellationToken cancellationToken = default)
    {
        await _sessionService.DeleteSessionAsync(sessionId, cancellationToken);
    }
}
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Claude.Services;

namespace Claude.Controllers;

/// <summary>
/// API controller for AI interactions.
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class AiController : ControllerBase
{
    private readonly IApiService _apiService;
    private readonly ILogger<AiController> _logger;

    public AiController(IApiService apiService, ILogger<AiController> logger)
    {
        _apiService = apiService ?? throw new ArgumentNullException(nameof(apiService));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <summary>
    /// Sends a message to the AI and gets a response.
    /// </summary>
    /// <param name="request">The AI request.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>AI response.</returns>
    [HttpPost("chat")]
    public async Task<ActionResult<AIResponse>> ChatAsync([FromBody] AIRequest request, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(request.Message))
        {
            return BadRequest(new { error = "Message is required" });
        }

        try
        {
            var response = await _apiService.ProcessAIRequestAsync(request, cancellationToken);
            return Ok(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to process AI request");
            return StatusCode(500, new { error = "Internal server error", detail = ex.Message });
        }
    }

    /// <summary>
    /// Gets the health status of the AI service.
    /// </summary>
    /// <returns>Health status.</returns>
    [HttpGet("health")]
    public ActionResult<object> GetHealth()
    {
        return Ok(new
        {
            status = "healthy",
            timestamp = DateTime.UtcNow,
            version = "1.0.0-preview.1"
        });
    }
}

/// <summary>
/// API controller for session management.
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class SessionsController : ControllerBase
{
    private readonly IApiService _apiService;
    private readonly ILogger<SessionsController> _logger;

    public SessionsController(IApiService apiService, ILogger<SessionsController> logger)
    {
        _apiService = apiService ?? throw new ArgumentNullException(nameof(apiService));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <summary>
    /// Lists all sessions.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>List of sessions.</returns>
    [HttpGet]
    public async Task<ActionResult<SessionListResponse>> GetSessionsAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            var response = await _apiService.ListSessionsAsync(cancellationToken);
            return Ok(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to list sessions");
            return StatusCode(500, new { error = "Failed to list sessions", detail = ex.Message });
        }
    }

    /// <summary>
    /// Gets a specific session.
    /// </summary>
    /// <param name="id">Session ID.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Session information.</returns>
    [HttpGet("{id}")]
    public async Task<ActionResult<Session>> GetSessionAsync(string id, CancellationToken cancellationToken = default)
    {
        try
        {
            var session = await _apiService.GetSessionInfoAsync(id, cancellationToken);
            if (session == null)
            {
                return NotFound(new { error = $"Session {id} not found" });
            }
            return Ok(session);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get session {SessionId}", id);
            return StatusCode(500, new { error = "Failed to get session", detail = ex.Message });
        }
    }

    /// <summary>
    /// Creates a new session.
    /// </summary>
    /// <param name="request">Session creation request.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Created session.</returns>
    [HttpPost]
    public async Task<ActionResult<Session>> CreateSessionAsync([FromBody] CreateSessionRequest? request = null, CancellationToken cancellationToken = default)
    {
        try
        {
            var session = await _apiService.CreateSessionAsync(
                request?.WorkingDirectory, 
                request?.ParentSessionId, 
                cancellationToken);
            return CreatedAtAction(nameof(GetSessionAsync), new { id = session.Id }, session);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to create session");
            return StatusCode(500, new { error = "Failed to create session", detail = ex.Message });
        }
    }

    /// <summary>
    /// Deletes a session.
    /// </summary>
    /// <param name="id">Session ID.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>No content.</returns>
    [HttpDelete("{id}")]
    public async Task<ActionResult> DeleteSessionAsync(string id, CancellationToken cancellationToken = default)
    {
        try
        {
            await _apiService.DeleteSessionAsync(id, cancellationToken);
            return NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to delete session {SessionId}", id);
            return StatusCode(500, new { error = "Failed to delete session", detail = ex.Message });
        }
    }
}

/// <summary>
/// Request for creating a new session.
/// </summary>
public class CreateSessionRequest
{
    public string? WorkingDirectory { get; set; }
    public string? ParentSessionId { get; set; }
}
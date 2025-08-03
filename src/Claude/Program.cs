using System.CommandLine;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Builder;
using Serilog;
using Claude.Services;

namespace Claude;

/// <summary>
/// Entry point for the Claude CLI application.
/// </summary>
public static class Program
{
    /// <summary>
    /// Main entry point for the application.
    /// </summary>
    /// <param name="args">Command line arguments</param>
    /// <returns>Exit code</returns>
    public static async Task<int> Main(string[] args)
    {
        // Configure Serilog
        Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Information()
            .WriteTo.Console()
            .CreateLogger();

        try
        {
            // Create root command
            var rootCommand = CreateRootCommand();

            // Execute command
            return await rootCommand.InvokeAsync(args);
        }
        catch (Exception ex)
        {
            Log.Fatal(ex, "Application terminated unexpectedly");
            return 1;
        }
        finally
        {
            Log.CloseAndFlush();
        }
    }

    /// <summary>
    /// Creates the host builder with dependency injection configuration.
    /// </summary>
    /// <param name="args">Command line arguments</param>
    /// <returns>Configured host builder</returns>
    private static IHostBuilder CreateHostBuilder(string[] args) =>
        Host.CreateDefaultBuilder(args)
            .UseSerilog()
            .ConfigureServices((context, services) =>
            {
                // Register core services
                services.AddSingleton<IFileSystemService, FileSystemService>();
                services.AddSingleton<IConfigurationService, ConfigurationService>();
                services.AddSingleton<ISessionService, SessionService>();
                services.AddSingleton<IApiService, ApiService>();
                services.AddSingleton<IInteractiveService, InteractiveService>();
            });

    /// <summary>
    /// Creates the root command with all subcommands.
    /// </summary>
    /// <returns>Configured root command</returns>
    private static RootCommand CreateRootCommand()
    {
        // Define global options
        var verboseOption = new Option<bool>("--verbose", "Enable verbose logging");
        var configOption = new Option<string>("--config", "Configuration file path");

        var rootCommand = new RootCommand("claude dotnet - High-performance .NET reimplementation of Anthropic's Claude Code CLI");
        
        // Add global options
        rootCommand.AddGlobalOption(verboseOption);
        rootCommand.AddGlobalOption(configOption);

        // Add subcommands for the three usage modes
        rootCommand.AddCommand(CreateAnalyzeCommand(verboseOption, configOption));
        rootCommand.AddCommand(CreateImplementCommand(verboseOption, configOption));
        rootCommand.AddCommand(CreateInteractiveCommand(verboseOption, configOption));
        rootCommand.AddCommand(CreateApiServerCommand(verboseOption, configOption));
        rootCommand.AddCommand(CreateConfigCommand(verboseOption, configOption));
        rootCommand.AddCommand(CreateSessionCommand(verboseOption, configOption));

        // Set default handler for root command
        rootCommand.SetHandler(async (bool verbose, string config) =>
        {
            Console.WriteLine("=== Claude .NET - AI-Powered Development CLI ===");
            Console.WriteLine();
            Console.WriteLine("Available Usage Modes:");
            Console.WriteLine();
            Console.WriteLine("1. Interactive Shell Terminal:");
            Console.WriteLine("   claude interactive [--session <id>]");
            Console.WriteLine("   - Chat with AI agents in an interactive terminal");
            Console.WriteLine("   - Session management and context preservation");
            Console.WriteLine("   - Sub-agent allocation for specialized tasks");
            Console.WriteLine();
            Console.WriteLine("2. Command Line Interface:");
            Console.WriteLine("   claude analyze [options] <files>");
            Console.WriteLine("   claude implement <description>");
            Console.WriteLine("   claude config [set|get|list] [key] [value]");
            Console.WriteLine("   claude session [list|create|delete] [id]");
            Console.WriteLine("   - Traditional CLI commands for configuration and analysis");
            Console.WriteLine();
            Console.WriteLine("3. API Server Protocol:");
            Console.WriteLine("   claude serve [--port <port>] [--host <host>]");
            Console.WriteLine("   - HTTP REST API for external application integration");
            Console.WriteLine("   - Endpoints: /api/ai/chat, /api/sessions, etc.");
            Console.WriteLine();
            Console.WriteLine("Use 'claude --help' for detailed command information.");
            Console.WriteLine("Use 'claude <command> --help' for specific command help.");
        }, verboseOption, configOption);

        return rootCommand;
    }

    /// <summary>
    /// Creates the analyze command.
    /// </summary>
    /// <returns>Configured analyze command</returns>
    private static Command CreateAnalyzeCommand(Option<bool> verboseOption, Option<string> configOption)
    {
        var analyzeCommand = new Command("analyze", "Analyze code and provide insights")
        {
            new Option<string>("--scope", "Analysis scope (file|module|project|system)") { IsRequired = false },
            new Option<string>("--focus", "Focus area (performance|security|quality|architecture)") { IsRequired = false },
            new Option<string>("--session", "Session ID to use for context") { IsRequired = false },
            new Argument<string[]>("files", "Files to analyze") { Arity = ArgumentArity.ZeroOrMore }
        };

        analyzeCommand.SetHandler(async (string scope, string focus, string sessionId, string[] files, bool verbose, string config) =>
        {
            // Create services
            var hostBuilder = CreateHostBuilder(Array.Empty<string>());
            var host = hostBuilder.Build();
            var apiService = host.Services.GetRequiredService<IApiService>();
            
            Console.WriteLine($"=== Code Analysis ===");
            Console.WriteLine($"Scope: {scope ?? "project"}");
            Console.WriteLine($"Focus: {focus ?? "general"}");
            
            if (files.Length > 0)
            {
                Console.WriteLine($"Files: {string.Join(", ", files)}");
            }
            Console.WriteLine();

            // Create analysis request
            var message = $"Please analyze the code ";
            if (files.Length > 0)
            {
                message += $"in files: {string.Join(", ", files)} ";
            }
            message += $"with scope '{scope ?? "project"}' and focus on '{focus ?? "general"}'.";

            var request = new Services.AIRequest
            {
                Message = message,
                SessionId = sessionId,
                Parameters = new Dictionary<string, object>
                {
                    ["command"] = "analyze",
                    ["scope"] = scope ?? "project",
                    ["focus"] = focus ?? "general",
                    ["files"] = files
                }
            };

            var response = await apiService.ProcessAIRequestAsync(request);
            
            if (response.Success)
            {
                Console.WriteLine(response.Content);
                if (!string.IsNullOrEmpty(sessionId))
                {
                    Console.WriteLine($"\nSession: {response.SessionId}");
                }
            }
            else
            {
                Console.WriteLine($"Error: {response.Error}");
            }
        }, 
        new Option<string>("--scope"), 
        new Option<string>("--focus"), 
        new Option<string>("--session"),
        new Argument<string[]>("files"),
        verboseOption,
        configOption);

        return analyzeCommand;
    }

    /// <summary>
    /// Creates the implement command.
    /// </summary>
    /// <returns>Configured implement command</returns>
    private static Command CreateImplementCommand(Option<bool> verboseOption, Option<string> configOption)
    {
        var implementCommand = new Command("implement", "Implement features based on requirements")
        {
            new Option<string>("--type", "Implementation type (component|api|service|feature)") { IsRequired = false },
            new Option<string>("--framework", "Target framework") { IsRequired = false },
            new Option<string>("--session", "Session ID to use for context") { IsRequired = false },
            new Argument<string>("description", "Description of what to implement") { Arity = ArgumentArity.ExactlyOne }
        };

        implementCommand.SetHandler(async (string type, string framework, string sessionId, string description, bool verbose, string config) =>
        {
            // Create services
            var hostBuilder = CreateHostBuilder(Array.Empty<string>());
            var host = hostBuilder.Build();
            var apiService = host.Services.GetRequiredService<IApiService>();
            
            Console.WriteLine($"=== Feature Implementation ===");
            Console.WriteLine($"Description: {description}");
            Console.WriteLine($"Type: {type ?? "feature"}");
            Console.WriteLine($"Framework: {framework ?? "auto-detect"}");
            Console.WriteLine();

            var message = $"Please implement: {description}";
            if (!string.IsNullOrEmpty(type))
            {
                message += $" as a {type}";
            }
            if (!string.IsNullOrEmpty(framework))
            {
                message += $" using {framework}";
            }

            var request = new Services.AIRequest
            {
                Message = message,
                SessionId = sessionId,
                Parameters = new Dictionary<string, object>
                {
                    ["command"] = "implement",
                    ["type"] = type ?? "feature",
                    ["framework"] = framework ?? "auto-detect",
                    ["description"] = description
                }
            };

            var response = await apiService.ProcessAIRequestAsync(request);
            
            if (response.Success)
            {
                Console.WriteLine(response.Content);
                if (!string.IsNullOrEmpty(sessionId))
                {
                    Console.WriteLine($"\nSession: {response.SessionId}");
                }
            }
            else
            {
                Console.WriteLine($"Error: {response.Error}");
            }
        }, 
        new Option<string>("--type"), 
        new Option<string>("--framework"), 
        new Option<string>("--session"),
        new Argument<string>("description"),
        verboseOption,
        configOption);

        return implementCommand;
    }

    /// <summary>
    /// Creates the interactive command.
    /// </summary>
    /// <returns>Configured interactive command</returns>
    private static Command CreateInteractiveCommand(Option<bool> verboseOption, Option<string> configOption)
    {
        var interactiveCommand = new Command("interactive", "Start interactive shell terminal mode")
        {
            new Option<string>("--session", "Session ID to resume") { IsRequired = false }
        };

        interactiveCommand.SetHandler(async (string sessionId, bool verbose, string config) =>
        {
            // Create services
            var hostBuilder = CreateHostBuilder(Array.Empty<string>());
            var host = hostBuilder.Build();
            var interactiveService = host.Services.GetRequiredService<IInteractiveService>();
            
            await interactiveService.StartInteractiveAsync(sessionId);
        }, 
        new Option<string>("--session"),
        verboseOption,
        configOption);

        return interactiveCommand;
    }

    /// <summary>
    /// Creates the API server command.
    /// </summary>
    /// <returns>Configured API server command</returns>
    private static Command CreateApiServerCommand(Option<bool> verboseOption, Option<string> configOption)
    {
        var apiCommand = new Command("serve", "Start HTTP API server for external application integration")
        {
            new Option<int>("--port", () => 5000, "Port to listen on"),
            new Option<string>("--host", () => "localhost", "Host to bind to"),
            new Option<bool>("--swagger", "Enable Swagger UI")
        };

        apiCommand.SetHandler(async (int port, string host, bool swagger, bool verbose, string config) =>
        {
            // Ensure host is not null or empty
            if (string.IsNullOrEmpty(host))
            {
                host = "localhost";
            }
            
            Console.WriteLine($"=== Claude .NET API Server ===");
            Console.WriteLine($"Starting HTTP API server on {host}:{port}");
            Console.WriteLine();
            Console.WriteLine("Available endpoints:");
            Console.WriteLine($"  POST   http://{host}:{port}/api/ai/chat");
            Console.WriteLine($"  GET    http://{host}:{port}/api/ai/health");
            Console.WriteLine($"  GET    http://{host}:{port}/api/sessions");
            Console.WriteLine($"  POST   http://{host}:{port}/api/sessions");
            Console.WriteLine($"  GET    http://{host}:{port}/api/sessions/{{id}}");
            Console.WriteLine($"  DELETE http://{host}:{port}/api/sessions/{{id}}");
            if (swagger)
            {
                Console.WriteLine($"  Swagger UI: http://{host}:{port}/swagger");
            }
            Console.WriteLine();
            Console.WriteLine("Press Ctrl+C to stop the server...");
            Console.WriteLine();

            var builder = WebApplication.CreateBuilder();
            
            // Configure services
            builder.Services.AddControllers();
            builder.Services.AddSingleton<IFileSystemService, FileSystemService>();
            builder.Services.AddSingleton<IConfigurationService, ConfigurationService>();
            builder.Services.AddSingleton<ISessionService, SessionService>();
            builder.Services.AddSingleton<IApiService, ApiService>();
            
            if (swagger)
            {
                builder.Services.AddEndpointsApiExplorer();
                builder.Services.AddSwaggerGen();
            }

            // Configure logging
            builder.Logging.ClearProviders();
            builder.Host.UseSerilog();

            var app = builder.Build();

            // Configure pipeline
            if (swagger)
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseRouting();
            app.MapControllers();

            // Add simple health check at root
            app.MapGet("/", () => new { 
                service = "Claude .NET API",
                version = "1.0.0-preview.1",
                timestamp = DateTime.UtcNow,
                endpoints = new[] { 
                    "/api/ai/chat", 
                    "/api/ai/health",
                    "/api/sessions" 
                }
            });

            // Properly format the URL
            var url = $"http://{host}:{port}";
            Console.WriteLine($"Starting server at: {url}");
            
            await app.RunAsync(url);
        }, 
        new Option<int>("--port"), 
        new Option<string>("--host"), 
        new Option<bool>("--swagger"),
        verboseOption,
        configOption);

        return apiCommand;
    }

    /// <summary>
    /// Creates the configuration command.
    /// </summary>
    /// <returns>Configured configuration command</returns>
    private static Command CreateConfigCommand(Option<bool> verboseOption, Option<string> configOption)
    {
        var configCommand = new Command("config", "Manage application configuration");

        var setCommand = new Command("set", "Set a configuration value")
        {
            new Argument<string>("key", "Configuration key"),
            new Argument<string>("value", "Configuration value")
        };

        var getCommand = new Command("get", "Get a configuration value")
        {
            new Argument<string>("key", "Configuration key")
        };

        var listCommand = new Command("list", "List all configuration values");

        setCommand.SetHandler(async (string key, string value, bool verbose, string config) =>
        {
            var hostBuilder = CreateHostBuilder(Array.Empty<string>());
            var host = hostBuilder.Build();
            var configService = host.Services.GetRequiredService<IConfigurationService>();
            
            configService.SetValue(key, value);
            Console.WriteLine($"Configuration '{key}' set to '{value}'");
        },
        new Argument<string>("key"),
        new Argument<string>("value"),
        verboseOption,
        configOption);

        getCommand.SetHandler(async (string key, bool verbose, string config) =>
        {
            var hostBuilder = CreateHostBuilder(Array.Empty<string>());
            var host = hostBuilder.Build();
            var configService = host.Services.GetRequiredService<IConfigurationService>();
            
            var value = configService.GetValue<string>(key);
            if (value != null)
            {
                Console.WriteLine($"{key}: {value}");
            }
            else
            {
                Console.WriteLine($"Configuration '{key}' not found");
            }
        },
        new Argument<string>("key"),
        verboseOption,
        configOption);

        listCommand.SetHandler(async (bool verbose, string config) =>
        {
            Console.WriteLine("Configuration management will be implemented in the next phase.");
            Console.WriteLine("Available configuration options:");
            Console.WriteLine("  SessionsDirectory - Directory for storing session files");
            Console.WriteLine("  DefaultWorkingDirectory - Default working directory for new sessions");
        },
        verboseOption,
        configOption);

        configCommand.AddCommand(setCommand);
        configCommand.AddCommand(getCommand);
        configCommand.AddCommand(listCommand);

        return configCommand;
    }

    /// <summary>
    /// Creates the session management command.
    /// </summary>
    /// <returns>Configured session command</returns>
    private static Command CreateSessionCommand(Option<bool> verboseOption, Option<string> configOption)
    {
        var sessionCommand = new Command("session", "Manage conversation sessions");

        var listCommand = new Command("list", "List all sessions");
        var createCommand = new Command("create", "Create a new session")
        {
            new Option<string>("--directory", "Working directory for the session")
        };
        var deleteCommand = new Command("delete", "Delete a session")
        {
            new Argument<string>("id", "Session ID to delete")
        };
        var showCommand = new Command("show", "Show session details")
        {
            new Argument<string>("id", "Session ID to show")
        };

        listCommand.SetHandler(async (bool verbose, string config) =>
        {
            var hostBuilder = CreateHostBuilder(Array.Empty<string>());
            var host = hostBuilder.Build();
            var apiService = host.Services.GetRequiredService<IApiService>();
            
            var response = await apiService.ListSessionsAsync();
            
            if (response.Sessions.Count == 0)
            {
                Console.WriteLine("No sessions found.");
                return;
            }

            Console.WriteLine($"Found {response.TotalCount} sessions:");
            Console.WriteLine();
            
            foreach (var session in response.Sessions)
            {
                var agentInfo = !string.IsNullOrEmpty(session.SubAgentId) 
                    ? $" [{session.SubAgentId}]" 
                    : "";
                
                Console.WriteLine($"  {session.Id}{agentInfo}");
                Console.WriteLine($"    Created: {session.Created:yyyy-MM-dd HH:mm:ss}");
                Console.WriteLine($"    Last accessed: {session.LastAccessed:yyyy-MM-dd HH:mm:ss}");
                Console.WriteLine($"    Conversation turns: {session.ConversationLength}");
                Console.WriteLine($"    Working directory: {session.WorkingDirectory}");
                Console.WriteLine();
            }
        },
        verboseOption,
        configOption);

        createCommand.SetHandler(async (string directory, bool verbose, string config) =>
        {
            var hostBuilder = CreateHostBuilder(Array.Empty<string>());
            var host = hostBuilder.Build();
            var apiService = host.Services.GetRequiredService<IApiService>();
            
            var session = await apiService.CreateSessionAsync(directory);
            
            Console.WriteLine($"Created new session: {session.Id}");
            Console.WriteLine($"Working directory: {session.WorkingDirectory}");
            Console.WriteLine($"Created: {session.Created:yyyy-MM-dd HH:mm:ss}");
        },
        new Option<string>("--directory"),
        verboseOption,
        configOption);

        deleteCommand.SetHandler(async (string id, bool verbose, string config) =>
        {
            var hostBuilder = CreateHostBuilder(Array.Empty<string>());
            var host = hostBuilder.Build();
            var apiService = host.Services.GetRequiredService<IApiService>();
            
            await apiService.DeleteSessionAsync(id);
            Console.WriteLine($"Deleted session: {id}");
        },
        new Argument<string>("id"),
        verboseOption,
        configOption);

        showCommand.SetHandler(async (string id, bool verbose, string config) =>
        {
            var hostBuilder = CreateHostBuilder(Array.Empty<string>());
            var host = hostBuilder.Build();
            var apiService = host.Services.GetRequiredService<IApiService>();
            
            var session = await apiService.GetSessionInfoAsync(id);
            
            if (session == null)
            {
                Console.WriteLine($"Session {id} not found.");
                return;
            }

            Console.WriteLine($"Session Details:");
            Console.WriteLine($"  ID: {session.Id}");
            Console.WriteLine($"  Created: {session.Created:yyyy-MM-dd HH:mm:ss}");
            Console.WriteLine($"  Last accessed: {session.LastAccessed:yyyy-MM-dd HH:mm:ss}");
            Console.WriteLine($"  Working directory: {session.WorkingDirectory}");
            Console.WriteLine($"  Conversation turns: {session.Conversation.Count}");
            
            if (!string.IsNullOrEmpty(session.SubAgentId))
            {
                Console.WriteLine($"  Sub-agent ID: {session.SubAgentId}");
            }
            
            if (!string.IsNullOrEmpty(session.ParentSessionId))
            {
                Console.WriteLine($"  Parent session: {session.ParentSessionId}");
            }
        },
        new Argument<string>("id"),
        verboseOption,
        configOption);

        sessionCommand.AddCommand(listCommand);
        sessionCommand.AddCommand(createCommand);
        sessionCommand.AddCommand(deleteCommand);
        sessionCommand.AddCommand(showCommand);

        return sessionCommand;
    }
}

/// <summary>
/// Placeholder for file system service interface.
/// </summary>
public interface IFileSystemService
{
    Task<string> ReadFileAsync(string path, CancellationToken cancellationToken = default);
    Task WriteFileAsync(string path, string content, CancellationToken cancellationToken = default);
}

/// <summary>
/// Placeholder for file system service implementation.
/// </summary>
public class FileSystemService : IFileSystemService
{
    public Task<string> ReadFileAsync(string path, CancellationToken cancellationToken = default)
    {
        // TODO: Implement with System.IO.Abstractions
        return File.ReadAllTextAsync(path, cancellationToken);
    }

    public Task WriteFileAsync(string path, string content, CancellationToken cancellationToken = default)
    {
        // TODO: Implement with System.IO.Abstractions
        return File.WriteAllTextAsync(path, content, cancellationToken);
    }
}

/// <summary>
/// Placeholder for configuration service interface.
/// </summary>
public interface IConfigurationService
{
    T GetValue<T>(string key, T defaultValue = default!);
    Task SetValueAsync<T>(string key, T value, CancellationToken cancellationToken = default);
    void SetValue<T>(string key, T value);
    Task<bool> HasKeyAsync(string key, CancellationToken cancellationToken = default);
    Task SaveAsync(CancellationToken cancellationToken = default);
}

/// <summary>
/// Placeholder for configuration service implementation.
/// </summary>
public class ConfigurationService : IConfigurationService
{
    private readonly Dictionary<string, object> _configuration = new();
    
    public T GetValue<T>(string key, T defaultValue = default!)
    {
        if (_configuration.TryGetValue(key, out var value) && value is T typedValue)
        {
            return typedValue;
        }
        return defaultValue;
    }

    public Task SetValueAsync<T>(string key, T value, CancellationToken cancellationToken = default)
    {
        _configuration[key] = value ?? throw new ArgumentNullException(nameof(value));
        return Task.CompletedTask;
    }
    
    public void SetValue<T>(string key, T value)
    {
        _configuration[key] = value ?? throw new ArgumentNullException(nameof(value));
    }

    public Task<bool> HasKeyAsync(string key, CancellationToken cancellationToken = default)
    {
        return Task.FromResult(_configuration.ContainsKey(key));
    }

    public Task SaveAsync(CancellationToken cancellationToken = default)
    {
        // TODO: Implement configuration persistence
        return Task.CompletedTask;
    }
}
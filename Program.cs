using Serilog;

Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .WriteTo.File("logs/myapp.txt", rollingInterval: RollingInterval.Day)
    .CreateLogger();

var builder = WebApplication.CreateBuilder(args);

// Hook Serilog into the host
builder.Host.UseSerilog();

var app = builder.Build();

// Global Exception Handling Middleware
app.Use(async (context, next) =>
{
    try
    {
        await next();
    }
    catch (Exception ex)
    {
        Log.Error(ex, "Global exception caught");
        
        context.Response.StatusCode = 500;
        await context.Response.WriteAsync("An unexpected error occurred. Please try again later.");
    }
});

// Test endpoint to trigger an error
app.MapGet("/trigger-error", () =>
{
    throw new InvalidOperationException("Simulated error for testing!");
});

app.MapGet("/", () => "Hello World! Try visiting /trigger-error");

app.Run();
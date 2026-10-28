// Fake Discord webhook endpoint for tests.
// POST /webhook/{name}   -> records the JSON body, returns 204 like Discord does
// GET  /messages         -> all recorded messages as a JSON array
// GET  /messages/search?q=text -> recorded messages whose JSON contains text (case-insensitive)
// DELETE /messages       -> clears recorded messages
// Optional: set FORWARD_URL to also forward every payload to a real Discord webhook.
using System.Collections.Concurrent;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;

var builder = WebApplication.CreateBuilder(args);
builder.Logging.SetMinimumLevel(LogLevel.Warning);
var app = builder.Build();

var messages = new ConcurrentQueue<JsonObject>();
var logPath = Environment.GetEnvironmentVariable("CAPTURE_LOG");
var forwardUrl = Environment.GetEnvironmentVariable("FORWARD_URL");
var http = new HttpClient();

app.MapPost("/webhook/{name}", async (string name, HttpRequest req) =>
{
    using var reader = new StreamReader(req.Body, Encoding.UTF8);
    var raw = await reader.ReadToEndAsync();
    JsonNode? body;
    try { body = JsonNode.Parse(raw); } catch { body = JsonValue.Create(raw); }

    var entry = new JsonObject
    {
        ["receivedAt"] = DateTime.UtcNow.ToString("o"),
        ["webhook"] = name,
        ["body"] = body
    };
    messages.Enqueue(entry);
    var line = entry.ToJsonString();
    Console.WriteLine(line);
    if (!string.IsNullOrEmpty(logPath)) await File.AppendAllTextAsync(logPath, line + Environment.NewLine);

    if (!string.IsNullOrEmpty(forwardUrl))
    {
        try { await http.PostAsync(forwardUrl, new StringContent(raw, Encoding.UTF8, "application/json")); }
        catch (Exception e) { Console.Error.WriteLine($"forward failed: {e.Message}"); }
    }
    return Results.NoContent();
});

app.MapGet("/messages", () => Results.Json(messages.ToArray()));
app.MapGet("/messages/search", (string q) =>
    Results.Json(messages.Where(m => m.ToJsonString().Contains(q, StringComparison.OrdinalIgnoreCase)).ToArray()));
app.MapDelete("/messages", () => { messages.Clear(); return Results.NoContent(); });
app.MapGet("/health", () => "ok");

app.Run(Environment.GetEnvironmentVariable("CAPTURE_URL") ?? "http://127.0.0.1:8787");

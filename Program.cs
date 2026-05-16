using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.Data.Sqlite;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

// Initialize database
DatabaseHelper.Initialize();

// Setup Semantic Kernel with Ollama
var kernel = Kernel.CreateBuilder()
    .AddOllamaChatCompletion(
        modelId: "llama3",
        endpoint: new Uri("http://localhost:11434")
    )
    .Build();

// ✅ FIXED ENDPOINT
app.MapGet("/ask", async (HttpContext context) =>
{
    var question = context.Request.Query["question"].ToString();

    var chat = kernel.GetRequiredService<IChatCompletionService>();

    string prompt = $@"
You are a strict SQL generator.

Rules:
- ONLY output valid SQLite SELECT query
- DO NOT include explanation
- ALWAYS use proper case (Apple, Samsung, etc.)
- Output ONLY SQL

Schema:
Table Sales(Id, Product, Amount, SaleDate)

Question: {question}

SQL:
";

    var result = await chat.GetChatMessageContentAsync(prompt);
    string sql = result.ToString();

// Extract only SELECT part
int index = sql.ToLower().IndexOf("select");
if (index != -1)
{
    sql = sql.Substring(index);
}

// Remove extra text after semicolon
int end = sql.IndexOf(";");
if (end != -1)
{
    sql = sql.Substring(0, end + 1);
}

    // Safety check
    if (!sql.ToLower().Contains("select"))
        return Results.BadRequest("Only SELECT queries allowed");

    using var conn = new SqliteConnection("Data Source=sales.db");
    conn.Open();

    var cmd = new SqliteCommand(sql, conn);
    var reader = cmd.ExecuteReader();

    var data = new List<string>();

    while (reader.Read())
    {
        data.Add(reader[0]?.ToString());
    }

    return Results.Ok(new
    {
        SQL = sql,
        Result = data
    });
});
app.UseDefaultFiles();
app.UseStaticFiles();
app.Run();
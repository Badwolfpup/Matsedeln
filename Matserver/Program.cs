using MatsedelnShared;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
// --- SERVICES SECTION ---
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        // This tells the serializer to keep track of objects it has already 
        // processed and use a reference ($id) instead of repeating them.
        options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;

        // Optional: Makes the JSON more readable in Swagger
        options.JsonSerializerOptions.WriteIndented = true;
    });
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddOpenApi();
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection"),
        // This is the key line:
        x => x.MigrationsAssembly("Matserver")
    ));

var app = builder.Build();

// --- PIPELINE SECTION ---

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/openapi/v1.json", "My Pet Project API v1");
    });
}

// 1. CORS should usually be early
app.UseCors("AllowWpfApp");

// 2. API KEY GATEKEEPER

//app.Use(async (context, next) =>
//{
//    // BYPASS: Let Swagger/OpenAPI through so you can actually see the UI!
//    var path = context.Request.Path.Value?.ToLower();
//    if (path != null && (path.Contains("swagger") || path.Contains("openapi")))
//    {
//        await next();
//        return;
//    }

//    // A. Check for header
//    if (!context.Request.Headers.TryGetValue("X-Api-Key", out var extractedApiKey))
//    {
//        context.Response.StatusCode = 401;
//        await context.Response.WriteAsync("API Key is missing.");
//        return;
//    }

//    // B. Get key from config (assuming "ApiKey" is in your appsettings.json)
//    var config = context.RequestServices.GetRequiredService<IConfiguration>();
//    var actualKey = config.GetValue<string>("ApiKey");

//    // C. Compare
//    if (string.IsNullOrEmpty(actualKey) || actualKey != extractedApiKey)
//    {
//        context.Response.StatusCode = 401;
//        await context.Response.WriteAsync("Unauthorized client.");
//        return;
//    }

//    await next();
//});

app.Use(async (context, next) =>
{
    var path = context.Request.Path.Value?.ToLower() ?? "";
    var isSwagger = path.Contains("swagger") || path.Contains("openapi");

    // NEW: Bypass security if we are in Development mode OR if it's Swagger
    if (app.Environment.IsDevelopment() || isSwagger)
    {
        await next();
        return;
    }

    // A. Check for header
    if (!context.Request.Headers.TryGetValue("X-Api-Key", out var extractedApiKey))
    {
        context.Response.StatusCode = 401;
        await context.Response.WriteAsync("API Key is missing.");
        return;
    }

    // B. Get key from config
    var config = context.RequestServices.GetRequiredService<IConfiguration>();
    var actualKey = config.GetValue<string>("ApiKey");

    // C. Compare
    if (string.IsNullOrEmpty(actualKey) || actualKey != extractedApiKey)
    {
        context.Response.StatusCode = 401;
        await context.Response.WriteAsync("Unauthorized client.");
        return;
    }

    await next();
});

app.UseHttpsRedirection();
// -------------------

app.UseAuthorization();
// 3. Routing and Endpoints
app.UseStaticFiles();
app.MapControllers();

app.Run();
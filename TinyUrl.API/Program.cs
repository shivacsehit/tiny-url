using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi;
using Serilog;
using TinyUrl.API.Data;
using TinyUrl.API.Helpers;
using TinyUrl.API.Models;

var builder = WebApplication.CreateBuilder(args);

// Serilog
Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .WriteTo.File("logs/app.log", rollingInterval: RollingInterval.Day)
    .CreateLogger();
builder.Host.UseSerilog();

// SQL Server
builder.Services.AddDbContext<AppDbContext>(opt =>
    opt.UseSqlServer(builder.Configuration
        .GetConnectionString("Default")));

// CORS
builder.Services.AddCors(o => o.AddDefaultPolicy(p =>
    p.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader()));

// Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Tiny URL API",
        Version = "v1"
    });
});

var app = builder.Build();

// Auto migrate
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider
        .GetRequiredService<AppDbContext>();
    db.Database.Migrate();
}

app.UseCors();

// Swagger — available in all environments
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Tiny URL API v1");
    c.RoutePrefix = "swagger";
});

// POST /api/add
app.MapPost("/api/add", async (
    TinyUrlAddDto dto,
    AppDbContext db,
    HttpContext ctx) =>
{
    string code;
    do { code = ShortCodeGenerator.Generate(); }
    while (await db.TinyUrls.AnyAsync(x => x.ShortCode == code));

    var entry = new TinyUrl.API.Models.TinyUrl
    {
        OriginalUrl = dto.Url,
        ShortCode = code,
        IsPrivate = dto.IsPrivate
    };
    db.TinyUrls.Add(entry);
    await db.SaveChangesAsync();

    var baseUrl = $"{ctx.Request.Scheme}://{ctx.Request.Host}";
    return Results.Ok(new { shortUrl = $"{baseUrl}/{code}", code });
});

// GET /api/public
app.MapGet("/api/public", async (string? search, AppDbContext db) =>
{
    var query = db.TinyUrls.Where(x => !x.IsPrivate);
    if (!string.IsNullOrEmpty(search))
        query = query.Where(x =>
            x.ShortCode.Contains(search) ||
            x.OriginalUrl.Contains(search));
    return await query
        .OrderByDescending(x => x.CreatedAt)
        .ToListAsync();
});

// GET /{code} - redirect
app.MapGet("/{code}", async (string code, AppDbContext db) =>
{
    var entry = await db.TinyUrls
        .FirstOrDefaultAsync(x => x.ShortCode == code);
    if (entry is null) return Results.NotFound();
    entry.Clicks++;
    await db.SaveChangesAsync();
    return Results.Redirect(entry.OriginalUrl);
});

// DELETE /api/delete/{code}
app.MapDelete("/api/delete/{code}", async (string code, AppDbContext db) =>
{
    var entry = await db.TinyUrls
        .FirstOrDefaultAsync(x => x.ShortCode == code);
    if (entry is null) return Results.NotFound();
    db.TinyUrls.Remove(entry);
    await db.SaveChangesAsync();
    return Results.Ok();
});

// DELETE /api/delete-all
app.MapDelete("/api/delete-all", async (AppDbContext db) =>
{
    db.TinyUrls.RemoveRange(db.TinyUrls);
    await db.SaveChangesAsync();
    return Results.Ok();
});

// PUT /api/update/{code}
app.MapPut("/api/update/{code}", async (
    string code,
    TinyUrlAddDto dto,
    AppDbContext db) =>
{
    var entry = await db.TinyUrls
        .FirstOrDefaultAsync(x => x.ShortCode == code);
    if (entry is null) return Results.NotFound();
    entry.OriginalUrl = dto.Url;
    entry.IsPrivate = dto.IsPrivate;
    await db.SaveChangesAsync();
    return Results.Ok(entry);
});

app.Run();

record TinyUrlAddDto(string Url, bool IsPrivate);
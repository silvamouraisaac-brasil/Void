using Microsoft.EntityFrameworkCore;
using VoidPass.Data;
using VoidPass.Services;

var builder = WebApplication.CreateBuilder(args);

// ============================================================
// DATABASE
// ============================================================

builder.Services.AddDbContext<VoidPassDbContext>(options =>
    options.UseNpgsql(
        builder.Configuration.GetConnectionString("DefaultConnection")));

// ============================================================
// SERVICES
// ============================================================

builder.Services.AddScoped<PasswordGenerator>();
builder.Services.AddScoped<PasswordHasher>();
builder.Services.AddScoped<LimboService>();

// ============================================================
// CORS
// ============================================================

builder.Services.AddCors(options =>
{
    options.AddPolicy("Frontend", policy =>
    {
        policy
            .WithOrigins(
                "https://seu-site-oficial.com",
                "http://localhost:5500",
                "http://127.0.0.1:5500")
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

var app = builder.Build();

// ============================================================
// MIDDLEWARE
// ============================================================

app.UseCors("Frontend");

// ============================================================
// HEALTH CHECK
// ============================================================

app.MapGet("/", () => "VOID v2 online");

// ============================================================
// PASSWORD GENERATOR + LIMBO
// ============================================================

app.MapGet("/api/v1/gerar-senha", async (
    int tamanho,
    LimboService limbo,
    CancellationToken cancellationToken) =>
{
    try
    {
        string senha = await limbo.GerarSenhaUnicaAsync(
            tamanho,
            cancellationToken);

        return Results.Ok(new
        {
            senha
        });
    }
    catch (ArgumentOutOfRangeException ex)
    {
        return Results.BadRequest(new
        {
            erro = ex.Message
        });
    }
});

// ============================================================
// START
// ============================================================

app.Run();
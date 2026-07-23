using Core.Abstractions;
using Infrastructure.Data;
using Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using SShopAPI.Endpoints;
using SShopAPI.Security;

DotNetEnv.Env.Load();

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException("ConnectionStrings:DefaultConnection is not configured.");

var connectionParts = new[]
{
    "SQL_SERVER_HOST",
    "SQL_SERVER_PORT",
    "SQL_SERVER_DB",
    "SQL_SERVER_USER",
    "SQL_SERVER_PASSWORD"
};

foreach (var variableName in connectionParts)
{
    var placeholder = "${" + variableName + "}";
    if (!connectionString.Contains(placeholder, StringComparison.Ordinal))
    {
        continue;
    }

    var value = builder.Configuration[variableName];
    if (string.IsNullOrWhiteSpace(value))
    {
        throw new InvalidOperationException($"Environment variable {variableName} is required by the database connection string.");
    }

    connectionString = connectionString.Replace(placeholder, value, StringComparison.Ordinal);
}

builder.Services.AddDbContext<SS_DbContext>(options =>
    options.UseSqlServer(connectionString));

builder.Services
    .AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme = ApiKeyAuthenticationOptions.Scheme;
        options.DefaultChallengeScheme = ApiKeyAuthenticationOptions.Scheme;
    })
    .AddScheme<ApiKeyAuthenticationOptions, ApiKeyAuthenticationHandler>(
        ApiKeyAuthenticationOptions.Scheme,
        options =>
        {
            options.ApiKey = builder.Configuration["Security:AdminApiKey"]
                ?? builder.Configuration["ADMIN_API_KEY"];
        });
builder.Services.AddAuthorization();
builder.Services.AddCors(options => options.AddPolicy("Client", policy =>
    policy.WithOrigins("http://localhost:4200")
        .AllowAnyHeader()
        .AllowAnyMethod()));
builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));

var app = builder.Build();

try
{
    using var scope = app.Services.CreateScope();
    var dbContext = scope.ServiceProvider.GetRequiredService<SS_DbContext>();
    dbContext.Database.Migrate();
    await SSContextSeed.SeedAsync(dbContext, app.Environment.ContentRootPath);
}
catch (Exception ex)
{
    Console.WriteLine($"An error occurred while preparing the database: {ex.Message}");
    throw;
}

app.UseHttpsRedirection();
app.UseCors("Client");
app.UseAuthentication();
app.UseAuthorization();

app.MapProductEndpoints();

app.Run();

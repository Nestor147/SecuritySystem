using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.OpenApi.Models;
using SecuritySystem.Infrastructure.DependencyInjection;

const string TARGET_ENV_TEST = "Test";
const string TARGET_ENV_PRODUCTION = "Production";

var builder = WebApplication.CreateBuilder(new WebApplicationOptions
{
    EnvironmentName = TARGET_ENV_TEST, // o usa ASPNETCORE_ENVIRONMENT si prefieres
    Args = args
});

var cfg = builder.Configuration;

// Infrastructure (Db, Jwt, HttpClients, etc.)
builder.Services.AddInfrastructure(cfg);
// builder.Services.AddAtacadoSecurity(cfg); // si aún lo usas

// CORS: lee orígenes desde appsettings: "Cors": { "Origins": [...] }
var origins = cfg.GetSection("Cors:Origins").Get<string[]>() ?? Array.Empty<string>();

builder.Services.AddCors(options =>
{
    options.AddPolicy("Frontends", policy =>
    {
        policy.WithOrigins(origins)
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials();
    });
});

// Controllers
builder.Services.AddControllers()
    .ConfigureApiBehaviorOptions(o =>
    {
        o.SuppressModelStateInvalidFilter = true;
    });

// Swagger (si AddInfrastructure no lo configura, esto lo asegura)
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Backend Atacado API",
        Version = "v1"
    });
});

var app = builder.Build();

var env = app.Environment;

// Manejo de errores según ambiente
if (env.IsDevelopment() || env.EnvironmentName == TARGET_ENV_TEST)
{
    app.UseDeveloperExceptionPage();
}
else
{
    app.UseExceptionHandler(errorApp =>
    {
        errorApp.Run(async context =>
        {
            var feature = context.Features.Get<IExceptionHandlerFeature>();
            context.Response.ContentType = "text/plain";
            await context.Response.WriteAsync(feature?.Error?.ToString() ?? "Unknown error");
        });
    });
}

// Swagger siempre disponible (o solo en Test/Dev si prefieres)
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Backend Atacado API V1");
    c.RoutePrefix = "swagger";
});

app.UseHttpsRedirection();
app.UseRouting();

// CORS debe ir después de UseRouting y antes de UseAuthorization
app.UseCors("Frontends");

app.UseAuthentication();
app.UseAuthorization();

// Endpoints
app.MapControllers();

// Root endpoint público
app.MapGet("/", [AllowAnonymous] () =>
    $"Backend Atacado OK ({app.Environment.EnvironmentName})");

app.Run();

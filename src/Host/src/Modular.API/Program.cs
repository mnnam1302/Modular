using Microsoft.AspNetCore.Diagnostics.HealthChecks;

var builder = WebApplication.CreateBuilder(args);

// Readiness checks are tagged "ready". Each module registers its own dependency checks
// (database, broker...) with that tag in its IModule.AddServices.
builder.Services.AddHealthChecks();

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

// No HTTPS redirection: TLS terminates at the ingress / load balancer and the container serves HTTP on 8080.

app.UseAuthorization();

// Liveness: is the process able to serve requests? Runs no checks, so a slow or down dependency never makes Kubernetes restart healthy pods.
app.MapHealthChecks("/health/live", new HealthCheckOptions
{
    Predicate = _ => false
}).DisableHttpMetrics();

// Readiness: can this instance take traffic right now? Runs every check tagged "ready".
app.MapHealthChecks("/health/ready", new HealthCheckOptions
{
    Predicate = check => check.Tags.Contains("ready")
}).DisableHttpMetrics();

app.MapControllers();

await app.RunAsync();

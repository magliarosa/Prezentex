using MediatR;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.OpenApi.Models;
using Prezentex.Api.Middleware;
using Prezentex.Api.Services;
using Prezentex.Api.Services.Identity;
using Prezentex.Api.Services.Notifications;
using System.Net.Mime;
using System.Reflection;
using System.Text.Json;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers(options =>
{
    options.SuppressAsyncSuffixInActionNames = false;
});
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.EnableAnnotations();
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the bearer scheme",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey
    });

    var security = new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                },
                Scheme = "oauth2",
                Name = "Bearer",
                In = ParameterLocation.Header
            },
            new string[]{ }
        }
    };
    c.AddSecurityRequirement(security);
});
builder.Services.AddMediatR(
    cfg => cfg.RegisterServicesFromAssemblies(Assembly.GetExecutingAssembly()));
builder.Services.AddAuth(builder.Configuration);
builder.Services.AddPersistence(builder.Configuration);
builder.Services.AddHttpClient();

builder.Services.AddScoped<INotificationsService,NotificationsService>();
builder.Services.AddScoped<IEventService>(
    x => new EventService(
        x.GetRequiredService<INotificationsService>(),
        x.GetRequiredService<IIdentityService>()));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseWhen(context => context.Request.Path.Equals("/auth/fb"),
    appBuilder => appBuilder.UseEvents());

app.UseHttpsRedirection();


app.UseRouting();

app.UseHttpErrorHandling();

app.UseAuthentication();
app.UseAuthorization();

app.UseEndpoints(endpoints =>
{
    endpoints.MapControllers();
    endpoints.MapHealthChecks("/health/ready", new HealthCheckOptions
    {
        Predicate = (check) => check.Tags.Contains("ready"),
        ResponseWriter = async (context, report) =>
        {
            var result = JsonSerializer.Serialize(
                new
                {
                    status = report.Status.ToString(),
                    checks = report.Entries.Select(entry =>
                    new
                    {
                        name = entry.Key,
                        status = entry.Value.Status.ToString(),
                        exception = entry.Value.Exception != null ? entry.Value.Exception.Message : null,
                        duration = entry.Value.Duration.ToString()
                    })
                });

            context.Response.ContentType = MediaTypeNames.Application.Json;
            await context.Response.WriteAsync(result);
        }

    });

    endpoints.MapHealthChecks("/health/live", new HealthCheckOptions
    {
        Predicate = (_) => false
    });
});

app.MapControllers();

app.Run();

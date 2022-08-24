using Microsoft.EntityFrameworkCore;
using Prezentex.Repositories;
using Prezentex.Settings;

var builder = WebApplication.CreateBuilder(args);
var postgresSettings = builder.Configuration.GetSection(nameof(PostgresSettings)).Get<PostgresSettings>();

// Add services to the container.
builder.Services.AddSingleton<IGiftsRepository, InMemGiftsRepository>();
builder.Services.AddControllers(options =>
{
    options.SuppressAsyncSuffixInActionNames = false;
});
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.EnableAnnotations();
});

builder.Services.AddHealthChecks();
builder.Services.AddDbContext<EntitiesDbContext>(
    options => options.UseNpgsql(postgresSettings.ConnectionString)
);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();


app.UseRouting();

app.UseAuthorization();

app.UseEndpoints(endpoints =>
{
    endpoints.MapControllers();
    endpoints.MapHealthChecks("/health");
});


app.MapControllers();

app.Run();

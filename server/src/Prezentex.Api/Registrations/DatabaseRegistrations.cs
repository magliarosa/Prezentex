using Microsoft.EntityFrameworkCore;
using Prezentex.Api.Settings;
using Prezentex.Application.Common.Interfaces.Repositories;
using Prezentex.Infrastructure.Persistence.Repositories;
using Prezentex.Infrastructure.Persistence.Repositories.Gifts;
using Prezentex.Infrastructure.Persistence.Repositories.Notifications;
using Prezentex.Infrastructure.Persistence.Repositories.Recipients;
using Prezentex.Infrastructure.Persistence.Repositories.Users;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class DatabaseRegistrations
    {
        public static IServiceCollection AddPersistence(
            this IServiceCollection services,
            IConfiguration config)
        {
            var postgresSettings = config.GetSection(nameof(PostgresSettings)).Get<PostgresSettings>();
            var cosmosSettings = config.GetSection(nameof(CosmosSettings)).Get<CosmosSettings>();

            services.AddDbContext<EntitiesDbContext>(
                options => options.UseNpgsql(postgresSettings.ConnectionString));

            services.AddDbContext<CosmosDbContext>(
                options => options.UseCosmos(cosmosSettings.ConnectionString, cosmosSettings.Database));

            services.AddScoped<IGiftsRepository, PostgresGitsRepository>();
            services.AddScoped<IRecipientsRepository, PostgresRecipientsRepository>();
            services.AddScoped<IUsersRepository, PostgresUsersRepository>();
            services.AddScoped<INotificationsRepository, CosmosNotificationsRepository>();

            services.AddHealthChecks()
                .AddNpgSql(
                postgresSettings.ConnectionString,
                name: "postgres",
                timeout: TimeSpan.FromSeconds(3),
                tags: new[] { "ready" })
                .AddCosmosDb(
                cosmosSettings.ConnectionString,
                cosmosSettings.Database,
                timeout: TimeSpan.FromSeconds(3),
                tags: new[] { "ready" });

            return services;
        }
    }
}

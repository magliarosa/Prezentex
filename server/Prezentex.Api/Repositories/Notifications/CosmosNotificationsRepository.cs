using Microsoft.EntityFrameworkCore;
using Prezentex.Api.Entities;

namespace Prezentex.Api.Repositories.Notifications
{
    public class CosmosNotificationsRepository : INotificationsRepository
    {
        private readonly CosmosDbContext _context;

        public CosmosNotificationsRepository(CosmosDbContext cosmosDbContext)
        {
            _context = cosmosDbContext;
            _context.Database.EnsureCreated();
        }

        public async Task CreateNotificationAsync(Notification notification)
        {
            await _context.AddAsync(notification);
            await _context.SaveChangesAsync();
        }

        public async Task<Notification> GetNotificationAsync(Guid id)
        {
            var notification = await _context.Notifications.Select(o => new Notification { Id = id }).SingleAsync();
            return await Task.FromResult(notification);
        }

        public async Task<IEnumerable<Notification>> GetNotificationsAsync()
        {
            var notifications = await _context.Notifications.Select(x => x).ToListAsync();
            return await Task.FromResult(notifications);
        }
    }
}

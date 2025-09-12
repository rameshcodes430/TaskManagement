using Microsoft.EntityFrameworkCore;
using TaskManagement.Api.Data;

namespace TaskManagement.Api.Services
{
    public class MigrationService : IHostedService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<MigrationService> _logger;
        public MigrationService(IServiceProvider serviceProvider, ILogger<MigrationService> logger)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
        }
        public async Task StartAsync(CancellationToken cancellationToken)
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                try
                {
                    _logger.LogInformation("Starting database migration...");
                    await dbContext.Database.MigrateAsync(cancellationToken);
                    _logger.LogInformation("Database migration completed successfully.");
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "An error occurred while migrating the database.");
                    throw; // Rethrow to prevent the application from starting
                }
            }
        }
        public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;

    }
}

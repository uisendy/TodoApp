using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using TodoAppApi.Interfaces;

namespace TodoAppApi.Infrastructure.Jobs
{
    public class TodoCleanupService : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<TodoCleanupService> _logger;
        private readonly TimeSpan _interval = TimeSpan.FromHours(24);

        public TodoCleanupService(IServiceProvider serviceProvider, ILogger<TodoCleanupService> logger, ITodoService todoService)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                using var scope = _serviceProvider.CreateScope();
                var todoService = scope.ServiceProvider.GetRequiredService<ITodoService>();

                try
                {
                    var deletedCount = await todoService.DeleteOldArchivedTodosAsync();
                    _logger.LogInformation($"{deletedCount} archived todos older than 30 days deleted at {DateTime.UtcNow}");
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Failed to delete old archived todos.");
                }

                await Task.Delay(_interval, stoppingToken);
            }
        }
    }
}

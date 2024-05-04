using AC_Jobs_API_Domian_Layer.Models;
using AC_Jobs_API_Repository_Layer.IRepository;
using AC_Jobs_API_Service_Layer.IService;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace AC_Contact_Services.BaseService
{
    public class AutomationBackgroundService : IHostedService, IDisposable, IAutomationBackgroundService
    {

        private readonly IServiceProvider _serviceProvider;

        public AutomationBackgroundService(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        private Timer _timer;

        public Task StartAsync(CancellationToken cancellationToken)
        {

            _timer = new Timer(async state => await ExecuteAsync(state), null, TimeSpan.Zero, TimeSpan.FromMinutes(1));

            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {

            _timer?.Change(Timeout.Infinite, 0);

            return Task.CompletedTask;
        }

        public void Dispose()
        {
            _timer?.Dispose();
        }

        private async Task ExecuteAsync(object state)
        {
            try
            {
                using var scope = _serviceProvider.CreateScope();
                var repositoryFactory = scope.ServiceProvider.GetRequiredService<IRepositoryFactory>();

                var entityRepository = repositoryFactory.GetRepository<AutomationEntity>();
                var now = DateTime.Now;
                var next10Minutes = now.AddMinutes(10);

                // Query the entities
                var entities = await entityRepository.GetAllAsync()
                    .Where(a =>
                        a.TriggerType.ToLower() == "timebased" &&
                        a.IsActive == true &&
                        a.NextExecutionTime.HasValue &&
                        a.NextExecutionTime >= now &&
                        a.NextExecutionTime <= next10Minutes &&
                        a.IsDeleted != true)
                    .ToListAsync();


                // Process entities asynchronously using TPL
                var tasks = entities.Select(entity => ProcessEntityAsync(entity));
                await Task.WhenAll(tasks);
            }
            catch (Exception ex)
            {
                // Handle exceptions
            }
        }

        private async Task ProcessEntityAsync(AutomationEntity entity)
        {
            try
            {
                // Process the entity asynchronously

            }
            catch (Exception ex)
            {
                // Handle exceptions
            }
        }


        public void RestartService()
        {
            _timer?.Change(Timeout.Infinite, 0);
            _timer = new Timer(async state => await ExecuteAsync(state), null, TimeSpan.Zero, TimeSpan.FromMinutes(1));
        }

    }

}

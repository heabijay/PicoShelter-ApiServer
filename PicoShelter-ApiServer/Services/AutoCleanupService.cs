using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using PicoShelter_ApiServer.BLL.Interfaces;
using PicoShelter_ApiServer.DAL.Interfaces;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace PicoShelter_ApiServer.Services
{
    public class AutoCleanupService : IHostedService, IDisposable
    {
        ILogger<AutoCleanupService> _logger;
        Timer timer;
        IServiceScope scope;
        public AutoCleanupService(ILogger<AutoCleanupService> logger, IServiceProvider serviceProvider)
        {
            _logger = logger;
            scope = serviceProvider.CreateScope();
        }

        public void Dispose()
        {
            timer?.Dispose();
        }

        public void DoWork(object obj)
        {
            try
            {
                var db = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();
                var _imageService = scope.ServiceProvider.GetRequiredService<IImageService>();

                // Images auto-delete task
                var now = DateTime.UtcNow;
                var imageCodes = db.Images.Where(t => t.DeleteIn <= now).Select(t => t.ImageCode).ToList();
                _logger.LogInformation($"Cleanup task running: {imageCodes.Count()} images to delete");

                foreach (var code in imageCodes)
                {
                    _imageService.ForceDeleteImage(code);
                }


                // Delete outdated confirmations
                var confirmationIds = db.Confirmations.Where(t => t.ValidUntilUTC < now).Select(t => t.Id).ToList();
                _logger.LogInformation($"Cleanup task running: {confirmationIds.Count()} confirmations to delete");

                foreach (var id in confirmationIds)
                {
                    db.Confirmations.Delete(id);
                }

                db.Save();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Exception while running cleanup");
            }
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Cleanup service running.");

            timer = new Timer(DoWork, null, TimeSpan.Zero, TimeSpan.FromDays(1));

            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Cleanup service is stopping.");

            timer?.Dispose();

            return Task.CompletedTask;
        }
    }
}

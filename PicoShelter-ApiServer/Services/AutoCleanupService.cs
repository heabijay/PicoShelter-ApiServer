using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using PicoShelter_ApiServer.BLL.Interfaces;
using PicoShelter_ApiServer.DAL.Interfaces;
using PicoShelter_ApiServer.FDAL.Interfaces;
using System;
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

        private void DoWork(object obj)
        {
            var db = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();
            var _imageService = scope.ServiceProvider.GetRequiredService<IImageService>();

            // Images auto-delete task

            var now = DateTime.UtcNow;
            var images = db.Images.Where(t => t.DeleteIn <= now);
            _logger.LogInformation($"Cleanup task running: {images.Length} images to delete");

            foreach (var image in images)
            {
                _imageService.ForceDeleteImage(image.ImageCode);
            }


            // Delete outdated confirmations
            var confirmations = db.Confirmations.Where(t => t.ValidUntilUTC < now);
            _logger.LogInformation($"Cleanup task running: {images.Length} confirmations to delete");
            
            foreach (var confirm in confirmations)
            {
                db.Confirmations.Delete(confirm.Id);
            }

            db.Save();
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

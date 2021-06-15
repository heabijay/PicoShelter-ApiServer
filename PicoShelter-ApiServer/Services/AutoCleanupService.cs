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
        private readonly ILogger<AutoCleanupService> _log;
        private readonly IServiceScope _scope;
        private Timer _timer;

        public AutoCleanupService(ILogger<AutoCleanupService> logger, IServiceProvider serviceProvider)
        {
            _log = logger;
            _scope = serviceProvider.CreateScope();
        }

        public void Dispose()
        {
            _timer?.Dispose();
        }

        public void DoWork(object obj)
        {
            try
            {
                var db = _scope.ServiceProvider.GetRequiredService<IUnitOfWork>();
                var _imageService = _scope.ServiceProvider.GetRequiredService<IImageService>();

                // Images auto-delete task
                var now = DateTime.UtcNow;
                var imageCodes = db.Images.Where(t => t.DeleteIn <= now).Select(t => t.ImageCode).ToList();
                _log.LogInformation($"Cleanup task running: {imageCodes.Count} images to delete");

                foreach (var code in imageCodes)
                {
                    _imageService.ForceDeleteImage(code);
                }


                // Delete outdated confirmations
                var confirmationIds = db.Confirmations.Where(t => t.ValidUntilUTC < now).Select(t => t.Id).ToList();
                _log.LogInformation($"Cleanup task running: {confirmationIds.Count} confirmations to delete");

                foreach (var id in confirmationIds)
                {
                    db.Confirmations.Delete(id);
                }

                db.Save();
            }
            catch (Exception ex)
            {
                _log.LogError(ex, "Exception while running cleanup");
            }
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _log.LogInformation("Cleanup service running.");

            _timer = new Timer(DoWork, null, TimeSpan.Zero, TimeSpan.FromDays(1));

            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _log.LogInformation("Cleanup service is stopping.");

            _timer?.Dispose();

            return Task.CompletedTask;
        }
    }
}

using Quartz;
using System;
using System.Threading;

namespace TaskManager.Services.Impl
{
    public class TimerService : BackgroundService
    {
        private readonly ILogger<TimerService> _logger;
        private readonly IServiceScopeFactory _scopeFactory;

        public TimerService(ILogger<TimerService> logger, IServiceScopeFactory scopeFactory)
        {
            _logger = logger;
            _scopeFactory = scopeFactory;
        }

        // her gece 12 de çalışıp job servisinin UpdateExpireJobs ve MustInProgress methodlarını çalıştırır
        // expired entities tamamlanma süresi geçmiş ve tamamlanmamış görevleri expired olarak işaretler
        // bunu sayesinde günlük olarak tamamlamayı unuttuğu görevleri görebilir

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                _logger.LogInformation("TimerService executing...");

                var scope = _scopeFactory.CreateScope();
                IJobService _jobService = scope.ServiceProvider.GetRequiredService<IJobService>();
                int expiredEntities = await _jobService.UpdateExpiredJobs();
                int inProgressedEntities = await _jobService.MustInProgress();


                _logger.LogInformation("Committed to InProgress = " + inProgressedEntities);
                _logger.LogInformation("Expired entities = " + expiredEntities);

                DateTime now = DateTime.Now;
                DateTime nextMidnight = now.AddDays(1).Date;
                TimeSpan timeUntilNextMidnight = nextMidnight - now;
                int millisecondsUntilNextMidnight = (int)timeUntilNextMidnight.TotalMilliseconds;
                Console.WriteLine("next update (in milliseconds)-> " + millisecondsUntilNextMidnight);
                await Task.Delay(millisecondsUntilNextMidnight, stoppingToken);
            }
        }

    }
}

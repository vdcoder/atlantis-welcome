using Microsoft.Extensions.Hosting;

namespace Atlantis.Api.Citizens.Runtime
{
    public sealed class CitizenHostedService : BackgroundService
    {
        private readonly CitizenRuntime _runtime;
        private readonly ILogger<CitizenHostedService> _logger;
        private readonly TimeSpan _interval = TimeSpan.FromSeconds(5);

        public CitizenHostedService(
            CitizenRuntime runtime,
            ILogger<CitizenHostedService> logger)
        {
            _runtime = runtime;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Citizen agent loop started.");

            var timer = new PeriodicTimer(_interval);

            try
            {
                while (await timer.WaitForNextTickAsync(stoppingToken))
                {
                    try
                    {
                        // For now, run the first citizen (Orestes by ID)
                        // This can be extended to iterate over all autonomous citizens
                        await _runtime.RunOneIterationAsync(
                            "orestes",
                            stoppingToken);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(
                            ex,
                            "Error during citizen agent iteration.");
                    }
                }
            }
            finally
            {
                timer.Dispose();
                _logger.LogInformation("Citizen agent loop stopped.");
            }
        }
    }
}

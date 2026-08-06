using Microsoft.Extensions.Hosting;

namespace Atlantis.Api.Citizens.Runtime
{
    public sealed class CitizenHostedService : BackgroundService
    {
        private readonly IServiceScopeFactory
        _scopeFactory;

        private readonly ILogger<CitizenHostedService>
            _logger;

        private readonly TimeSpan _interval =
            TimeSpan.FromSeconds(5);

        public CitizenHostedService(
            IServiceScopeFactory scopeFactory,
            ILogger<CitizenHostedService> logger)
        {
            _scopeFactory = scopeFactory;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(
            CancellationToken stoppingToken)
        {
            _logger.LogInformation(
                "\x1b[35mCitizen\x1b[0m agent loop started.");

            using var timer =
                new PeriodicTimer(
                    _interval);

            try
            {
                while (await timer.WaitForNextTickAsync(
                           stoppingToken))
                {
                    try
                    {
                        await using var scope =
                            _scopeFactory
                                .CreateAsyncScope();

                        var runtime =
                            scope.ServiceProvider
                                .GetRequiredService<
                                    CitizenRuntime>();

                        await runtime.RunOneIterationAsync(
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
                _logger.LogInformation(
                    "\x1b[35mCitizen\x1b[0m agent loop stopped.");
            }
        }
    }
}

namespace ScraperService;

using System;

public class Worker : BackgroundService
{
      private readonly ILogger _logger;
      private static readonly CancellationTokenSource cts = new();

      public Timer TaskTimer { get; }

      public static event EventHandler? LaunchScraper;


      public Worker(ILogger<Worker> logger)
      {
            _logger ??= logger;
            this.TaskTimer = new Timer(OnTimerTick, null, TimeSpan.FromSeconds(30), TimeSpan.FromMinutes(10));

      }



      protected override async Task ExecuteAsync(CancellationToken stoppingToken)
      {
            _ = stoppingToken.Register(OnServiceCancelled);
            LaunchScraper += OnLaunchScraperAsync;

            while (!stoppingToken.IsCancellationRequested)
            {
                  //   _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
                  await Task.Delay(500, stoppingToken);
            }
      }






      private void OnLaunchScraperAsync(object? sender, EventArgs e)
      {
            PageScraper ps = new(_logger);
            try
            {
                  Task tsk = Task.Factory.StartNew(() => ps.BeginSiteScrapeAsync(cts.Token));
            }
            catch
            {
_logger.UnknownGeneralError();

            }
            finally
            {
                  ps.Dispose();
            }


      }

      public static void TurnTimerOff()
      {

      }




      /// <summary>
      /// 
      /// </summary>
      /// <param name="state"></param>
      public void OnTimerTick(object? state) => LaunchScraper?.Invoke(this, new EventArgs());




      private void OnServiceCancelled()
      {
            _logger.LogWarning("Service Recieved a stop signal - Cancelling running events.");
            cts.Cancel();


      }








}

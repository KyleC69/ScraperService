using ScraperService;
using ScraperService.Data;

IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureServices((context, services) =>
    {
        services.AddLogging();
          services.AddHostedService<Worker>();
    })
    .Build();


await host.RunAsync();

using ScraperService;
using ScraperService.Data;

IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureServices((context, services) =>
    {
          services.AddHostedService<Worker>();
          services.AddTransient<IDataController, DataController>();
    })
    .Build();


await host.RunAsync();

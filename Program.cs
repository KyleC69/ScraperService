using ScraperService;
using ScraperService.Data;








IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureServices((context, services) =>
    {
        services.AddLogging();
        services.AddHostedService<Worker>();
        services.AddDbContext<LinkDBContext>();
    })
    .Build();


await host.RunAsync();

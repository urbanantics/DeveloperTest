using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Threading;
using System.Threading.Tasks;



class Program
{

    static async Task Main(string[] args)
    {
        var host = CreateHostBuilder(args).Build();
        await host.RunAsync();
    }

    static IHostBuilder CreateHostBuilder(string[] args) =>
        Host.CreateDefaultBuilder(args)
            .ConfigureServices((hostContext, services) =>
            {
                services.AddScoped<IPaymentService, PaymentService>();
                services.AddTransient<BacsPaymentValidator>();
                services.AddTransient<FasterPaymentsValidator>();
                services.AddTransient<ChapsPaymentValidator>();
                services.AddHostedService<App>();
            });
}

public class App : IHostedService
{
    int __worker_Count = 10;
    private readonly IServiceProvider _serviceProvider;

    public App(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        using var scope = _serviceProvider.CreateScope();
        var services = scope.ServiceProvider;

        Console.WriteLine("Hello World");

    }

    public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
}
using Arrow.DeveloperTest.Services;
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
                services.AddTransient<IAccountDataStore, MockAccountDataStore>();
                services.AddScoped<IPaymentService, PaymentService>();
                services.AddScoped<IReporter, ConsoleReporter>();
                services.AddSingleton<IPaymentServiceFactory, PaymentServiceFactory>();
                services.AddTransient<BacsPaymentValidator>();
                services.AddTransient<FasterPaymentsValidator>();
                services.AddTransient<ChapsPaymentValidator>();
                services.AddTransient<IPaymentSimulator, PaymentSimulator>();
                services.AddHostedService<App>();
            });
}

public class App : IHostedService
{
    private readonly IServiceProvider _serviceProvider;

    public App(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        using var scope = _serviceProvider.CreateScope();
        var services = scope.ServiceProvider;

        var paymentSimulator = services.GetRequiredService<IPaymentSimulator>();
        await paymentSimulator.RunSimulation(Environment.ProcessorCount, 10000, cancellationToken);
    }

    public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
}
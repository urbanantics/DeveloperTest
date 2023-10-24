using Arrow.DeveloperTest.Services;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;

/// <summary>
/// Create Payment Service with fixed list of validators using .NET DI 
/// </summary>
public class PaymentServiceFactory : IPaymentServiceFactory
{
    private readonly IServiceProvider services;

    public PaymentServiceFactory(IServiceProvider services)
    {
        this.services = services;
    }

    /// <summary>
    /// Create instance of Payment Service for a specific Partition Key
    /// </summary>
    /// <param name="partitionKey"></param>
    /// <returns></returns>
    public IPaymentService CreateInstance(int partitionKey)
    {
        var datastore = new MockAccountDataStore(partitionKey);

        var validators = new Dictionary<PaymentScheme, IPaymentValidator>
            {
                { PaymentScheme.Bacs, services.GetService<BacsPaymentValidator>() },
                { PaymentScheme.FasterPayments, services.GetService<FasterPaymentsValidator>() },
                { PaymentScheme.Chaps, services.GetService<ChapsPaymentValidator>() }
            };

        return ActivatorUtilities.CreateInstance<PaymentService>(services,
            new object[] { datastore, validators });
    }
}
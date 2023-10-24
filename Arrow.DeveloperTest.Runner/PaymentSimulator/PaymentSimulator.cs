using Arrow.DeveloperTest.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Concurrency;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reactive.Threading.Tasks;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Transactions;

/// <summary>
/// Simulate Payments
/// </summary>
public class PaymentSimulator : IPaymentSimulator
{
    private readonly IPaymentServiceFactory _paymentServiceFactory;
    private readonly IReporter _reporter;

    /// <summary>
    /// Create instance of Payment Simulator
    /// </summary>
    /// <param name="paymentServiceFactory">Factory object that creates a Payment Service Instance</param>
    /// <param name="reporter">Reporter that will output Payment Service progress and final report</param>
    public PaymentSimulator(IPaymentServiceFactory paymentServiceFactory, IReporter reporter)
    {
        this._paymentServiceFactory = paymentServiceFactory;
        this._reporter = reporter;
    }

    /// <summary>
    /// Run Payment Simulator
    /// </summary>
    /// <param name="workerCount">Number of Workers to create</param>
    /// <param name="transactionCount">Number of Transaction to simulate</param>
    /// <param name="cancellationToken">Cancellation token to force stop Simulation</param>
    /// <returns></returns>
    public async Task RunSimulation(int workerCount, int transactionCount, CancellationToken cancellationToken)
    {

        var paymentServiceWorkers = new Dictionary<int, IPaymentService>();

        // Create Workers
        for (int i = 0; i < workerCount; i++)
        {
            paymentServiceWorkers[i] = _paymentServiceFactory.CreateInstance(i);
        }

        // Use reactive extensions to simulate multiple workers running on multiple threads
        var report = await Observable.Create(async (IObserver<int> observer) =>
        {
            var ids = Enumerable.Range(1, transactionCount).ToArray();

            await Task.WhenAll(ids.Select(i => Task.Run(() => { observer.OnNext(i); }, cancellationToken)));

            observer.OnCompleted();

            return Disposable.Create(() => Console.WriteLine("Observer has unsubscribed"));
        })
            // Based on list of ids from 0 to $transactionCount, generate some random payments ..
            .Select(GenerateRandomPayment(workerCount, paymentServiceWorkers))
            // Buffer results and generate console output
            .Buffer(TimeSpan.FromSeconds(0.5))
            .Aggregate(new Report()
            {
                TotalTransactions = transactionCount,
                Threads = new List<int>(),
                workers = new Dictionary<int, List<Transaction>>()
            }, (report, tList) =>
            {
                UpdateReport(report, tList);

                _reporter.GenerateUpdate(report);

                return report;
            })
            .ToTask();

        _reporter.GenerateFinalReport(report);
    }

    /// <summary>
    /// Update report based on current execution metrics
    /// </summary>
    /// <param name="report">Overall status report</param>
    /// <param name="metrics">Current progress metrics</param>
    private static void UpdateReport(Report report, IList<Tuple<int, int, string, MakePaymentResult>> metrics)
    {
        foreach (var t in metrics)
        {
            report.Threads.Add(t.Item1);
            report.Threads = report.Threads.Distinct().ToList();
            if (!report.workers.ContainsKey(t.Item2)) report.workers[t.Item2] = new List<Transaction>();
            report.workers[t.Item2].Add(new Transaction(t.Item3, t.Item4.Success));
        }
    }

    /// <summary>
    /// Generate Random Payment and assign to Worker
    /// </summary>
    /// <param name="workerCount">total number of workers</param>
    /// <param name="paymentServiceWorkers">list of active payment service workers</param>
    /// <returns></returns>
    private static Func<int, Tuple<int, int, string, MakePaymentResult>> GenerateRandomPayment(int workerCount, Dictionary<int, IPaymentService> paymentServiceWorkers)
    {
        return (i) =>
        {
            // partition transation to worker based on $i
            var worker = i % workerCount;

            // generate some random data
            var rand = new Random();
            var size = rand.Next(100);
            var accountType = $"{rand.Next(3) + 1}";
            var accountNo = $"{rand.Next(5) + 1}";
            var paymentScheme = (PaymentScheme)Enum.ToObject(typeof(PaymentScheme), rand.Next(3));
            var request = new MakePaymentRequest
            {
                Amount = size,
                CreditorAccountNumber = "00000",
                DebtorAccountNumber = $"{accountType}00{accountNo}",
                PaymentDate = DateTime.Now,
                PaymentScheme = paymentScheme,
            };

            var result = paymentServiceWorkers[worker].MakePayment(request);
            return Tuple.Create(Thread.CurrentThread.ManagedThreadId, worker, $"{accountType}00{accountNo}", result);
        };
    }
}

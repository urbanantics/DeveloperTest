using System;
using System.Linq;

/// <summary>
/// Reports Payment Simulator output to console
/// </summary>
public class ConsoleReporter : IReporter
{
    /// <summary>
    /// Generate Payment Simulator Progress report
    /// </summary>
    /// <param name="report"></param>
    public void GenerateUpdate(Report report)
    {
        Console.Clear();
        var overAll = 0;

        Console.WriteLine("Simulation in Progress ..");
        Console.WriteLine();
        report.workers.ToList().OrderBy((x) => x.Key).ToList().ForEach(worker =>
        {
            float successCount = worker.Value.Where(x => x.Success).Count();
            float total = worker.Value.Count();
            overAll += (int)total;
            Console.WriteLine($"Worker: {worker.Key}, transactions: {worker.Value.Count()}, successRatio: {100 * successCount / total}% ");
        });
        Console.WriteLine();
        Console.WriteLine($"[{overAll} transactions executed out of {report.TotalTransactions}]");

    }

    /// <summary>
    /// Generate Payment Simulator Final report
    /// </summary>
    /// <param name="report"></param>
    public void GenerateFinalReport(Report report)
    {
        Console.Clear();

        Console.WriteLine($"Simulation Compete. Transactions: {report.TotalTransactions}");
        Console.WriteLine();

        report.workers.SelectMany(w => w.Value).GroupBy(t => t.account).ToList().ForEach(account =>
        {
            float total = account.Count();
            float successCount = account.Where(x => x.Success).Count();

            Console.WriteLine($"Account: {account.Key}, transactions: {total}, successRatio: {100 * successCount / total}% ");
        });
    }
}

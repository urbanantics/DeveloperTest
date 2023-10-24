using System.Threading;
using System.Threading.Tasks;
public interface IPaymentSimulator
{
    Task RunSimulation(int workerCount, int transactions, CancellationToken cancellationToken);
}
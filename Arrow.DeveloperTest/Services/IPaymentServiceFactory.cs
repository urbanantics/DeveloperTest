namespace Arrow.DeveloperTest.Services
{
    public interface IPaymentServiceFactory
    {
        IPaymentService CreateInstance(int partition);
    }
}
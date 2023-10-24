using Arrow.DeveloperTest.Types;

namespace Arrow.DeveloperTest.Services
{
    public interface IPaymentService
    {
        MakePaymentResult MakePayment(MakePaymentRequest request);
    }
}

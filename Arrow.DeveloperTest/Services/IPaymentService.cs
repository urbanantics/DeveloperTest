public interface IPaymentService
{
    MakePaymentResult MakePayment(MakePaymentRequest request);
}
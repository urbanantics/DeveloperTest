public interface IPaymentValidator
{
    bool Validate(Account account, MakePaymentRequest request);
}

using Arrow.DeveloperTest.Types;

namespace Arrow.DeveloperTest.PaymentValidators
{
    public interface IPaymentValidator
    {
        bool Validate(Account account, MakePaymentRequest request);
    }
}

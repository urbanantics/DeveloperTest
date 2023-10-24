using Arrow.DeveloperTest.Types;

namespace Arrow.DeveloperTest.PaymentValidators
{
    /// <summary>
    /// Business validation logic for Bacs Payments
    /// </summary>
    public class BacsPaymentValidator : IPaymentValidator
    {
        /// <summary>
        /// Execute validation logic for Bacs Payment
        /// </summary>
        /// <param name="account">Account for which Bacs payment should execute</param>
        /// <param name="request">Payment request details</param>
        /// <returns>true, if payment request is valid for account</returns>
        public bool Validate(Account account, MakePaymentRequest request)
        {
            return account != null && account.AllowedPaymentSchemes.HasFlag(AllowedPaymentSchemes.Bacs);
        }
    }
}

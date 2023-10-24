using Arrow.DeveloperTest.Types;

namespace Arrow.DeveloperTest.PaymentValidators
{
    /// <summary>
    /// Business validation logic for Faster Payments
    /// </summary>
    public class FasterPaymentsValidator : IPaymentValidator
    {
        /// <summary>
        /// Execute validation logic for Faster Payment
        /// </summary>
        /// <param name="account">Account for which Faster payment should execute</param>
        /// <param name="request">Payment request details</param>
        /// <returns>true, if payment request is valid for account</returns>
        public bool Validate(Account account, MakePaymentRequest request)
        {
            return 
                account != null && 
                account.AllowedPaymentSchemes.HasFlag(AllowedPaymentSchemes.FasterPayments) && 
                account.Balance >= request.Amount;
        }
    }
}

/// <summary>
/// Business validation logic for Chaps Payments
/// </summary>
public class ChapsPaymentValidator : IPaymentValidator
{
    /// <summary>
    /// Execute validation logic for Chaps Payment
    /// </summary>
    /// <param name="account">Account for which Chaps payment should execute</param>
    /// <param name="request">Payment request details</param>
    /// <returns>true, if payment request is valid for account</returns>
    public bool Validate(Account account, MakePaymentRequest request)
    {
        return
            account != null &&
            request.Amount > 0 &&
            account.AllowedPaymentSchemes.HasFlag(AllowedPaymentSchemes.Chaps) &&
            account.Status == AccountStatus.Live;
    }
}
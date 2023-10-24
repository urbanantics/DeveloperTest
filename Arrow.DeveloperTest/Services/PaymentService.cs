
using System;
using System.Collections.Generic;

/// <summary>
/// Payment Service resposible for making payments against account
/// </summary>
public class PaymentService : IPaymentService
{
    private readonly IAccountDataStore _accountDataStore;
    private readonly IDictionary<PaymentScheme, IPaymentValidator> _paymentValidators;

    public PaymentService(IAccountDataStore accountDataStore, IDictionary<PaymentScheme, IPaymentValidator> paymentValidators)
    {
        _accountDataStore = accountDataStore ?? throw new ArgumentNullException(nameof(accountDataStore));
        _paymentValidators = paymentValidators ?? throw new ArgumentNullException(nameof(paymentValidators));
    }

    /// <summary>
    /// Make payment based on MakePaymentRequest
    /// </summary>
    /// <param name="request">Payment Request details</param>
    /// <returns></returns>
    /// <exception cref="ArgumentNullException"></exception>
    public MakePaymentResult MakePayment(MakePaymentRequest request)
    {
        if (request == null) throw new ArgumentNullException(nameof(request));

        var account = _accountDataStore.GetAccount(request.DebtorAccountNumber);
        var result = new MakePaymentResult { Success = false };

        if (_paymentValidators.TryGetValue(request.PaymentScheme, out var validator) && validator.Validate(account, request))
        {
            account.Balance -= request.Amount;
            try
            {
                _accountDataStore.UpdateAccount(account);
            }
            catch (Exception)
            {
                // todo: log exception
                return result;
            }

            result.Success = true;
        }

        return result;
    }
}

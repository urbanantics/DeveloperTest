# Arrow Developer Test Instructions

In the 'PaymentService.cs' file you will find a method for making a payment. At a high level the steps for making a payment are:

 1. Lookup the account the payment is being made from.
 2. Check that the account is in a valid state to make the payment.
 3. Deduct the payment amount from the account�s balance and update the account in the database.

What we�d like you to do is refactor the code with the following things in mind:

 - Adherence to SOLID principals
 - Testability
 - Readability

We�d also like you to add some unit tests to the Arrow.DeveloperTest.Tests project to show how you would test the code that you�ve produced and run the PaymentService from the Arrow.DeveloperTest.Runner console application.

The only specific 'rules' are:

- The solution should build
- The tests should all pass
- You should **not** change the method signature of the MakePayment method.

You are free to use any frameworks/NuGet packages that you see fit. You should plan to spend around an hour completing the exercise.

## My Assumptions
- Payment Service actions account Debit leg only
- Business Logic as currently defined in Payment Service is correct except for *a few changes i.e:

    - ## Bacs Payment valid if:
    - Account AllowedPaymentSchemes includes Bacs
    - Account balance after payment is positive, negative or zero
    - * Payment Request is a positive amount 
    - AccountStatus = Live | Disabled | InboundPaymentsOnly, i.e. AccountStatus is not validated
    - ## FasterPayments Payment valid if:
    - Account AllowedPaymentSchemes includes FasterPayments
    - Account balance after payment is positive *or zero
    - * Payment Request is a positive amount
    - AccountStatus = Live | Disabled | InboundPaymentsOnly, i.e. AccountStatus is not validated
    - ## Chaps Payment valid if:
    - Account AllowedPaymentSchemes includes Chaps
    - Account balance after payment is positive, negative or zero
    - * Payment Request is a positive amount
    - AccountStatus = Live 
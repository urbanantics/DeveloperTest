using FluentAssertions;
using Moq;
using Xunit;

public class ChapsPaymentValidatorTests
{
    private Mock<IAccountDataStore> _accountDataStoreMock;
    private PaymentService _paymentService;
    private readonly Dictionary<PaymentScheme, IPaymentValidator> _paymentValidators;

    public ChapsPaymentValidatorTests()
    {
        this._paymentValidators = new Dictionary<PaymentScheme, IPaymentValidator>
            {
                { PaymentScheme.Bacs, new BacsPaymentValidator() },
                { PaymentScheme.FasterPayments, new FasterPaymentsValidator() },
                { PaymentScheme.Chaps, new ChapsPaymentValidator() }
            };
    }

    [Fact]
    public void Given_Chaps_Payment_When_MakePayment_Should_Return_Success_For_Valid_Amount()
    {
        // Arrange
        var request = new MakePaymentRequest
        {
            DebtorAccountNumber = "12345",
            PaymentScheme = PaymentScheme.Chaps,
            Amount = 100
        };
        var account = new Account
        {
            AccountNumber = "12345",
            Balance = 200,
            AllowedPaymentSchemes = AllowedPaymentSchemes.Chaps
        };

        _accountDataStoreMock = new Mock<IAccountDataStore>();
        _paymentService = new PaymentService(_accountDataStoreMock.Object, this._paymentValidators);
        _accountDataStoreMock.Setup(x => x.GetAccount(request.DebtorAccountNumber)).Returns(account);

        // Act
        var result = _paymentService.MakePayment(request);

        // Assert
        result.Success.Should().BeTrue();
        account.Balance.Should().Be(100);
    }

    [Fact]
    public void Given_AccountStatus_Is_Disabled_When_Chaps_Payment_Should_Return_Fail_For_Valid_Amount()
    {
        // Arrange
        var request = new MakePaymentRequest
        {
            DebtorAccountNumber = "12345",
            PaymentScheme = PaymentScheme.Chaps,
            Amount = 100
        };
        var account = new Account
        {
            AccountNumber = "12345",
            Balance = 200,
            AllowedPaymentSchemes = AllowedPaymentSchemes.Chaps,
            Status = AccountStatus.Disabled
        };

        _accountDataStoreMock = new Mock<IAccountDataStore>();
        _paymentService = new PaymentService(_accountDataStoreMock.Object, this._paymentValidators);
        _accountDataStoreMock.Setup(x => x.GetAccount(request.DebtorAccountNumber)).Returns(account);

        // Act
        var result = _paymentService.MakePayment(request);

        // Assert
        result.Success.Should().BeFalse();
        account.Balance.Should().Be(200);
    }

    [Fact]
    public void Given_AccountStatus_Is_Live_When_Chaps_Payment_Should_Return_Success_For_Valid_Amount()
    {
        // Arrange
        var request = new MakePaymentRequest
        {
            DebtorAccountNumber = "12345",
            PaymentScheme = PaymentScheme.Chaps,
            Amount = 100
        };
        var account = new Account
        {
            AccountNumber = "12345",
            Balance = 200,
            AllowedPaymentSchemes = AllowedPaymentSchemes.Chaps,
            Status = AccountStatus.Live
        };

        _accountDataStoreMock = new Mock<IAccountDataStore>();
        _paymentService = new PaymentService(_accountDataStoreMock.Object, this._paymentValidators);
        _accountDataStoreMock.Setup(x => x.GetAccount(request.DebtorAccountNumber)).Returns(account);

        // Act
        var result = _paymentService.MakePayment(request);

        // Assert
        result.Success.Should().BeTrue();
        account.Balance.Should().Be(100);
    }

    [Fact]
    public void Given_Chaps_Payment_When_MakePayment_Amount_Greater_Than_Balance_Should_Return_Success_And_Negative_Balance()
    {
        // Arrange
        var request = new MakePaymentRequest
        {
            DebtorAccountNumber = "12345",
            PaymentScheme = PaymentScheme.Chaps,
            Amount = 500
        };
        var account = new Account
        {
            AccountNumber = "12345",
            Balance = 200,
            AllowedPaymentSchemes = AllowedPaymentSchemes.Chaps
        };

        _accountDataStoreMock = new Mock<IAccountDataStore>();
        _paymentService = new PaymentService(_accountDataStoreMock.Object, this._paymentValidators);
        _accountDataStoreMock.Setup(x => x.GetAccount(request.DebtorAccountNumber)).Returns(account);

        // Act
        var result = _paymentService.MakePayment(request);

        // Assert
        result.Success.Should().BeTrue();
        account.Balance.Should().Be(-300);
    }
}

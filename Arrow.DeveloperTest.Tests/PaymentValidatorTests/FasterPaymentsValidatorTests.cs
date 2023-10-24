using Arrow.DeveloperTest.Data;
using Arrow.DeveloperTest.PaymentValidators;
using Arrow.DeveloperTest.Services;
using Arrow.DeveloperTest.Types;
using FluentAssertions;
using Moq;
using Xunit;

namespace Arrow.DeveloperTest.Tests.PaymentValidatorTests
{
    public class FasterPaymentsValidatorTests
    {
        private Mock<IAccountDataStore> _accountDataStoreMock;
        private PaymentService _paymentService;
        private readonly Dictionary<PaymentScheme, IPaymentValidator> _paymentValidators;
        public FasterPaymentsValidatorTests()
        {
            this._paymentValidators = new Dictionary<PaymentScheme, IPaymentValidator>
            {
                { PaymentScheme.Bacs, new BacsPaymentValidator() },
                { PaymentScheme.FasterPayments, new FasterPaymentsValidator() },
                { PaymentScheme.Chaps, new ChapsPaymentValidator() }
            };
        }

        [Fact]
        public void Given_Faster_Payment_When_MakePayment_Should_Return_Success_For_Valid_Amount()
        {
            // Arrange
            var request = new MakePaymentRequest
            {
                DebtorAccountNumber = "12345",
                PaymentScheme = PaymentScheme.FasterPayments,
                Amount = 100
            };
            var account = new Account
            {
                AccountNumber = "12345",
                Balance = 200,
                AllowedPaymentSchemes = AllowedPaymentSchemes.FasterPayments
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
        public void Given_AccountStatus_Is_Disabled_When_FasterPayments_Payment_Should_Still_Succeed_For_Valid_Amount()
        {
            // Arrange
            var request = new MakePaymentRequest
            {
                DebtorAccountNumber = "12345",
                PaymentScheme = PaymentScheme.FasterPayments,
                Amount = 100
            };
            var account = new Account
            {
                AccountNumber = "12345",
                Balance = 200,
                AllowedPaymentSchemes = AllowedPaymentSchemes.FasterPayments,
                Status = AccountStatus.Disabled
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
        public void Given_Faster_Payment_When_MakePayment_Amount_Greater_Than_Balance_Should_Return_Fail()
        {
            // Arrange
            var request = new MakePaymentRequest
            {
                DebtorAccountNumber = "12345",
                PaymentScheme = PaymentScheme.FasterPayments,
                Amount = 1000
            };
            var account = new Account
            {
                AccountNumber = "12345",
                Balance = 200,
                AllowedPaymentSchemes = AllowedPaymentSchemes.FasterPayments | AllowedPaymentSchemes.Chaps
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
        public void Given_Faster_Unavailable_When_Faster_Payment_Should_Return_Fail_For_Valid_Amount()
        {
            // Arrange
            var request = new MakePaymentRequest
            {
                DebtorAccountNumber = "12345",
                PaymentScheme = PaymentScheme.FasterPayments,
                Amount = 100
            };
            var account = new Account
            {
                AccountNumber = "12345",
                Balance = 200,
                AllowedPaymentSchemes = AllowedPaymentSchemes.Bacs | AllowedPaymentSchemes.Chaps
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
    }
}

using Arrow.DeveloperTest.Data;
using Arrow.DeveloperTest.PaymentValidators;
using Arrow.DeveloperTest.Services;
using Arrow.DeveloperTest.Types;
using FluentAssertions;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Arrow.DeveloperTest.Tests
{
    public class PaymentServiceTests
    {
        private Mock<IAccountDataStore> _accountDataStoreMock;
        private PaymentService _paymentService;
        private readonly Dictionary<PaymentScheme, IPaymentValidator> _paymentValidators;

        public PaymentServiceTests()
        {
            this._paymentValidators = new Dictionary<PaymentScheme, IPaymentValidator>
            {
                { PaymentScheme.Bacs, new BacsPaymentValidator() },
                { PaymentScheme.FasterPayments, new FasterPaymentsValidator() },
                { PaymentScheme.Chaps, new ChapsPaymentValidator() }
            };
        }

        [Fact]
        public void Given_Null_AccountDataStore_When_Instantiating_Payments_Service_Should_Throw_Exception()
        {
            // Arrange
            // Act

            Action instantiation = () => { var paymentService = new PaymentService(null, this._paymentValidators); };

            // Assert
            instantiation.Should().Throw<ArgumentNullException>();

        }

        [Fact]
        public void Given_Null_PaymentValidators_When_Instantiating_Payments_Service_Should_Throw_Exception()
        {
            // Arrange
            var request = new MakePaymentRequest
            {
                DebtorAccountNumber = "12345",
                PaymentScheme = PaymentScheme.Bacs,
                Amount = 100
            };
            var account = new Account
            {
                AccountNumber = "12345",
                Balance = 200,
                AllowedPaymentSchemes = AllowedPaymentSchemes.Bacs
            };
            _accountDataStoreMock = new Mock<IAccountDataStore>();
            _accountDataStoreMock.Setup(x => x.GetAccount(request.DebtorAccountNumber)).Returns(account);

            // Act

            Action instantiation = () => { var paymentService = new PaymentService(this._accountDataStoreMock.Object, null); };

            // Assert
            instantiation.Should().Throw<ArgumentNullException>();

        }

        [Fact]
        public void Given_Null_PaymentRequest_When_Calling_Payments_Service_MakePayment_Should_Throw_Exception()
        {
            // Arrange
            var request = new MakePaymentRequest
            {
                DebtorAccountNumber = "12345",
                PaymentScheme = PaymentScheme.Bacs,
                Amount = 100
            };
            var account = new Account
            {
                AccountNumber = "12345",
                Balance = 200,
                AllowedPaymentSchemes = AllowedPaymentSchemes.Bacs
            };
            _accountDataStoreMock = new Mock<IAccountDataStore>();
            _accountDataStoreMock.Setup(x => x.GetAccount(request.DebtorAccountNumber)).Returns(account);

            var paymentService = new PaymentService(this._accountDataStoreMock.Object, this._paymentValidators);

            // Act
            Action makePayment = () => { paymentService.MakePayment(null); };

            // Assert
            makePayment.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void Given_Unsupported_Payment_Scheme_When_Calling_Payments_Service_MakePayment_Should_Return_False()
        {
            // Arrange
            var request = new MakePaymentRequest
            {
                DebtorAccountNumber = "12345",
                PaymentScheme = PaymentScheme.Swift15022_unsupported,
                Amount = 100
            };
            var account = new Account
            {
                AccountNumber = "12345",
                Balance = 200,
                AllowedPaymentSchemes = AllowedPaymentSchemes.Bacs
            };

            _accountDataStoreMock = new Mock<IAccountDataStore>();
            _paymentService = new PaymentService(_accountDataStoreMock.Object, this._paymentValidators);
            _accountDataStoreMock.Setup(x => x.GetAccount(request.DebtorAccountNumber)).Returns(account);

            // Act
            var result = _paymentService.MakePayment(request);

            // Assert
            result.Success.Should().BeFalse();
        }

        [Fact]
        public void Given_Valid_Payment_Scheme_When_Calling_Payments_Service_MakePayment_Then_Should_Call_paymentValidators_Validate()
        {
            // Arrange
            var request = new MakePaymentRequest
            {
                DebtorAccountNumber = "12345",
                PaymentScheme = PaymentScheme.Bacs,
                Amount = 100
            };
            var account = new Account
            {
                AccountNumber = "12345",
                Balance = 200,
                AllowedPaymentSchemes = AllowedPaymentSchemes.Bacs
            };

            var paymentValidator = new Mock<IPaymentValidator>();
            paymentValidator.Setup(x => x.Validate(account, request)).Returns(true);

            var paymentValidators = new Dictionary<PaymentScheme, IPaymentValidator>()
            {
                { PaymentScheme.Bacs, paymentValidator.Object }
            };

            _accountDataStoreMock = new Mock<IAccountDataStore>();
            _paymentService = new PaymentService(_accountDataStoreMock.Object, paymentValidators);
            _accountDataStoreMock.Setup(x => x.GetAccount(request.DebtorAccountNumber)).Returns(account);

            // Act
            var result = _paymentService.MakePayment(request);

            // Assert
            result.Success.Should().BeTrue();
        }

        [Fact]
        public void Given_Valid_Payment_Scheme_When_Calling_accountDataStore_UpdateAccount_Throws_Error_Then_Result_Should_Be_False()
        {
            // Arrange
            var request = new MakePaymentRequest
            {
                DebtorAccountNumber = "12345",
                PaymentScheme = PaymentScheme.Bacs,
                Amount = 100
            };
            var account = new Account
            {
                AccountNumber = "12345",
                Balance = 200,
                AllowedPaymentSchemes = AllowedPaymentSchemes.Bacs
            };

            var paymentValidator = new Mock<IPaymentValidator>();
            paymentValidator.Setup(x => x.Validate(account, request)).Returns(true);

            var paymentValidators = new Dictionary<PaymentScheme, IPaymentValidator>()
            {
                { PaymentScheme.Bacs, paymentValidator.Object }
            };

            _accountDataStoreMock = new Mock<IAccountDataStore>();
            _paymentService = new PaymentService(_accountDataStoreMock.Object, paymentValidators);
            _accountDataStoreMock.Setup(x => x.GetAccount(request.DebtorAccountNumber)).Returns(account);
            _accountDataStoreMock.Setup(x => x.UpdateAccount(account)).Throws<Exception>();

            // Act
            var result = _paymentService.MakePayment(request);

            // Assert
            result.Success.Should().BeFalse();
        }
    }
}

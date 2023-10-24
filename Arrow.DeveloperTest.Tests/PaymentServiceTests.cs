using Arrow.DeveloperTest.Data;
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

        // -------- BACS ------------------------

        [Fact]
        public void Given_Bacs_Payment_When_MakePayment_Should_Return_Success_For_Valid_Amount()
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
            _paymentService = new PaymentService(_accountDataStoreMock.Object);
            _accountDataStoreMock.Setup(x => x.GetAccount(request.DebtorAccountNumber)).Returns(account);

            // Act
            var result = _paymentService.MakePayment(request);

            // Assert
            result.Success.Should().BeTrue();
            account.Balance.Should().Be(100);
        }

        [Fact]
        public void Given_Bacs_Payment_When_MakePayment_Amount_Greater_Than_Balance_Should_Return_Success_And_Negative_Balance()
        {
            // Arrange
            var request = new MakePaymentRequest
            {
                DebtorAccountNumber = "12345",
                PaymentScheme = PaymentScheme.Bacs,
                Amount = 300
            };
            var account = new Account
            {
                AccountNumber = "12345",
                Balance = 200,
                AllowedPaymentSchemes = AllowedPaymentSchemes.Bacs
            };

            _accountDataStoreMock = new Mock<IAccountDataStore>();
            _paymentService = new PaymentService(_accountDataStoreMock.Object);
            _accountDataStoreMock.Setup(x => x.GetAccount(request.DebtorAccountNumber)).Returns(account);

            // Act
            var result = _paymentService.MakePayment(request);

            // Assert
            result.Success.Should().BeTrue();
            account.Balance.Should().Be(-100);
        }

        [Fact]
        public void Given_Bacs_Unavailable_When_Bacs_Payment_Should_Return_Fail_For_Valid_Amount()
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
                AllowedPaymentSchemes = AllowedPaymentSchemes.FasterPayments | AllowedPaymentSchemes.Chaps
            };

            _accountDataStoreMock = new Mock<IAccountDataStore>();
            _paymentService = new PaymentService(_accountDataStoreMock.Object);
            _accountDataStoreMock.Setup(x => x.GetAccount(request.DebtorAccountNumber)).Returns(account);

            // Act
            var result = _paymentService.MakePayment(request);

            // Assert
            result.Success.Should().BeFalse();
            account.Balance.Should().Be(200);
        }

        [Fact]
        public void Given_Account_Null_When_Bacs_Payment_Should_Return_Fail()
        {
            // Arrange
            var request = new MakePaymentRequest
            {
                DebtorAccountNumber = "12345",
                PaymentScheme = PaymentScheme.Bacs,
                Amount = 100
            };
            Account? account = null;

            _accountDataStoreMock = new Mock<IAccountDataStore>();
            _paymentService = new PaymentService(_accountDataStoreMock.Object);
            _accountDataStoreMock.Setup(x => x.GetAccount(request.DebtorAccountNumber)).Returns(account);

            // Act
            var result = _paymentService.MakePayment(request);

            // Assert
            result.Success.Should().BeFalse();
            account.Should().BeNull();
        }

        [Fact]
        public void Given_AccountStatus_Is_Disabled_When_Bacs_Payment_Should_Return_Success_For_Valid_Amount()
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
                AllowedPaymentSchemes = AllowedPaymentSchemes.Bacs,
                Status = AccountStatus.Disabled
            };

            _accountDataStoreMock = new Mock<IAccountDataStore>();
            _paymentService = new PaymentService(_accountDataStoreMock.Object);
            _accountDataStoreMock.Setup(x => x.GetAccount(request.DebtorAccountNumber)).Returns(account);

            // Act
            var result = _paymentService.MakePayment(request);

            // Assert
            result.Success.Should().BeTrue();
            account.Balance.Should().Be(100);
        }

        // ---------------------------------

        // -------- CHAPS ------------------------

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
            _paymentService = new PaymentService(_accountDataStoreMock.Object);
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
            _paymentService = new PaymentService(_accountDataStoreMock.Object);
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
            _paymentService = new PaymentService(_accountDataStoreMock.Object);
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
            _paymentService = new PaymentService(_accountDataStoreMock.Object);
            _accountDataStoreMock.Setup(x => x.GetAccount(request.DebtorAccountNumber)).Returns(account);

            // Act
            var result = _paymentService.MakePayment(request);

            // Assert
            result.Success.Should().BeTrue();
            account.Balance.Should().Be(-300);
        }

        // ---------------------------------

        // -------- FASTER ------------------------

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
            _paymentService = new PaymentService(_accountDataStoreMock.Object);
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
            _paymentService = new PaymentService(_accountDataStoreMock.Object);
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
            _paymentService = new PaymentService(_accountDataStoreMock.Object);
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
            _paymentService = new PaymentService(_accountDataStoreMock.Object);
            _accountDataStoreMock.Setup(x => x.GetAccount(request.DebtorAccountNumber)).Returns(account);

            // Act
            var result = _paymentService.MakePayment(request);

            // Assert
            result.Success.Should().BeFalse();
            account.Balance.Should().Be(200);
        }

    }
}

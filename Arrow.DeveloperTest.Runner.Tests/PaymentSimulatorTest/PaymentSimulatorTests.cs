using Arrow.DeveloperTest.Services;
using Moq;

public class PaymentSimulatorTests
{
    [Fact]
    public async Task Given_RunSimulation_WithValidInput_Then_ProcessesTransactions()
    {
        // Arrange
        var workerCount = 5;
        var mockPaymentServiceFactory = new Mock<IPaymentServiceFactory>();
        var mockPaymentService = new Mock<IPaymentService>();
        var reporter = new Mock<IReporter>();
        mockPaymentService.Setup(x => x.MakePayment(It.IsAny<MakePaymentRequest>())).Returns(new MakePaymentResult() { Success = true });
        for (int i = 0; i < workerCount; i++)
        {
            mockPaymentServiceFactory.Setup(x => x.CreateInstance(i)).Returns(mockPaymentService.Object);
        }

        var paymentSimulator = new PaymentSimulator(mockPaymentServiceFactory.Object, reporter.Object);
        var cancellationToken = CancellationToken.None;

        // Act
        await paymentSimulator.RunSimulation(5, 100, cancellationToken);

        // Assert
        mockPaymentServiceFactory.Verify(factory => factory.CreateInstance(It.IsAny<int>()), Times.Exactly(5));
        reporter.Verify(report => report.GenerateUpdate(It.IsAny<Report>()));
    }
}

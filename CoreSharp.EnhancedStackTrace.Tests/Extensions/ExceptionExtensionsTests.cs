using CoreSharp.EnhancedStackTrace.Extensions;

namespace CoreSharp.EnhancedStackTrace.Tests.Extensions;

public sealed class ExceptionExtensionsTests
{
    [Test]
    public void Enhance_WhenExceptionIsNull_ShouldThrowArgumentNullException()
    {
        // Arrange
        Exception exception = null!;

        // Act
        Action action = () => exception.Enhance();

        // Assert
        action.Should().ThrowExactly<ArgumentNullException>();
    }
}

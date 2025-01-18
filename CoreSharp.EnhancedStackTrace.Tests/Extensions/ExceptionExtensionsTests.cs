using CoreSharp.EnhancedStackTrace.Extensions;

namespace CoreSharp.EnhancedStackTrace.Tests.Extensions;

public sealed class ExceptionExtensionsTests
{
    [Fact]
    public void Enhance_WhenExceptionIsNull_ShouldThrowArgumentNullException()
    {
        // Arrange
        Exception exception = null!;

        // Act
        void Action()
            => exception.Enhance();

        // Assert
        Assert.Throws<ArgumentNullException>(Action);
    }
}

using CoreSharp.EnhancedStackTrace.Extensions;

namespace CoreSharp.EnhancedStackTrace.Tests.Features.Factory;

public sealed class EnhancedStackTraceFactoryTests : TestsBase
{
    [Fact]
    public void Create_WhenExceptionIsNull_ShouldThrowArgumentNullException()
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

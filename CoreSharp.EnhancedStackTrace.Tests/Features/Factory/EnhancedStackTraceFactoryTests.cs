using CoreSharp.EnhancedStackTrace.Extensions;
using SystemStackTrace = System.Diagnostics.StackTrace;

namespace CoreSharp.EnhancedStackTrace.Tests.Features.Factory;

internal sealed class EnhancedStackTraceFactoryTests : TestsBase
{
    [Test]
    public void Create_WhenExceptionIsNull_ShouldThrowArgumentNullException()
    {
        // Arrange 
        Exception exception = null!;

        // Act
        Action action = () => exception.Enhance();

        // Assert
        action.Should().ThrowExactly<ArgumentNullException>();
    }
}

using CoreSharp.EnhancedStackTrace.Extensions;
using System.Diagnostics;

namespace CoreSharp.EnhancedStackTrace.Tests.Extensions;

public sealed class StackTraceExtensionsTests
{
    [Test]
    public void Enhanced_WhenStackTraceIsNull_ShouldThrowArgumentNullException()
    {
        // Arrange
        StackTrace stackTrace = null!;

        // Act
        Action action = () => stackTrace.Enhance();

        // Assert
        action.Should().ThrowExactly<ArgumentNullException>();
    }
}

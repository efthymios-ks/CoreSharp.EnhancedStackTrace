using CoreSharp.EnhancedStackTrace.Extensions;
using System.Diagnostics;

namespace CoreSharp.EnhancedStackTrace.Tests.Extensions;

public sealed class StackTraceExtensionsTests
{
    [Fact]
    public void Enhanced_WhenStackTraceIsNull_ShouldThrowArgumentNullException()
    {
        // Arrange
        StackTrace stackTrace = null!;

        // Act
        void Action()
            => stackTrace.Enhance();

        // Assert
        Assert.Throws<ArgumentNullException>(Action);
    }
}

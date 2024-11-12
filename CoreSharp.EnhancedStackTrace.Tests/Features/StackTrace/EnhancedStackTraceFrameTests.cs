using CoreSharp.EnhancedStackTrace.Features.StackTrace;

namespace CoreSharp.EnhancedStackTrace.Tests.Features.StackTrace;

public sealed class EnhancedStackTraceFrameTests
{
    [Test]
    public void ToString_WhenCalled_ShouldReturnValuePassedInConstructor()
    {
        // Arrange
        var input = "expectedValue";
        var enhancedStackTraceFrame = new EnhancedStackTraceFrame(input);

        // Act
        var result = enhancedStackTraceFrame.ToString();

        // Assert
        result.Should().Be(input);
    }
}

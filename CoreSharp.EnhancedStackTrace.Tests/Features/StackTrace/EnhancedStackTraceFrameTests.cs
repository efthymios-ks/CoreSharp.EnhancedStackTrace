using CoreSharp.EnhancedStackTrace.Features.StackTrace;

namespace CoreSharp.EnhancedStackTrace.Tests.Features.StackTrace;

public sealed class EnhancedStackTraceFrameTests
{
    [Fact]
    public void ToString_WhenCalled_ShouldReturnValuePassedInConstructor()
    {
        // Arrange
        const string input = "expectedValue";
        var enhancedStackTraceFrame = new EnhancedStackTraceFrame(input);

        // Act
        var result = enhancedStackTraceFrame.ToString();

        // Assert
        Assert.Equal(input, result);
    }
}

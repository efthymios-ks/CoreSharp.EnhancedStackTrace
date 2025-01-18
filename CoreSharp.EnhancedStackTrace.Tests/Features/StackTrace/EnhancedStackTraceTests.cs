using CoreSharp.EnhancedStackTrace.Features.StackTrace;

namespace CoreSharp.EnhancedStackTrace.Tests.Features.StackTrace;

public sealed class EnhancedStackTraceTests
{
    [Fact]
    public void ToString_WhenCalled_ShouldReturnConcatenatedFrames()
    {
        // Arrange
        var frames = new[]
        {
            new EnhancedStackTraceFrame("frame1"),
            new EnhancedStackTraceFrame("frame2"),
            new EnhancedStackTraceFrame("frame3")
        };
        var enhancedStackTrace = new EnhancedStackTrace.Features.StackTrace.EnhancedStackTrace(frames);

        // Act
        var result = enhancedStackTrace.ToString();

        // Assert
        Assert.Equal("""
           at frame1
           at frame2
           at frame3
        """, result);
    }
}

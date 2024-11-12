using System.Diagnostics;

namespace CoreSharp.EnhancedStackTrace.Features.StackTrace;

public sealed class EnhancedStackTrace
{
    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    private readonly string _toStringValue = null!;

    internal EnhancedStackTrace(EnhancedStackTraceFrame[] frames)
    {
        ArgumentNullException.ThrowIfNull(frames);

        Frames = frames;

        var framesAsString = frames.Select(frame => $"   at {frame}");
        _toStringValue = string.Join(Environment.NewLine, framesAsString);
    }

    public IEnumerable<EnhancedStackTraceFrame> Frames { get; } = [];

    public override string ToString()
        => _toStringValue;
}

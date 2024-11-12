using System.Diagnostics;

namespace CoreSharp.EnhancedStackTrace.Features.StackTrace;

public sealed class EnhancedStackTraceFrame
{
    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    private readonly string _toStringValue = null!;

    internal EnhancedStackTraceFrame(string toStringValue)
        => _toStringValue = toStringValue;

    public override string ToString()
        => _toStringValue;
}

using CoreSharp.EnhancedStackTrace.Features.Factory;
using System.Diagnostics;

namespace CoreSharp.EnhancedStackTrace.Extensions;

public static class StackTraceExtensions
{
    public static Features.StackTrace.EnhancedStackTrace Enhance(this StackTrace stackTrace)
    {
        ArgumentNullException.ThrowIfNull(stackTrace);

        return EnhancedStackTraceFactory.Instance
            .Create(stackTrace);
    }
}

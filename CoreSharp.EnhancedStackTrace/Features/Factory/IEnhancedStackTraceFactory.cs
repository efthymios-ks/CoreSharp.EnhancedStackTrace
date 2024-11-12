using SystemStackTrace = System.Diagnostics.StackTrace;

namespace CoreSharp.EnhancedStackTrace.Features.Factory;

public interface IEnhancedStackTraceFactory
{
    StackTrace.EnhancedStackTrace Create(Exception exception);
    StackTrace.EnhancedStackTrace Create(SystemStackTrace stackTrace);
}

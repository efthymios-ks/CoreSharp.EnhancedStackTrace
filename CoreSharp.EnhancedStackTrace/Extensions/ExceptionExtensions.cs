using CoreSharp.EnhancedStackTrace.Features.Factory;
using System.Reflection;

namespace CoreSharp.EnhancedStackTrace.Extensions;

public static class ExceptionExtensions
{
    private static readonly FieldInfo _stackTraceField = typeof(Exception).GetField("_stackTraceString", BindingFlags.Instance | BindingFlags.NonPublic)!;

    public static Exception Enhance<TException>(this TException exception)
        where TException : Exception
    {
        ArgumentNullException.ThrowIfNull(exception);

        exception.EnhanceInternal();

        if (exception is AggregateException aggregatedException)
        {
            foreach (var innerException in aggregatedException.InnerExceptions)
            {
                innerException.Enhance();
            }
        }

        exception.InnerException?.Enhance();

        return exception;
    }

    private static void EnhanceInternal(this Exception exception)
        => _stackTraceField.SetValue(exception, EnhancedStackTraceFactory.Instance.Create(exception).ToString());
}

using CoreSharp.EnhancedStackTrace.Features.Reflection;
using System.Diagnostics;
using System.Reflection;
using SystemStackTrace = System.Diagnostics.StackTrace;

namespace CoreSharp.EnhancedStackTrace.Tests.Features.Serializers.Common;

internal abstract class ToStringArgsTestBase<TToStringArgs>
    where TToStringArgs : class
{
    private static readonly IReflectionHelper _reflectionHelper = new ReflectionHelper();

    public required string Label { get; init; }
    public virtual Action ThrowErrorFactory { get; init; } = null!;
    public StackFrame StackFrame { get; protected set; } = null!;
    public required string ExpectedToString { get; init; }
    public string FileName { get; set; } = null!;
    public int LineNumber { get; set; }

    public static void Init(IEnumerable<ToStringArgsTestBase<TToStringArgs>> toStringArgsList)
    {
        foreach (var toStringArgs in toStringArgsList)
        {
            try
            {
                toStringArgs.ThrowErrorFactory();
            }
            catch (Exception exception)
            {
                toStringArgs.SetFromException(exception);
            }
        }
    }

    public override string ToString()
        => Label;

    protected void SetFromException(Exception exception)
    {
        exception = exception.InnerException ?? exception;
        var methodBase = GetMethodBase(exception);
        StackFrame = CreateStackFrame(methodBase, FileName, LineNumber);
    }

    private MethodBase GetMethodBase(Exception exception)
        => new SystemStackTrace(exception)
             .GetFrames()
             .Select(method => method.GetMethod()!)
             .First(method => !_reflectionHelper.IsHiddenInStackTrace(method));

    private static StackFrame CreateStackFrame(
        MethodBase methodBase,
        string? fileName,
        int lineNumber)
    {
        var frame = new MockStackFrame();
        frame.SetMethodBase(methodBase);
        frame.SetFileName(fileName);
        frame.SetLineNumber(lineNumber);
        return frame;
    }
}


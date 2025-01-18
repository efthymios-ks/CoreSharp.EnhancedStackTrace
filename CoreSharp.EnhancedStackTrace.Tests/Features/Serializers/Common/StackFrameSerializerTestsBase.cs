using CoreSharp.EnhancedStackTrace.Features.Reflection;
using System.Diagnostics;
using System.Reflection;
using Tests.Common.Mocks;
using SystemStackTrace = System.Diagnostics.StackTrace;

namespace CoreSharp.EnhancedStackTrace.Tests.Features.Serializers.Common;

public abstract class StackFrameSerializerTestsBase : TestsBase
{
    private static readonly IReflectionHelper _reflectionHelper = new ReflectionHelper();

    protected StackFrame RunAndCapture(Action throwErrorFactory, string fileName, int lineNumber)
    {
        try
        {
            throwErrorFactory();
            throw new InvalidOperationException("Expected exception was not thrown.");
        }
        catch (Exception exception)
        {
            exception = exception.InnerException ?? exception;
            var methodBase = GetMethodBase(exception);
            return CreateStackFrame(methodBase, fileName, lineNumber);
        }
    }

    protected async Task<StackFrame> RunAndCaptureAsync(Func<Task> throwErrorFactory, string fileName, int lineNumber)
    {
        try
        {
            await throwErrorFactory();
            throw new InvalidOperationException("Expected exception was not thrown.");
        }
        catch (Exception exception)
        {
            exception = exception.InnerException ?? exception;
            var methodBase = GetMethodBase(exception);
            return CreateStackFrame(methodBase, fileName, lineNumber);
        }
    }

    private static MethodBase GetMethodBase(Exception exception)
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


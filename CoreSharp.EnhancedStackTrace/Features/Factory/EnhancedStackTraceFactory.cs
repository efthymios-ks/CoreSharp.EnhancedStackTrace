using CoreSharp.EnhancedStackTrace.Features.Reflection;
using CoreSharp.EnhancedStackTrace.Features.Serializers;
using CoreSharp.EnhancedStackTrace.Features.StackTrace;
using Microsoft.Extensions.DependencyInjection;
using SystemStackTrace = System.Diagnostics.StackTrace;

namespace CoreSharp.EnhancedStackTrace.Features.Factory;

internal sealed class EnhancedStackTraceFactory(
    IStackFrameSerializerFactory stackFrameSerializerFactory,
    IReflectionHelper reflectionHelper
    ) : IEnhancedStackTraceFactory
{
    private static IEnhancedStackTraceFactory _instance = null!;
    private readonly IStackFrameSerializerFactory _stackFrameSerializerFactory = stackFrameSerializerFactory;
    private readonly IReflectionHelper _reflectionHelper = reflectionHelper;

    public StackTrace.EnhancedStackTrace Create(Exception exception)
    {
        ArgumentNullException.ThrowIfNull(exception);

        return Create(new SystemStackTrace(exception, fNeedFileInfo: true));
    }

    public StackTrace.EnhancedStackTrace Create(SystemStackTrace stackTrace)
    {
        ArgumentNullException.ThrowIfNull(stackTrace);

        var frames = stackTrace
            .GetFrames()
            .Select(frame =>
            {
                var baseMethod = frame.GetMethod()!;
                if (_reflectionHelper.IsHiddenInStackTrace(baseMethod))
                {
                    return null!;
                }

                var serializer = _stackFrameSerializerFactory.Create(frame);
                var toStringValue = serializer.ToString(frame);
                return new EnhancedStackTraceFrame(toStringValue);
            })
            .Where(frame => frame is not null)
            .ToArray();

        return new(frames);
    }

    internal static IEnhancedStackTraceFactory Instance
    {
        get
        {
            if (_instance is not null)
            {
                return _instance;
            }

            _instance = new ServiceCollection()
                .AddEnhancedStackTrace()
                .BuildServiceProvider()
                .GetRequiredService<IEnhancedStackTraceFactory>();

            return _instance!;
        }
    }
}

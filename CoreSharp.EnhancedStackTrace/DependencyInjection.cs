using CoreSharp.EnhancedStackTrace.Features.Factory;
using CoreSharp.EnhancedStackTrace.Features.Reflection;
using CoreSharp.EnhancedStackTrace.Features.Serializers;
using Microsoft.Extensions.DependencyInjection;
using System.Diagnostics.CodeAnalysis;

namespace CoreSharp.EnhancedStackTrace;

[ExcludeFromCodeCoverage]
public static class DependencyInjection
{
    public static IServiceCollection AddEnhancedStackTrace(this IServiceCollection services)
    {
        ArgumentNullException.ThrowIfNull(services);

        services
            .AddTransient<IEnhancedStackTraceFactory, EnhancedStackTraceFactory>()
            .AddTransient<IStackFrameSerializerFactory, StackFrameSerializerFactory>()
            .AddTransient<IReflectionHelper, ReflectionHelper>()
            .AddTransient<ITypeAliasProvider, TypeAliasProvider>()
            .AddTransient<IParameterInfoHelper, ParameterInfoHelper>();

        // Order matters
        services.AddTransient<IStackFrameSerializer, LambdaStackFrameSerializer>();
        services.AddTransient<IStackFrameSerializer, AsyncStateStackFrameSerializer>();
        services.AddTransient<IStackFrameSerializer, MethodStackFrameSerializer>();
        services.AddTransient<IStackFrameSerializer, ConstructorStackFrameSerializer>();

        return services;
    }
}

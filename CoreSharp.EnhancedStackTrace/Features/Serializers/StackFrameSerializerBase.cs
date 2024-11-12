using CoreSharp.EnhancedStackTrace.Features.Reflection;
using System.Diagnostics;
using System.Reflection;

namespace CoreSharp.EnhancedStackTrace.Features.Serializers;

internal abstract class StackFrameSerializerBase(
    ITypeAliasProvider typeAliasProvider,
    IParameterInfoHelper parameterInfoHelper,
    IReflectionHelper reflectionHelper
    ) : IStackFrameSerializer
{
    private readonly ITypeAliasProvider _typeAliasProvider = typeAliasProvider;
    private readonly IParameterInfoHelper _parameterInfoHelper = parameterInfoHelper;
    private readonly IReflectionHelper _reflectionHelper = reflectionHelper;

    public abstract bool CanSerialize(StackFrame frame);
    public abstract string ToString(StackFrame frame);

    protected string GetAlias(Type type)
    {
        ArgumentNullException.ThrowIfNull(type);

        return _typeAliasProvider.GetAlias(type);
    }

    protected string GetAlias(ParameterInfo parameter)
    {
        ArgumentNullException.ThrowIfNull(parameter);

        return _parameterInfoHelper.GetAlias(parameter);
    }

    protected DeconstructedMethodName DeconstructMethodName(string rawMethodName)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(rawMethodName);

        return _reflectionHelper.DeconstructMethodName(rawMethodName);
    }

    protected string GetMethodsArgumentsAsString(MethodBase methodBase)
    {
        ArgumentNullException.ThrowIfNull(methodBase);

        var parameters = methodBase
            .GetParameters()
            .Select(GetAlias);

        return string.Join(", ", parameters);
    }

    protected string GetGenericArgumentsAsString(MethodBase methodBase)
    {
        ArgumentNullException.ThrowIfNull(methodBase);

        if (!methodBase.IsGenericMethod)
        {
            return string.Empty;
        }

        var argumentTypes = methodBase
            .GetGenericArguments()
            .Select(GetAlias);

        return string.Join(", ", argumentTypes);
    }

    protected static string GetLineInfoAsString(StackFrame frame)
    {
        ArgumentNullException.ThrowIfNull(frame);

        return $"in {frame.GetFileName()}:line {frame.GetFileLineNumber()}";
    }
}


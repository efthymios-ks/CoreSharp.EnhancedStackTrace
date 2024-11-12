using CoreSharp.EnhancedStackTrace.Extensions;
using CoreSharp.EnhancedStackTrace.Features.Reflection;
using System.Diagnostics;
using System.Reflection;
using System.Text;

namespace CoreSharp.EnhancedStackTrace.Features.Serializers;

internal sealed class MethodStackFrameSerializer(
    ITypeAliasProvider typeAliasProvider,
    IParameterInfoHelper parameterInfoHelper,
    IReflectionHelper reflectionHelper
    ) : StackFrameSerializerBase(typeAliasProvider, parameterInfoHelper, reflectionHelper), IStackFrameSerializer
{
    public override bool CanSerialize(StackFrame frame)
    {
        ArgumentNullException.ThrowIfNull(frame);

        return frame.GetMethod() is MethodInfo;
    }

    public override string ToString(StackFrame frame)
    {
        ArgumentNullException.ThrowIfNull(frame);

        var method = (MethodInfo)frame.GetMethod()!;
        var returnType = GetAlias(method.ReturnParameter);
        var declaringType = GetAlias(method.DeclaringType!);
        var (methodName, subMethodName, _) = DeconstructMethodName(method.Name);
        var methodArguments = GetMethodsArgumentsAsString(method);
        var genericArguments = GetGenericArgumentsAsString(method);

        return new StringBuilder()
            .Append($"{returnType} {declaringType}.{methodName}")
            .AppendIfNotEmpty(subMethodName, $"()+{subMethodName}")
            .AppendIfNotEmpty(genericArguments, $"<{genericArguments}>")
            .Append($"({methodArguments})")
            .Append($" {GetLineInfoAsString(frame)}")
            .ToString();
    }
}

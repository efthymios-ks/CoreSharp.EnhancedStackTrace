using CoreSharp.EnhancedStackTrace.Extensions;
using CoreSharp.EnhancedStackTrace.Features.Reflection;
using System.Diagnostics;
using System.Reflection;
using System.Text;

namespace CoreSharp.EnhancedStackTrace.Features.Serializers;

internal sealed class ConstructorStackFrameSerializer(
    ITypeAliasProvider typeAliasProvider,
    IParameterInfoHelper parameterInfoHelper,
    IReflectionHelper reflectionHelper
    ) : StackFrameSerializerBase(typeAliasProvider, parameterInfoHelper, reflectionHelper), IStackFrameSerializer
{
    public override bool CanSerialize(StackFrame frame)
    {
        ArgumentNullException.ThrowIfNull(frame);

        return frame.GetMethod() is ConstructorInfo;
    }

    public override string ToString(StackFrame frame)
    {
        ArgumentNullException.ThrowIfNull(frame);

        var constructor = (ConstructorInfo)frame.GetMethod()!;
        var declaringType = GetDeclaringType(constructor);
        var methodArguments = GetMethodsArgumentsAsString(constructor);
        var genericArguments = GetGenericArgumentsAsString(constructor);

        return new StringBuilder()
            .AppendIf(constructor.IsStatic, "static ", "new ")
            .Append(declaringType)
            .AppendIfNotEmpty(genericArguments, $"<{genericArguments}>")
            .Append($"({methodArguments})")
            .Append($" {GetLineInfoAsString(frame)}")
            .ToString();
    }

    private string GetDeclaringType(ConstructorInfo constructor)
    {
        var declaringType = constructor.DeclaringType!;
        var declaringTypeAsString = GetAlias(constructor.DeclaringType!);

        if (string.IsNullOrWhiteSpace(declaringType.Namespace)
            || declaringTypeAsString.StartsWith(declaringType.Namespace))
        {
            return declaringTypeAsString;
        }

        return $"{declaringType.Namespace}.{declaringTypeAsString}";
    }
}

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
    private const string IndexerGetterName = "get_Item";
    private const string IndexerSetterName = "set_Item";

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
        var methodArguments = GetMethodsArgumentsAsString(method);

        if (IsIndexer(method))
        {
            return new StringBuilder()
                .Append($"{returnType} {declaringType}[{methodArguments}]")
                .Append($" {GetLineInfoAsString(frame)}")
                .ToString();
        }

        var (methodName, subMethodName, _) = DeconstructMethodName(method.Name);
        var genericArguments = GetGenericArgumentsAsString(method);

        return new StringBuilder()
            .Append($"{returnType} {declaringType}.{methodName}")
            .AppendIfNotEmpty(subMethodName, $"()+{subMethodName}")
            .AppendIfNotEmpty(genericArguments, $"<{genericArguments}>")
            .Append($"({methodArguments})")
            .Append($" {GetLineInfoAsString(frame)}")
            .ToString();
    }

    private bool IsIndexer(MethodBase methodBase)
    {
        if (methodBase.DeclaringType == null
            || methodBase.Name != IndexerGetterName
            && methodBase.Name != IndexerSetterName)
        {
            return false;
        }

        return methodBase
            .DeclaringType
            .GetProperties(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)
            .FirstOrDefault(property => property.GetMethod == methodBase || property.SetMethod == methodBase)?
            .GetIndexParameters()
            .Any() is true;
    }
}

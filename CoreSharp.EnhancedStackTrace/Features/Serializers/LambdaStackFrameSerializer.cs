using CoreSharp.EnhancedStackTrace.Extensions;
using CoreSharp.EnhancedStackTrace.Features.Reflection;
using System.Diagnostics;
using System.Reflection;
using System.Text;

namespace CoreSharp.EnhancedStackTrace.Features.Serializers;

internal sealed class LambdaStackFrameSerializer(
    ITypeAliasProvider typeAliasProvider,
    IParameterInfoHelper parameterInfoHelper,
    IReflectionHelper reflectionHelper
    ) : StackFrameSerializerBase(typeAliasProvider, parameterInfoHelper, reflectionHelper), IStackFrameSerializer
{
    private readonly IReflectionHelper _reflectionHelper = reflectionHelper;

    public override bool CanSerialize(StackFrame frame)
    {
        ArgumentNullException.ThrowIfNull(frame);

        var methodBase = frame.GetMethod();
        if (methodBase is not MethodInfo)
        {
            return false;
        }

        var (methodName, _, subMethodIdentifier) = _reflectionHelper.DeconstructMethodName(methodBase.Name);
        return methodName is ".cctor" && subMethodIdentifier is SubMethodIdentifier.LambdaMethod;
    }

    public override string ToString(StackFrame frame)
    {
        ArgumentNullException.ThrowIfNull(frame);

        var method = (MethodInfo)frame.GetMethod()!;
        var delegateMember = UnwrapLambdaMember(method);
        var returnType = GetAlias(delegateMember.FieldType);
        var (methodName, subMethodName, subMethodIdentifier) = DeconstructMethodName(delegateMember.Name);
        var declaringType = GetDeclaringType(delegateMember);

        if (subMethodIdentifier is SubMethodIdentifier.AutoPropertyBackingField)
        {
            return new StringBuilder()
                .Append($"{returnType} {declaringType}.{methodName}")
                .Append($" {GetLineInfoAsString(frame)}")
                .ToString();
        }

        var genericArguments = GetGenericArgumentsAsString(method);
        return new StringBuilder()
            .Append($"{returnType} {declaringType}.{methodName}")
            .AppendIfNotEmpty(subMethodName, $"()+{subMethodName}")
            .AppendIfNotEmpty(genericArguments, $"<{genericArguments}>")
            .Append("()")
            .Append($" {GetLineInfoAsString(frame)}")
            .ToString();
    }

    private static FieldInfo UnwrapLambdaMember(MethodBase methodBase)
    {
        var declaringType = methodBase.DeclaringType!;
        while (declaringType is not null)
        {
            var fields = declaringType
                .GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.Instance | BindingFlags.DeclaredOnly);

            foreach (var field in fields)
            {
                var fieldValue = field.GetValue(field);
                if (fieldValue is Delegate @delegate && ReferenceEquals(@delegate.Method, methodBase))
                {
                    return field;
                }
            }

            declaringType = declaringType.DeclaringType!;
        }

        throw new InvalidOperationException("Lambda method not found.");
    }

    private string GetDeclaringType(FieldInfo delegateMember)
    {
        var declaringType = delegateMember.DeclaringType!;
        var declaringTypeAsString = GetAlias(delegateMember.DeclaringType!);

        if (string.IsNullOrWhiteSpace(declaringType.Namespace) || declaringTypeAsString.StartsWith(declaringType.Name))
        {
            return declaringTypeAsString;
        }

        return $"{declaringType.Namespace}.{declaringTypeAsString}";
    }
}

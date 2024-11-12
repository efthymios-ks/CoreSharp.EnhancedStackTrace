using CoreSharp.EnhancedStackTrace.Extensions;
using CoreSharp.EnhancedStackTrace.Features.Reflection;
using System.Diagnostics;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;

namespace CoreSharp.EnhancedStackTrace.Features.Serializers;

internal sealed class AsyncStateStackFrameSerializer(
    ITypeAliasProvider typeAliasProvider,
    IParameterInfoHelper parameterInfoHelper,
    IReflectionHelper reflectionHelper
    ) : StackFrameSerializerBase(typeAliasProvider, parameterInfoHelper, reflectionHelper), IStackFrameSerializer
{

    public override bool CanSerialize(StackFrame frame)
    {
        ArgumentNullException.ThrowIfNull(frame);

        var methodBase = frame.GetMethod();
        if (methodBase is not MethodInfo || methodBase.ReflectedType is null)
        {
            return false;
        }

        return methodBase
            .ReflectedType
            .GetInterfaces()
            .Contains(typeof(IAsyncStateMachine));
    }

    public override string ToString(StackFrame frame)
    {
        ArgumentNullException.ThrowIfNull(frame);

        var method = (MethodInfo)frame.GetMethod()!;
        var taskMethod = UnwrapTaskMethod(method);

        var returnType = GetAlias(taskMethod.ReturnParameter);
        var declaringType = GetAlias(method.DeclaringType!.DeclaringType!);
        var (methodName, _, _) = DeconstructMethodName(taskMethod.Name);
        var methodArguments = GetMethodsArgumentsAsString(taskMethod);
        var genericArguments = GetGenericArgumentsAsString(taskMethod);

        return new StringBuilder()
            .Append($"async {returnType} {declaringType}.{methodName}")
            .AppendIfNotEmpty(genericArguments, $"<{genericArguments}>")
            .Append($"({methodArguments})")
            .Append($" {GetLineInfoAsString(frame)}")
            .ToString();
    }

    private static MethodInfo UnwrapTaskMethod(MethodInfo method)
    {
        // Async execution path (outmost to innermost): 
        // > Container Class | Parent
        // > > TaskAwaiter<T> | Actual calling method
        // > > > IAsyncStateMachine.MoveNext() | StackFrame Method

        var taskAwaiterType = method.DeclaringType!;
        var taskAwaiterParentType = taskAwaiterType.DeclaringType!;

        return taskAwaiterParentType
            .GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.Instance | BindingFlags.DeclaredOnly)
            .Single(declaredMethod => declaredMethod
                .GetCustomAttributes(inherit: false)
                .OfType<StateMachineAttribute>()
                .Any(stateMachineAttribute => stateMachineAttribute.StateMachineType == taskAwaiterType)
            )!;
    }
}

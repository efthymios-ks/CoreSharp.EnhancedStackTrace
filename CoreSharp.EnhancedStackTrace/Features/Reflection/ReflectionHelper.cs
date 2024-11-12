using System.Diagnostics;
using System.Reflection;

namespace CoreSharp.EnhancedStackTrace.Features.Reflection;

internal sealed class ReflectionHelper : IReflectionHelper
{
    private static readonly string _tupleType = typeof(Tuple).FullName!;
    private static readonly string _valueTupleType = typeof(ValueTuple).FullName!;

    public bool IsHiddenInStackTrace(MethodBase methodBase)
    {
        ArgumentNullException.ThrowIfNull(methodBase);

        return methodBase.GetCustomAttribute<StackTraceHiddenAttribute>() is not null;
    }

    public DeconstructedMethodName DeconstructMethodName(string rawMethodName)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(rawMethodName);

        var closeIndex = rawMethodName.IndexOf('>');
        if (closeIndex is -1)
        {
            RemoveGenericTick(ref rawMethodName);
            return new(rawMethodName, SubMethodName: null, SubMethodIdentifier: null);
        }

        var openIndex = rawMethodName.LastIndexOf('<', closeIndex);
        var methodName = rawMethodName.Substring(openIndex + 1, closeIndex - openIndex - 1);
        RemoveGenericTick(ref methodName);

        const string underscore = "__";
        var underscoreIndex = rawMethodName.IndexOf(underscore, closeIndex);
        if (underscoreIndex is -1)
        {
            return new(methodName, SubMethodName: null, SubMethodIdentifier: null);
        }

        var lastCloseIndex = rawMethodName.LastIndexOf('>');
        var subMethodIdentifierAsString = rawMethodName[(lastCloseIndex + 1)..underscoreIndex];
        var subMethodIdentifier = subMethodIdentifierAsString switch
        {
            "b" => SubMethodIdentifier.LambdaMethod,
            "g" => SubMethodIdentifier.LocalFunction,
            "k" => SubMethodIdentifier.AutoPropertyBackingField,
            _ => default(SubMethodIdentifier?)
        };

        var pipeIndex = rawMethodName.IndexOf('|', underscoreIndex);
        if (pipeIndex is -1)
        {
            pipeIndex = rawMethodName.Length;
        }

        var charsToSkip = 1 + subMethodIdentifierAsString.Length + 2; // 1 (>) +  1 (Identifier) + 2 (Underscores)
        var subMethodName = rawMethodName[(closeIndex + charsToSkip)..pipeIndex];
        RemoveGenericTick(ref subMethodName);
        return new(methodName, subMethodName, subMethodIdentifier);
    }

    public bool IsTuple(Type type)
    {
        ArgumentNullException.ThrowIfNull(type);

        return type.IsGenericType && type.GetGenericTypeDefinition().FullName?.StartsWith(_tupleType) is true;
    }

    public bool IsTuple(Type type, int argumentCount)
    {
        ArgumentNullException.ThrowIfNull(type);

        return IsTuple(type) && type.GetGenericArguments().Length == argumentCount;
    }

    public bool IsValueTuple(Type type)
    {
        ArgumentNullException.ThrowIfNull(type);

        return type.IsGenericType && type.GetGenericTypeDefinition().FullName?.StartsWith(_valueTupleType) is true;
    }

    public bool IsValueTuple(Type type, int argumentCount)
    {
        ArgumentNullException.ThrowIfNull(type);

        return IsValueTuple(type) && type.GetGenericArguments().Length == argumentCount;
    }

    private static void RemoveGenericTick(ref string name)
    {
        var tickIndex = name.LastIndexOf('`');
        if (tickIndex is -1)
        {
            return;
        }

        name = name[..tickIndex];
    }
}

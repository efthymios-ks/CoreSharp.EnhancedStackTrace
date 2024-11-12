using System.Reflection;

namespace CoreSharp.EnhancedStackTrace.Features.Reflection;

internal interface IReflectionHelper
{
    bool IsHiddenInStackTrace(MethodBase methodBase);
    DeconstructedMethodName DeconstructMethodName(string rawMethodName);
    bool IsTuple(Type type);
    bool IsTuple(Type type, int argumentCount);
    bool IsValueTuple(Type type);
    bool IsValueTuple(Type type, int argumentCount);
}

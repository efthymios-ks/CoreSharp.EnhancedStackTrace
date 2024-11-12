using System.Reflection;

namespace CoreSharp.EnhancedStackTrace.Features.Reflection;

internal interface IParameterInfoHelper
{
    string GetAlias(ParameterInfo parameter);
}

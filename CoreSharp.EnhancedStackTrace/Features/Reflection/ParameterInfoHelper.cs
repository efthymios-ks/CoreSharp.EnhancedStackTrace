using CoreSharp.EnhancedStackTrace.Extensions;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;

namespace CoreSharp.EnhancedStackTrace.Features.Reflection;

internal sealed class ParameterInfoHelper(
    IReflectionHelper reflectionHelper,
    ITypeAliasProvider typeAliasProvider
    ) : IParameterInfoHelper
{
    private readonly IReflectionHelper _reflectionHelper = reflectionHelper;
    private readonly ITypeAliasProvider _typeAliasProvider = typeAliasProvider;

    public string GetAlias(ParameterInfo parameter)
    {
        ArgumentNullException.ThrowIfNull(parameter);

        return new StringBuilder()
            .Append(GetTypeAlias(parameter))
            .AppendIfNotEmpty(parameter.Name, $" {parameter.Name}")
            .AppendIf(parameter.HasDefaultValue && parameter.DefaultValue is not null, $" = {parameter.DefaultValue}")
            .ToString();
    }

    private string GetTypeAlias(ParameterInfo parameter)
    {
        var result = new StringBuilder()
            .AppendIf(IsParamsArray(parameter), "params ")
            .AppendIf(parameter.IsOut, "out ")
            .AppendIf(parameter.IsIn, "in ")
            .AppendIf(!parameter.IsOut && !parameter.IsIn && parameter.ParameterType.IsByRef, "ref ");

        if (TryGetDynamicTypeAlias(parameter, out var alias))
        {
            result.Append(alias);
        }

        else if (TryGetValueTupleTypeAlias(parameter, out alias))
        {
            result.Append(alias);
        }

        else
        {
            result.Append(_typeAliasProvider.GetAlias(parameter.ParameterType));
        }

        return result.ToString();
    }

    private static bool IsParamsArray(ParameterInfo parameter)
        => parameter.IsDefined(typeof(ParamArrayAttribute));

    private static bool TryGetDynamicTypeAlias(ParameterInfo parameter, out string? alias)
    {
        if (!parameter.IsDefined(typeof(DynamicAttribute)))
        {
            alias = null;
            return false;
        }

        alias = "dynamic";
        return true;
    }

    private bool TryGetValueTupleTypeAlias(ParameterInfo parameter, out string? alias)
    {
        alias = null;

        if (!_reflectionHelper.IsValueTuple(parameter.ParameterType))
        {
            return false;
        }

        var genericArguments = parameter.ParameterType.GetGenericArguments();

        var argumentNames = parameter
            .GetCustomAttribute<TupleElementNamesAttribute>()
            ?.TransformNames
            ?? new string[genericArguments.Length];

        var arguments = genericArguments.Zip(argumentNames, (argumentType, name) =>
        {
            var argumentAlias = _typeAliasProvider.GetAlias(argumentType);
            if (string.IsNullOrWhiteSpace(name))
            {
                return argumentAlias;
            }

            return $"{argumentAlias} {name}";
        });

        alias = $"({string.Join(", ", arguments)})";
        return true;
    }
}

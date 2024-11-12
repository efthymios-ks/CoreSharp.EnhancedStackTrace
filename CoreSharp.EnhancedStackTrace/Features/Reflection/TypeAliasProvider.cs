namespace CoreSharp.EnhancedStackTrace.Features.Reflection;

internal sealed class TypeAliasProvider(IReflectionHelper reflectionHelper) : ITypeAliasProvider
{
    private readonly IReflectionHelper _reflectionHelper = reflectionHelper;

    private delegate bool TryOverrideTypeNameFunc(Type type, out string? alias);

    private static readonly IReadOnlyDictionary<Type, string> _typeOverrideConstants = new Dictionary<Type, string>
    {
        { typeof(void), "void" },
        { typeof(object), "object" },
        { typeof(bool), "bool" },
        { typeof(byte), "byte" },
        { typeof(sbyte), "sbyte" },
        { typeof(char), "char" },
        { typeof(decimal), "decimal" },
        { typeof(double), "double" },
        { typeof(float), "float" },
        { typeof(int), "int" },
        { typeof(uint), "uint" },
        { typeof(long), "long" },
        { typeof(ulong), "ulong" },
        { typeof(short), "short" },
        { typeof(ushort), "ushort" },
        { typeof(string), "string" }
    };

    private static readonly TryOverrideTypeNameFunc[] _typeNameOverrideFuncs =
    [
        _typeOverrideConstants.TryGetValue,
        TryGetSystemTypeAlias
    ];

    public string GetAlias(Type type)
    {
        var nullableMark = GetNullableMark(ref type);

        if (TryGetArrayAlias(type, out var alias))
        {
            return alias!;
        }

        if (TryGetGenericAlias(type, out alias))
        {
            return alias!;
        }

        return GetAliasInternal(type) + nullableMark;
    }

    private static string GetNullableMark(ref Type type)
    {
        if (Nullable.GetUnderlyingType(type) is Type underlyingType)
        {
            type = underlyingType;
            return "?";
        }

        return string.Empty;
    }

    private bool TryGetArrayAlias(Type type, out string? alias)
    {
        if (!type.IsArray || !type.HasElementType)
        {
            alias = null;
            return false;
        }

        var elementType = GetAlias(type.GetElementType()!);
        var rank = type.GetArrayRank();
        var commas = new string(',', rank - 1);
        alias = $"{elementType}[{commas}]";
        return true;
    }

    private bool TryGetGenericAlias(Type type, out string? alias)
    {
        if (!type.IsGenericType)
        {
            alias = null;
            return false;
        }

        var genericType = type.GetGenericTypeDefinition();
        var genericArguments = type.GetGenericArguments().Select(GetAlias);
        var genericArgumentsAsString = string.Join(", ", genericArguments);

        alias = GetAliasInternal(genericType);
        alias = $"{alias}<{genericArgumentsAsString}>";
        return true;
    }

    private string GetAliasInternal(Type type)
    {
        if (type.IsByRef)
        {
            type = type.GetElementType()!;
        }

        foreach (var typeOverrideFunc in _typeNameOverrideFuncs)
        {
            if (typeOverrideFunc(type, out var aliasOverride))
            {
                return aliasOverride!;
            }
        }

        var rawName = type.FullName ?? type.Name;
        var (methodName, subMethodName, _) = _reflectionHelper.DeconstructMethodName(rawName);
        if (string.IsNullOrWhiteSpace(subMethodName))
        {
            return methodName;
        }

        return $"{methodName}.{subMethodName}";
    }

    private static bool TryGetSystemTypeAlias(Type type, out string? alias)
    {
        if (type.FullName is null || !type.FullName.StartsWith("System."))
        {
            alias = null;
            return false;
        }

        alias = type.Name;
        RemoveGenericTick(ref alias);
        return true;
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

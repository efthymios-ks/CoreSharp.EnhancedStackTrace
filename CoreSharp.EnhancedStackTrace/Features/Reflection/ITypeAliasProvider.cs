namespace CoreSharp.EnhancedStackTrace.Features.Reflection;

internal interface ITypeAliasProvider
{
    string GetAlias(Type type);
}

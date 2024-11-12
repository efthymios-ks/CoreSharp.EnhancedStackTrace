using System.Text;

namespace CoreSharp.EnhancedStackTrace.Extensions;

internal static class StringBuilderExtensions
{
    public static StringBuilder AppendIfNotEmpty(this StringBuilder builder, string? valueToCheck, string value)
    {
        ArgumentNullException.ThrowIfNull(builder);

        return builder.AppendIf(!string.IsNullOrWhiteSpace(valueToCheck), value);
    }

    public static StringBuilder AppendIf(this StringBuilder builder, bool condition, string value)
    {
        ArgumentNullException.ThrowIfNull(builder);

        if (condition)
        {
            builder.Append(value);
        }

        return builder;
    }

    public static StringBuilder AppendIf(this StringBuilder builder, bool condition, string truthValue, string falseValue)
    {
        ArgumentNullException.ThrowIfNull(builder);

        if (condition)
        {
            builder.Append(truthValue);
        }
        else
        {
            builder.Append(falseValue);
        }

        return builder;
    }
}

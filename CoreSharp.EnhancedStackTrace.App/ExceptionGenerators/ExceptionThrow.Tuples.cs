namespace CoreSharp.EnhancedStackTrace.App.ExceptionGenerators;

public static partial class ExceptionThrow
{
    public static void Tuple()
        => Internal.Tuple(System.Tuple.Create<int, string?>(default, default));

    public static void ValueTuple()
        => Internal.ValueTuple((default, default));

    public static void ValueTupleWithNames()
        => Internal.ValueTupleWithNames((default, default));
}

static file class Internal
{
    public static Tuple<int, string?> Tuple(Tuple<int, string?> tuple)
        => throw new Exception("Error");

    public static (int, string?) ValueTuple((int, string?) tuple)
        => throw new Exception("Error");

    public static (int a, string?) ValueTupleWithNames((int, string? b) tuple)
        => throw new Exception("Error");
}

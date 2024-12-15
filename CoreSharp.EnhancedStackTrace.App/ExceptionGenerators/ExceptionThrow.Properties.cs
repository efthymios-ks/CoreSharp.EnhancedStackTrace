namespace CoreSharp.EnhancedStackTrace.App.ExceptionGenerators;

public static partial class ExceptionThrow
{
    public static void PropertyGetter()
        => _ = Internal.Property;

    public static void PropertySetter()
        => Internal.Property = 0;

    public static void AutoPropertyBackingField()
        => _ = Internal.AutoProperty;

    public static void IndexerGet()
        => Internal.IndexerGet();

    public static void IndexerSet()
        => Internal.IndexerSet();
}

static file class Internal
{
    public static int Property
    {
        get => throw new Exception("Error");
        set => throw new Exception("Error");
    }

    public static int AutoProperty { get; set; } = ThrowDivideByZero();

    public static void IndexerGet()
        => _ = new IndexerClass()[0];

    public static void IndexerSet()
        => new IndexerClass()[0] = 0;

    private static int ThrowDivideByZero()
    {
        var zero = 0;
        return 1 / zero;
    }

    private sealed class IndexerClass
    {
        public int this[int index]
        {
            get => throw new Exception("Error");
            set => throw new Exception("Error");
        }
    }
}

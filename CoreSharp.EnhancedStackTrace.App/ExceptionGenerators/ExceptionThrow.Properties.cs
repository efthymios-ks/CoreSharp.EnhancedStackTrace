namespace CoreSharp.EnhancedStackTrace.App.ExceptionGenerators;

public static partial class ExceptionThrow
{
    public static void PropertyGetter()
        => _ = Internal.Property;

    public static void PropertySetter()
        => Internal.Property = 0;

    public static void AutoPropertyBackingField()
        => _ = Internal.AutoProperty;
}

static file class Internal
{
    public static int Property
    {
        get => throw new Exception("Error");
        set => throw new Exception("Error");
    }

    public static int AutoProperty { get; set; } = ThrowDivideByZero();

    private static int ThrowDivideByZero()
    {
        var zero = 0;
        return 1 / zero;
    }
}

namespace CoreSharp.EnhancedStackTrace.App.ExceptionGenerators;

public static partial class ExceptionThrow
{
    public static void Constructor()
        => new Internal.Constructor();

    public static void ConstructorWithArgument()
        => new Internal.ConstructorWithArgument(5);

    public static void ConstructorWithGeneric()
        => new Internal.ConstructorWithGeneric<int>();

    public static void ConstructorStatic()
        => Internal.ConstructorStatic.Reference();
}

static file class Internal
{
    public sealed class Constructor
    {
        public Constructor()
            => throw new Exception("Error");
    }

    public sealed class ConstructorWithArgument
    {
        public ConstructorWithArgument(int argument)
            => throw new Exception("Error");
    }

    public sealed class ConstructorWithDefaultArgument
    {
        public ConstructorWithDefaultArgument(int argument = 5)
            => throw new Exception("Error");

    }

    public sealed class ConstructorWithGeneric<TValue>
    {
        public ConstructorWithGeneric()
            => throw new Exception("Error");
    }

    public static class ConstructorStatic
    {
        static ConstructorStatic()
            => throw new Exception("Error");

        public static void Reference()
        {
        }
    }
}

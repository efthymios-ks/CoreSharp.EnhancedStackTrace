namespace CoreSharp.EnhancedStackTrace.App.ExceptionGenerators;

public static partial class ExceptionThrow
{
    public static void MethodWithVoidReturnType()
       => throw new Exception("Error");

    public static void MethodWithReturnType()
        => Internal.MethodWithReturnType();

    public static void MethodWithArguments()
        => Internal.MethodWithArguments(5);

    public static void MethodWithDefaultArgument()
        => Internal.MethodWithDefaultArgument();

    public static void MethodWithRefOutInArguments()
    {
        var refArgument = 5;
        var inArgument = 5;
#pragma warning disable IDE0059 // Unnecessary assignment of a value
        Internal.MethodWithRefOutInArguments(ref refArgument, out var outArgument, in inArgument);
#pragma warning restore IDE0059 // Unnecessary assignment of a value
    }

    public static void DynamicMethod()
        => Internal.DynamicMethod(0);

    public static void GenericMethod()
        => Internal.GenericMethod<int>();

    public static void MethodWithRefReturn()
        => Internal.MethodWithRefReturn();

    public static void MethodWithPrimitiveArgument()
        => Internal.MethodWithPrimitiveArgument(default);

    public static void MethodWithNullableArgument()
        => Internal.MethodWithNullableArgument(default);

    public static void MethodWith1dArrayArgument()
        => Internal.MethodWith1dArrayArgument([]);

    public static void MethodWith2dArrayArgument()
        => Internal.MethodWith2dArrayArgument(new int[0, 0]);

    public static void MethodWithListArgument()
        => Internal.MethodWithListArgument([]);

    public static void MethodWithNullableListArgument()
        => Internal.MethodWithNullableListArgument([]);

    public static void MethodWithParamsCollection()
        => Internal.MethodWithParamsCollection();

    public static void MethodWithVoidLocalMethod()
        => Internal.MethodWithVoidLocalMethod();

    public static void MethodWithLocalMethodWithArguments()
        => Internal.MethodWithLocalMethodWithArguments();

    public static void MethodWithNestedLocalMethod()
        => Internal.MethodWithNestedLocalMethod();
}

static file class Internal
{
    public static int MethodWithReturnType()
        => throw new Exception("Error");

    public static void MethodWithArguments(int argument)
        => throw new Exception("Error");

    public static void MethodWithDefaultArgument(int argument = 5)
        => throw new Exception("Error");

    public static void MethodWithRefOutInArguments(ref int refArgument, out int outArgument, in int inArgument)
        => throw new Exception("Error");

    public static void DynamicMethod(dynamic dynamicArgument)
        => throw new Exception("Error");

    public static void GenericMethod<TValue>()
        => throw new Exception("Error");

    public static ref int MethodWithRefReturn()
        => throw new Exception("Error");

    public static void MethodWithPrimitiveArgument(int value)
        => throw new Exception("Error");

    public static void MethodWithNullableArgument(int? value)
        => throw new Exception("Error");

    public static void MethodWith1dArrayArgument(int[] value)
        => throw new Exception("Error");

    public static void MethodWith2dArrayArgument(int[,] value)
        => throw new Exception("Error");

    public static void MethodWithListArgument(List<int> value)
        => throw new Exception("Error");

    public static void MethodWithNullableListArgument(List<int?> value)
        => throw new Exception("Error");

    public static void MethodWithParamsCollection(params int[] values)
        => throw new Exception("Error");

    public static void MethodWithVoidLocalMethod()
    {
        LocalMethod();

        static void LocalMethod()
            => throw new Exception("Error");
    }

    public static void MethodWithLocalMethodWithArguments()
    {
        LocalMethod<int>(default);

#pragma warning disable IDE0060 // Remove unused parameter
        static int LocalMethod<TValue>(string? value)
            => throw new Exception("Error");
#pragma warning restore IDE0060 // Remove unused parameter
    }

    public static void MethodWithNestedLocalMethod()
    {
        LocalMethod1();

        static void LocalMethod1()
        {
            LocalMethod2();

            static void LocalMethod2()
                => throw new Exception("Error");
        }
    }
}

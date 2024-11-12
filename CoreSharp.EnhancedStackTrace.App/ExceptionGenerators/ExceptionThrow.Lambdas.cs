namespace CoreSharp.EnhancedStackTrace.App.ExceptionGenerators;

public static partial class ExceptionThrow
{
    public static void FieldAction()
        => Internal.FieldAction();

    public static void FieldActionWithArgument()
        => Internal.FieldActionWithArgument(default);

    public static void FieldFunc()
        => Internal.FieldFunc();

    public static void FieldFuncWithArgument()
        => Internal.FieldFuncWithArgument(default);

    public static void PropertyAction()
        => Internal.PropertyAction();

    public static void PropertyActionWithArgument()
        => Internal.PropertyActionWithArgument = _ => { };

    public static void PropertyFunc()
        => Internal.PropertyFunc();
}

static file class Internal
{

    public static readonly Action FieldAction
        = () => throw new Exception("Error");

    public static readonly Action<int> FieldActionWithArgument
        = _ => throw new Exception("Error");

    public static readonly Func<int> FieldFunc
        = () => throw new Exception("Error");

    public static readonly Func<string?, int> FieldFuncWithArgument
        = _ => throw new Exception("Error");

    public static Action PropertyAction { get; }
        = () => throw new Exception("Error");

    public static Action<int> PropertyActionWithArgument { get; set; }
        = _ => throw new Exception("Error");

    public static Func<int> PropertyFunc { get; }
        = () => throw new Exception("Error");
}

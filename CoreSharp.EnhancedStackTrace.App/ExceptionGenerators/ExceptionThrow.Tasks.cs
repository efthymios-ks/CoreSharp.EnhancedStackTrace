namespace CoreSharp.EnhancedStackTrace.App.ExceptionGenerators;

public static partial class ExceptionThrow
{
    public static async Task TaskWithNoReturnType()
        => await Internal.TaskWithNoReturnType();

    public static async Task TaskWithReturnType()
        => await Internal.TaskWithReturnType();

    public static Task TaskNonAwaited()
        => Internal.TaskNonAwaited();

    public static void TaskGetAwaiterGetResult()
        => Internal.TaskGetAwaiterGetResult();

    public static void TaskResult()
        => Internal.TaskResult();
}

static file class Internal
{
    public static async Task TaskWithNoReturnType()
        => await Task.FromException(new Exception("Error"));

    public static async Task<int> TaskWithReturnType()
        => await Task.FromException<int>(new Exception("Error"));

    public static Task TaskNonAwaited()
        => Task.FromException(new Exception("Error"));

    public static void TaskGetAwaiterGetResult()
        => Task.FromException(new Exception("Error")).GetAwaiter().GetResult();

    public static int TaskResult()
        => Task.FromException<int>(new Exception("Error")).Result;
}

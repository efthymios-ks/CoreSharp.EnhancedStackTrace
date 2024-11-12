namespace CoreSharp.EnhancedStackTrace.Tests.Features.Serializers.Common;

internal abstract class AsyncToStringArgsBase<TToStringArgs> : ToStringArgsTestBase<TToStringArgs>
    where TToStringArgs : class
{
    public new Func<Task> ThrowErrorFactory { get; init; } = null!;

    public static async Task InitAsync(IEnumerable<AsyncToStringArgsBase<TToStringArgs>> toStringArgsList)
    {
        foreach (var toStringArgs in toStringArgsList)
        {
            try
            {
                await toStringArgs.ThrowErrorFactory();
            }
            catch (Exception exception)
            {
                toStringArgs.SetFromException(exception);
            }
        }
    }
}

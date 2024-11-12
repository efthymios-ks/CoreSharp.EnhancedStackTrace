using System.Collections;

namespace CoreSharp.EnhancedStackTrace.App.ExceptionGenerators;

// TODO: 
public static partial class ExceptionThrow
{
    public static void MethodWithEnumerableArgument(IEnumerable<string> value = null!)
        => throw new Exception("Error");

    public static void MethodWithLinqSelect()
        => Enumerable.Range(0, 2).Select(i =>
        {
            if (i == 1)
            {
                throw new Exception("Error");
            }

            return i;
        });

    public static void MethodWithEnumerator()
    {
        var enumerator = new EnumeratorWithError();
        while (enumerator.MoveNext())
        {
        }
    }

    private sealed class EnumeratorWithError : IEnumerator
    {
        public object Current
            => throw new NotImplementedException();

        public bool MoveNext()
            => throw new NotImplementedException();

        public void Reset()
            => throw new NotImplementedException();
    }
}

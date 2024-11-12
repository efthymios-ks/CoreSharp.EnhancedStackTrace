using System.Reflection;

namespace CoreSharp.EnhancedStackTrace.Tests.MockTypes;

public class MockException : Exception
{
    private static readonly FieldInfo _stackTraceField = typeof(Exception)
        .GetField("_stackTraceString", BindingFlags.Instance | BindingFlags.NonPublic)!;

    public void SetStackTrace(string? stackTrace)
        => _stackTraceField.SetValue(this, stackTrace);
}

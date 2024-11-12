using FluentAssertions.Execution;
using FluentAssertions.Primitives;

namespace CoreSharp.EnhancedStackTrace.Tests.Common;

public static class CustomAssertions
{
    public static AndConstraint<StringAssertions> BeGuid(
        this StringAssertions assertions,
        string because = "",
        params object[] becauseArgs)
    {
        var isGuid = Guid.TryParse(assertions.Subject, out _);

        Execute.Assertion
           .ForCondition(isGuid)
           .BecauseOf(because, becauseArgs)
           .FailWith("Expected a GUID converted to a string {reason}, but found {0}.", assertions.Subject);

        return new AndConstraint<StringAssertions>(assertions);
    }
}

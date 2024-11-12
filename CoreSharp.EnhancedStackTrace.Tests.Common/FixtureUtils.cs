using AutoFixture;
using AutoFixture.AutoNSubstitute;
using NSubstitute;

namespace CoreSharp.EnhancedStackTrace.Tests.Common;

public static class FixtureUtils
{
    public static IFixture CreateFixture()
    {
        var fixture = new Fixture();
        fixture.Customize(new AutoNSubstituteCustomization()
        {
            ConfigureMembers = true,
        });

        fixture.Register(() => Substitute.For<Type>());
        fixture.Register<IFixture>(() => fixture);
        fixture.Register(() => false);

        return fixture;
    }
}

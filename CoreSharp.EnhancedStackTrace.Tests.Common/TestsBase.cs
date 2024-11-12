using AutoFixture;

namespace CoreSharp.EnhancedStackTrace.Tests.Common;

public abstract class TestsBase
{
    private IFixture _fixture = null!;

    [SetUp]
    protected virtual void SetUp()
        => _fixture = FixtureUtils.CreateFixture();

    protected TElement MockCreate<TElement>()
        => _fixture.Create<TElement>();

    protected TElement[] MockCreateMany<TElement>()
        => _fixture
            .CreateMany<TElement>()
            .ToArray();

    protected TElement[] MockCreateMany<TElement>(int count)
        => _fixture
            .CreateMany<TElement>(count)
            .ToArray();

    protected TElement MockFreeze<TElement>()
        => _fixture.Freeze<TElement>();
}

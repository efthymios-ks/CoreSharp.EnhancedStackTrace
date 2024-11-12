using CoreSharp.EnhancedStackTrace.Features.Reflection;
using CoreSharp.EnhancedStackTrace.Features.Serializers;
using CoreSharp.EnhancedStackTrace.Tests.Features.Serializers.Common;

namespace CoreSharp.EnhancedStackTrace.Tests.Features.Serializers;

internal sealed class ConstructorStackFrameSerializerTests : TestsBase
{
    [OneTimeSetUp]
    public void OneTimeSetUp()
        => ToStringTestArgs.Init(ToStringTestArgs.Source);

    [Test]
    public void CanSerialize_WhenFrameIsNull_ShouldThrowArgumentNullException()
    {
        // Arrange
        var serializer = MockCreate<ConstructorStackFrameSerializer>();

        // Act
        Action action = () => serializer.CanSerialize(frame: null!);

        // Assert
        action.Should().Throw<ArgumentNullException>();
    }

    [Test]
    public void CanSerialize_WhenMethodBaseIsNotConstructorInfo_ShouldReturnFalse()
    {
        // Arrange
        var serializer = MockCreate<ConstructorStackFrameSerializer>();
        var frame = new MockStackFrame();

        var method = new MockMethodInfo();
        frame.SetMethodBase(method);

        // Act
        var result = serializer.CanSerialize(frame);

        // Assert
        result.Should().BeFalse();
    }

    [Test]
    public void CanSerialize_WhenMethodBaseIsConstructorInfo_ShouldReturnTrue()
    {
        // Arrange
        var serializer = MockCreate<ConstructorStackFrameSerializer>();
        var frame = new MockStackFrame();

        var method = new MockConstructorInfo();
        frame.SetMethodBase(method);

        // Act
        var result = serializer.CanSerialize(frame);

        // Assert
        result.Should().BeTrue();
    }

    [Test]
    public void ToString_WhenFrameIsNull_ShouldThrowArgumentNullException()
    {
        // Arrange
        var serializer = MockCreate<ConstructorStackFrameSerializer>();

        // Act
        Action action = () => serializer.ToString(frame: null!);

        // Assert
        action.Should().Throw<ArgumentNullException>();
    }

    [Test]
    [TestCaseSource(typeof(ToStringTestArgs), nameof(ToStringTestArgs.Source))]
    public void ToString_WhenCalled_ShouldReturnCorrectValue(ToStringTestArgs arguments)
    {
        // Arrange
        var reflectionHelper = new ReflectionHelper();
        var typeAliasProvider = new TypeAliasProvider(reflectionHelper);
        var parameterInfoHelper = new ParameterInfoHelper(reflectionHelper, typeAliasProvider);
        var serializer = new ConstructorStackFrameSerializer(
            typeAliasProvider,
            parameterInfoHelper,
            reflectionHelper);

        // Act
        var result = serializer.ToString(arguments.StackFrame);

        // Assert
        result.Should().Be(arguments.ExpectedToString);
    }

    public sealed class ToStringTestArgs : ToStringArgsTestBase<ToStringTestArgs>
    {
        public static IEnumerable<ToStringTestArgs> Source { get; } = [
            new()
            {
                Label = "HasGenericArguments",
                ThrowErrorFactory = () => new ConstructorGeneric<int>(),
                FileName = "MyFile",
                LineNumber = 10,
                ExpectedToString = "new CoreSharp.EnhancedStackTrace.Tests.Features.Serializers.ConstructorStackFrameSerializerTests+ToStringTestArgs+ConstructorGeneric<TValue>() in MyFile:line 10"
            },
            new()
            {
                Label = "HasArguments",
                ThrowErrorFactory = () => new ConstructorWithArguments(default),
                FileName =  "MyFile",
                LineNumber = 10,
                ExpectedToString = "new CoreSharp.EnhancedStackTrace.Tests.Features.Serializers.ConstructorStackFrameSerializerTests+ToStringTestArgs+ConstructorWithArguments(int argument) in MyFile:line 10"
            },
            new()
            {
                Label = "Static",
                ThrowErrorFactory =  ConstructorStatic.Reference,
                FileName = "MyFile",
                LineNumber = 10,
                ExpectedToString = "static CoreSharp.EnhancedStackTrace.Tests.Features.Serializers.ConstructorStackFrameSerializerTests+ToStringTestArgs+ConstructorStatic() in MyFile:line 10"
            }
        ];

        private sealed class ConstructorGeneric<TValue>
        {
            public ConstructorGeneric()
                => throw new Exception();
        }

        private sealed class ConstructorWithArguments
        {
            public ConstructorWithArguments(int argument)
                => throw new Exception();
        }

        private static class ConstructorStatic
        {
            static ConstructorStatic()
                => throw new Exception();

            public static void Reference()
            {
            }
        }
    }
}

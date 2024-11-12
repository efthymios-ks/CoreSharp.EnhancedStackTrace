using CoreSharp.EnhancedStackTrace.Features.Reflection;
using CoreSharp.EnhancedStackTrace.Features.Serializers;
using CoreSharp.EnhancedStackTrace.Tests.Features.Serializers.Common;

namespace CoreSharp.EnhancedStackTrace.Tests.Features.Serializers;

internal sealed class MethodStackFrameSerializerTests : TestsBase
{
    [OneTimeSetUp]
    public void OneTimeSetUp()
        => ToStringTestArgs.Init(ToStringTestArgs.Source);


    [Test]
    public void CanSerialize_WhenFrameIsNull_ShouldThrowArgumentNullException()
    {
        // Arrange
        var serializer = MockCreate<MethodStackFrameSerializer>();

        // Act
        Action action = () => serializer.CanSerialize(frame: null!);

        // Assert
        action.Should().Throw<ArgumentNullException>();
    }

    [Test]
    public void CanSerialize_WhenMethodBaseIsNotMethodInfo_ShouldReturnFalse()
    {
        // Arrange
        var serializer = MockCreate<MethodStackFrameSerializer>();
        var frame = new MockStackFrame();

        var method = new MockConstructorInfo();
        frame.SetMethodBase(method);

        // Act
        var result = serializer.CanSerialize(frame);

        // Assert
        result.Should().BeFalse();
    }

    [Test]
    public void CanSerialize_WhenMethodBaseIsMethodInfo_ShouldReturnTrue()
    {
        // Arrange
        var serializer = MockCreate<MethodStackFrameSerializer>();
        var method = new MockMethodInfo();

        var frame = new MockStackFrame();
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
        var serializer = MockCreate<MethodStackFrameSerializer>();

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
        var serializer = new MethodStackFrameSerializer(
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
        public static IEnumerable<ToStringTestArgs> Source { get; } =
        [
            new()
            {
                Label = "ReturnsVoid",
                ThrowErrorFactory = MethodVoid,
                FileName = "MyFile",
                LineNumber = 10,
                ExpectedToString = "void CoreSharp.EnhancedStackTrace.Tests.Features.Serializers.MethodStackFrameSerializerTests+ToStringTestArgs.MethodVoid() in MyFile:line 10"
            },
            new()
            {
                Label = "HasReturnType",
                ThrowErrorFactory = () => MethodWithResult(),
                FileName = "MyFile",
                LineNumber = 10,
                ExpectedToString = "int CoreSharp.EnhancedStackTrace.Tests.Features.Serializers.MethodStackFrameSerializerTests+ToStringTestArgs.MethodWithResult() in MyFile:line 10"
            },
            new()
            {
                Label = "LocalMethod",
                ThrowErrorFactory = MethodWithLocalMethod,
                FileName = "MyFile",
                LineNumber = 10,
                ExpectedToString = "void CoreSharp.EnhancedStackTrace.Tests.Features.Serializers.MethodStackFrameSerializerTests+ToStringTestArgs.MethodWithLocalMethod()+LocalMethod() in MyFile:line 10"
            },
            new()
            {
                Label = "HasGenericArguments",
                ThrowErrorFactory = MethodWithGenericArguments<int>,
                FileName = "MyFile",
                LineNumber = 10,
                ExpectedToString = "void CoreSharp.EnhancedStackTrace.Tests.Features.Serializers.MethodStackFrameSerializerTests+ToStringTestArgs.MethodWithGenericArguments<TValue>() in MyFile:line 10"
            },
            new()
            {
                Label = "HasArguments",
                ThrowErrorFactory = () => MethodWithArguments(default),
                FileName = "MyFile",
                LineNumber = 10,
                ExpectedToString = "void CoreSharp.EnhancedStackTrace.Tests.Features.Serializers.MethodStackFrameSerializerTests+ToStringTestArgs.MethodWithArguments(int _) in MyFile:line 10"
            }
        ];

        private static void MethodVoid()
           => throw new Exception();

        private static int MethodWithResult()
            => throw new Exception();

        private static void MethodWithLocalMethod()
        {
            LocalMethod();

            static void LocalMethod()
                => throw new Exception();
        }

        private static void MethodWithGenericArguments<TValue>()
            => throw new Exception();

        private static void MethodWithArguments(int _)
            => throw new Exception();

    }
}

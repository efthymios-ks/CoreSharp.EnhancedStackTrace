using CoreSharp.EnhancedStackTrace.Features.Reflection;
using CoreSharp.EnhancedStackTrace.Features.Serializers;
using CoreSharp.EnhancedStackTrace.Tests.Features.Serializers.Common;
using NSubstitute;
using System.Diagnostics.CodeAnalysis;

namespace CoreSharp.EnhancedStackTrace.Tests.Features.Serializers;

internal sealed class LambdaStackFrameSerializerTests : TestsBase
{
    [OneTimeSetUp]
    public void OneTimeSetUp()
        => ToStringTestArgs.Init(ToStringTestArgs.Source);

    [Test]
    public void CanSerialize_WhenFrameIsNull_ShouldThrowArgumentNullException()
    {
        // Arrange
        var serializer = MockCreate<LambdaStackFrameSerializer>();

        // Act
        Action action = () => serializer.CanSerialize(frame: null!);

        // Assert
        action.Should().Throw<ArgumentNullException>();
    }

    [Test]
    public void CanSerialize_WhenMethodBaseIsNotMethodInfo_ShouldReturnFalse()
    {
        // Arrange
        var serializer = MockCreate<LambdaStackFrameSerializer>();
        var frame = new MockStackFrame();
        frame.SetMethodBase(new MockConstructorInfo());

        // Act
        var result = serializer.CanSerialize(frame);

        // Assert
        result.Should().BeFalse();
    }

    [Test]
    public void CanSerialize_WhenMethodBaseNameIsNotCctor_ShouldReturnFalse()
    {
        // Arrange
        var reflectionHelper = MockFreeze<IReflectionHelper>();
        var serializer = MockCreate<LambdaStackFrameSerializer>();
        var frame = new MockStackFrame();
        frame.SetMethodBase(new MockMethodInfo());

        reflectionHelper
            .DeconstructMethodName(default!)
            .ReturnsForAnyArgs(new DeconstructedMethodName(
                MethodName: "NotCctor",
                SubMethodName: null,
                SubMethodIdentifier: null));

        // Act
        var result = serializer.CanSerialize(frame);

        // Assert
        result.Should().BeFalse();
    }

    [Test]
    public void CanSerialize_WhenMethodBaseSubMethodIdentfierIsNotLambda_ShouldReturnFalse()
    {
        // Arrange
        var reflectionHelper = MockFreeze<IReflectionHelper>();
        var serializer = MockCreate<LambdaStackFrameSerializer>();
        var frame = new MockStackFrame();
        frame.SetMethodBase(new MockMethodInfo());

        reflectionHelper
            .DeconstructMethodName(default!)
            .ReturnsForAnyArgs(new DeconstructedMethodName(
                MethodName: ".cctor",
                SubMethodName: null,
                SubMethodIdentifier: null));

        // Act
        var result = serializer.CanSerialize(frame);

        // Assert
        result.Should().BeFalse();
    }

    [Test]
    public void CanSerialize_WhenMethodBaseIsLambda_ShouldReturnTrue()
    {
        // Arrange
        var reflectionHelper = MockFreeze<IReflectionHelper>();
        var serializer = MockCreate<LambdaStackFrameSerializer>();
        var frame = new MockStackFrame();
        frame.SetMethodBase(new MockMethodInfo());

        reflectionHelper
            .DeconstructMethodName(default!)
            .ReturnsForAnyArgs(new DeconstructedMethodName(
                MethodName: ".cctor",
                SubMethodName: null,
                SubMethodIdentifier: SubMethodIdentifier.LambdaMethod));

        // Act
        var result = serializer.CanSerialize(frame);

        // Assert
        result.Should().BeTrue();
    }

    [Test]
    public void ToString_WhenFrameIsNull_ShouldThrowArgumentNullException()
    {
        // Arrange
        var serializer = MockCreate<AsyncStateStackFrameSerializer>();

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
        var serializer = new LambdaStackFrameSerializer(
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
                Label = "AutoPropertyBackingField",
                ThrowErrorFactory = () => AutoPropertyBackingField!(),
                FileName = "MyFile",
                LineNumber = 10,
                ExpectedToString = "Action CoreSharp.EnhancedStackTrace.Tests.Features.Serializers.CoreSharp.EnhancedStackTrace.Tests.Features.Serializers.LambdaStackFrameSerializerTests+ToStringTestArgs.AutoPropertyBackingField in MyFile:line 10"
            },
            new()
            {
                Label = "Action",
                ThrowErrorFactory = () => Action!(),
                FileName = "MyFile",
                LineNumber = 10,
                ExpectedToString = "Action CoreSharp.EnhancedStackTrace.Tests.Features.Serializers.CoreSharp.EnhancedStackTrace.Tests.Features.Serializers.LambdaStackFrameSerializerTests+ToStringTestArgs.Action() in MyFile:line 10"
            },
            new()
            {
                Label = "Func<T>",
                ThrowErrorFactory = () => Func!(),
                FileName = "MyFile",
                LineNumber = 10,
                ExpectedToString = "Func<int> CoreSharp.EnhancedStackTrace.Tests.Features.Serializers.CoreSharp.EnhancedStackTrace.Tests.Features.Serializers.LambdaStackFrameSerializerTests+ToStringTestArgs.Func() in MyFile:line 10"
            },
            new()
            {
                Label = "Action<T>",
                ThrowErrorFactory = () => ActionWithArgument!(default),
                FileName = "MyFile",
                LineNumber = 10,
                ExpectedToString = "Action<int> CoreSharp.EnhancedStackTrace.Tests.Features.Serializers.CoreSharp.EnhancedStackTrace.Tests.Features.Serializers.LambdaStackFrameSerializerTests+ToStringTestArgs.ActionWithArgument() in MyFile:line 10"
            },
        ];

        private static Action AutoPropertyBackingField { get; }
            = () => throw new Exception();

        [SuppressMessage("Style", "IDE1006:Naming Styles", Justification = "<Pending>")]
        private static readonly Action Action = () => throw new Exception();

        [SuppressMessage("Style", "IDE1006:Naming Styles", Justification = "<Pending>")]
        private static readonly Func<int> Func = () => throw new Exception();

        [SuppressMessage("Style", "IDE1006:Naming Styles", Justification = "<Pending>")]
        private static readonly Action<int> ActionWithArgument = _ => throw new Exception();
    }
}

using CoreSharp.EnhancedStackTrace.Features.Reflection;
using CoreSharp.EnhancedStackTrace.Features.Serializers;
using CoreSharp.EnhancedStackTrace.Tests.Features.Serializers.Common;
using Tests.Common.Mocks;

namespace CoreSharp.EnhancedStackTrace.Tests.Features.Serializers;

public sealed class LambdaStackFrameSerializerTests : StackFrameSerializerTestsBase
{
    [Fact]
    public void CanSerialize_WhenFrameIsNull_ShouldThrowArgumentNullException()
    {
        // Arrange
        var serializer = MockCreate<LambdaStackFrameSerializer>();

        // Act
        void Action()
            => serializer.CanSerialize(frame: null!);

        // Assert
        Assert.Throws<ArgumentNullException>(Action);
    }

    [Fact]
    public void CanSerialize_WhenMethodBaseIsNotMethodInfo_ShouldReturnFalse()
    {
        // Arrange
        var serializer = MockCreate<LambdaStackFrameSerializer>();
        var frame = new MockStackFrame();
        frame.SetMethodBase(new MockConstructorInfo());

        // Act
        var result = serializer.CanSerialize(frame);

        // Assert
        Assert.False(result);
    }

    [Fact]
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
        Assert.False(result);
    }

    [Fact]
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
        Assert.False(result);
    }

    [Fact]
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
        Assert.True(result);
    }

    [Fact]
    public void ToString_WhenFrameIsNull_ShouldThrowArgumentNullException()
    {
        // Arrange
        var serializer = MockCreate<AsyncStateStackFrameSerializer>();

        // Act
        void Action()
            => serializer.ToString(frame: null!);

        // Assert
        Assert.Throws<ArgumentNullException>(Action);
    }

    [Fact]
    public void ToString_WhenStackFrameIsAutoPropertyBackingField_ShouldReturnCorrectValue()
    {
        // Arrange
        var reflectionHelper = new ReflectionHelper();
        var typeAliasProvider = new TypeAliasProvider(reflectionHelper);
        var parameterInfoHelper = new ParameterInfoHelper(reflectionHelper, typeAliasProvider);
        var serializer = new LambdaStackFrameSerializer(
            typeAliasProvider,
            parameterInfoHelper,
            reflectionHelper);

        var stackFrame = RunAndCapture(
            () => ErrorFactory.AutoPropertyBackingField(),
            fileName: "MyFile",
            lineNumber: 10
        );

        // Act
        var result = serializer.ToString(stackFrame);

        // Assert
        Assert.Equal("Action CoreSharp.EnhancedStackTrace.Tests.Features.Serializers.LambdaStackFrameSerializerTests+ErrorFactory.AutoPropertyBackingField in MyFile:line 10", result);
    }

    [Fact]
    public void ToString_WhenStackFrameIsAction_ShouldReturnCorrectValue()
    {
        // Arrange
        var reflectionHelper = new ReflectionHelper();
        var typeAliasProvider = new TypeAliasProvider(reflectionHelper);
        var parameterInfoHelper = new ParameterInfoHelper(reflectionHelper, typeAliasProvider);
        var serializer = new LambdaStackFrameSerializer(
            typeAliasProvider,
            parameterInfoHelper,
            reflectionHelper);

        var stackFrame = RunAndCapture(
            () => ErrorFactory.Action(),
            fileName: "MyFile",
            lineNumber: 10
        );

        // Act
        var result = serializer.ToString(stackFrame);

        // Assert
        Assert.Equal("Action CoreSharp.EnhancedStackTrace.Tests.Features.Serializers.LambdaStackFrameSerializerTests+ErrorFactory.Action() in MyFile:line 10", result);
    }

    [Fact]
    public void ToString_WhenStackFrameIsFunc_ShouldReturnCorrectValue()
    {
        // Arrange
        var reflectionHelper = new ReflectionHelper();
        var typeAliasProvider = new TypeAliasProvider(reflectionHelper);
        var parameterInfoHelper = new ParameterInfoHelper(reflectionHelper, typeAliasProvider);
        var serializer = new LambdaStackFrameSerializer(
            typeAliasProvider,
            parameterInfoHelper,
            reflectionHelper);

        var stackFrame = RunAndCapture(
            () => _ = ErrorFactory.Func(),
            fileName: "MyFile",
            lineNumber: 10
        );

        // Act
        var result = serializer.ToString(stackFrame);

        // Assert
        Assert.Equal("Func<int> CoreSharp.EnhancedStackTrace.Tests.Features.Serializers.LambdaStackFrameSerializerTests+ErrorFactory.Func() in MyFile:line 10", result);
    }

    [Fact]
    public void ToString_WhenStackFrameIsActionWithArgument_ShouldReturnCorrectValue()
    {
        // Arrange
        var reflectionHelper = new ReflectionHelper();
        var typeAliasProvider = new TypeAliasProvider(reflectionHelper);
        var parameterInfoHelper = new ParameterInfoHelper(reflectionHelper, typeAliasProvider);
        var serializer = new LambdaStackFrameSerializer(
            typeAliasProvider,
            parameterInfoHelper,
            reflectionHelper);

        var stackFrame = RunAndCapture(
            () => ErrorFactory.ActionWithArgument(default),
            fileName: "MyFile",
            lineNumber: 10
        );

        // Act
        var result = serializer.ToString(stackFrame);

        // Assert
        Assert.Equal("Action<int> CoreSharp.EnhancedStackTrace.Tests.Features.Serializers.LambdaStackFrameSerializerTests+ErrorFactory.ActionWithArgument() in MyFile:line 10", result);
    }

    private static class ErrorFactory
    {
        public static Action AutoPropertyBackingField { get; }
            = () => throw new Exception();

        public static readonly Action Action = () => throw new Exception();

        public static readonly Func<int> Func = () => throw new Exception();

        public static readonly Action<int> ActionWithArgument = _ => throw new Exception();
    }
}

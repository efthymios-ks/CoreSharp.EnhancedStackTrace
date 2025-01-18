using CoreSharp.EnhancedStackTrace.Features.Reflection;
using CoreSharp.EnhancedStackTrace.Features.Serializers;
using CoreSharp.EnhancedStackTrace.Tests.Features.Serializers.Common;
using Tests.Common.Mocks;

namespace CoreSharp.EnhancedStackTrace.Tests.Features.Serializers;

public sealed class ConstructorStackFrameSerializerTests : StackFrameSerializerTestsBase
{
    [Fact]
    public void CanSerialize_WhenFrameIsNull_ShouldThrowArgumentNullException()
    {
        // Arrange
        var serializer = MockCreate<ConstructorStackFrameSerializer>();

        // Act
        void Action()
            => serializer.CanSerialize(frame: null!);

        // Assert
        Assert.Throws<ArgumentNullException>(Action);
    }

    [Fact]
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
        Assert.False(result);
    }

    [Fact]
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
        Assert.True(result);
    }

    [Fact]
    public void ToString_WhenFrameIsNull_ShouldThrowArgumentNullException()
    {
        // Arrange
        var serializer = MockCreate<ConstructorStackFrameSerializer>();

        // Act
        void Action()
            => serializer.ToString(frame: null!);

        // Assert
        Assert.Throws<ArgumentNullException>(Action);
    }

    [Fact]
    public void ToString_WhenStackFrameIsConstructorWithGenericArguments_ShouldReturnCorrectValue()
    {
        // Arrange
        var reflectionHelper = new ReflectionHelper();
        var typeAliasProvider = new TypeAliasProvider(reflectionHelper);
        var parameterInfoHelper = new ParameterInfoHelper(reflectionHelper, typeAliasProvider);
        var serializer = new ConstructorStackFrameSerializer(
            typeAliasProvider,
            parameterInfoHelper,
            reflectionHelper);

        var stackFrame = RunAndCapture(
            () => _ = new ErrorFactory.ConstructorGeneric<int>(),
            fileName: "MyFile",
            lineNumber: 10
        );

        // Act
        var result = serializer.ToString(stackFrame);

        // Assert
        Assert.Equal("new CoreSharp.EnhancedStackTrace.Tests.Features.Serializers.ConstructorStackFrameSerializerTests+ErrorFactory+ConstructorGeneric<TValue>() in MyFile:line 10", result);
    }

    [Fact]
    public void ToString_WhenStackFrameIsConstructorWithArguments_ShouldReturnCorrectValue()
    {
        // Arrange
        var reflectionHelper = new ReflectionHelper();
        var typeAliasProvider = new TypeAliasProvider(reflectionHelper);
        var parameterInfoHelper = new ParameterInfoHelper(reflectionHelper, typeAliasProvider);
        var serializer = new ConstructorStackFrameSerializer(
            typeAliasProvider,
            parameterInfoHelper,
            reflectionHelper);

        var stackFrame = RunAndCapture(
            () => _ = new ErrorFactory.ConstructorWithArguments(default),
            fileName: "MyFile",
            lineNumber: 10
        );

        // Act
        var result = serializer.ToString(stackFrame);

        // Assert
        Assert.Equal("new CoreSharp.EnhancedStackTrace.Tests.Features.Serializers.ConstructorStackFrameSerializerTests+ErrorFactory+ConstructorWithArguments(int argument) in MyFile:line 10", result);
    }

    [Fact]
    public void ToString_WhenStackFrameIsStaticConstructor_ShouldReturnCorrectValue()
    {
        // Arrange
        var reflectionHelper = new ReflectionHelper();
        var typeAliasProvider = new TypeAliasProvider(reflectionHelper);
        var parameterInfoHelper = new ParameterInfoHelper(reflectionHelper, typeAliasProvider);
        var serializer = new ConstructorStackFrameSerializer(
            typeAliasProvider,
            parameterInfoHelper,
            reflectionHelper);

        var stackFrame = RunAndCapture(
            ErrorFactory.ConstructorStatic.Reference,
            fileName: "MyFile",
            lineNumber: 10
        );

        // Act
        var result = serializer.ToString(stackFrame);

        // Assert
        Assert.Equal("static CoreSharp.EnhancedStackTrace.Tests.Features.Serializers.ConstructorStackFrameSerializerTests+ErrorFactory+ConstructorStatic() in MyFile:line 10", result);
    }

    private static class ErrorFactory
    {
        public sealed class ConstructorGeneric<TValue>
        {
            public ConstructorGeneric()
                => throw new Exception();
        }

        public sealed class ConstructorWithArguments
        {
            public ConstructorWithArguments(int argument)
                => throw new Exception();
        }

        public static class ConstructorStatic
        {
            static ConstructorStatic()
                => throw new Exception();

            public static void Reference()
            {
            }
        }
    }
}

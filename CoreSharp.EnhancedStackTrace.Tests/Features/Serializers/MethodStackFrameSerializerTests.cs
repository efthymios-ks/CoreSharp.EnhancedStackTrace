using CoreSharp.EnhancedStackTrace.Features.Reflection;
using CoreSharp.EnhancedStackTrace.Features.Serializers;
using CoreSharp.EnhancedStackTrace.Tests.Features.Serializers.Common;
using Tests.Common.Mocks;

namespace CoreSharp.EnhancedStackTrace.Tests.Features.Serializers;

public sealed class MethodStackFrameSerializerTests : StackFrameSerializerTestsBase
{
    [Fact]
    public void CanSerialize_WhenFrameIsNull_ShouldThrowArgumentNullException()
    {
        // Arrange
        var serializer = MockCreate<MethodStackFrameSerializer>();

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
        var serializer = MockCreate<MethodStackFrameSerializer>();
        var frame = new MockStackFrame();

        var method = new MockConstructorInfo();
        frame.SetMethodBase(method);

        // Act
        var result = serializer.CanSerialize(frame);

        // Assert
        Assert.False(result);
    }

    [Fact]
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
        Assert.True(result);
    }

    [Fact]
    public void ToString_WhenFrameIsNull_ShouldThrowArgumentNullException()
    {
        // Arrange
        var serializer = MockCreate<MethodStackFrameSerializer>();

        // Act
        void Action()
            => serializer.ToString(frame: null!);

        // Assert
        Assert.Throws<ArgumentNullException>(Action);
    }

    [Fact]
    public void ToString_WhenStackFrameIsMethodWithNoReturnType_ShouldReturnCorrectValue()
    {
        // Arrange
        var reflectionHelper = new ReflectionHelper();
        var typeAliasProvider = new TypeAliasProvider(reflectionHelper);
        var parameterInfoHelper = new ParameterInfoHelper(reflectionHelper, typeAliasProvider);
        var serializer = new MethodStackFrameSerializer(
            typeAliasProvider,
            parameterInfoHelper,
            reflectionHelper);

        var stackFrame = RunAndCapture(
            ErrorFactory.MethodWithNoReturnType,
            fileName: "MyFile",
            lineNumber: 10
        );

        // Act
        var result = serializer.ToString(stackFrame);

        // Assert
        Assert.Equal("void CoreSharp.EnhancedStackTrace.Tests.Features.Serializers.MethodStackFrameSerializerTests+ErrorFactory.MethodWithNoReturnType() in MyFile:line 10", result);
    }
    [Fact]
    public void ToString_WhenStackFrameIsMethodWithReturnType_ShouldReturnCorrectValue()
    {
        // Arrange
        var reflectionHelper = new ReflectionHelper();
        var typeAliasProvider = new TypeAliasProvider(reflectionHelper);
        var parameterInfoHelper = new ParameterInfoHelper(reflectionHelper, typeAliasProvider);
        var serializer = new MethodStackFrameSerializer(
            typeAliasProvider,
            parameterInfoHelper,
            reflectionHelper);

        var stackFrame = RunAndCapture(
            () => _ = ErrorFactory.MethodWithReturnType(),
            fileName: "MyFile",
            lineNumber: 10
        );

        // Act
        var result = serializer.ToString(stackFrame);

        // Assert
        Assert.Equal("int CoreSharp.EnhancedStackTrace.Tests.Features.Serializers.MethodStackFrameSerializerTests+ErrorFactory.MethodWithReturnType() in MyFile:line 10", result);
    }

    [Fact]
    public void ToString_WhenStackFrameIsLocalMethod_ShouldReturnCorrectValue()
    {
        // Arrange
        var reflectionHelper = new ReflectionHelper();
        var typeAliasProvider = new TypeAliasProvider(reflectionHelper);
        var parameterInfoHelper = new ParameterInfoHelper(reflectionHelper, typeAliasProvider);
        var serializer = new MethodStackFrameSerializer(
            typeAliasProvider,
            parameterInfoHelper,
            reflectionHelper);

        var stackFrame = RunAndCapture(
            ErrorFactory.MethodWithLocalMethod,
            fileName: "MyFile",
            lineNumber: 10
        );

        // Act
        var result = serializer.ToString(stackFrame);

        // Assert
        Assert.Equal("void CoreSharp.EnhancedStackTrace.Tests.Features.Serializers.MethodStackFrameSerializerTests+ErrorFactory.MethodWithLocalMethod()+LocalMethod() in MyFile:line 10", result);
    }

    [Fact]
    public void ToString_WhenStackFrameIsMethodWithGenericArguments_ShouldReturnCorrectValue()
    {
        // Arrange
        var reflectionHelper = new ReflectionHelper();
        var typeAliasProvider = new TypeAliasProvider(reflectionHelper);
        var parameterInfoHelper = new ParameterInfoHelper(reflectionHelper, typeAliasProvider);
        var serializer = new MethodStackFrameSerializer(
            typeAliasProvider,
            parameterInfoHelper,
            reflectionHelper);

        var stackFrame = RunAndCapture(
            ErrorFactory.MethodWithGenericArguments<int>,
            fileName: "MyFile",
            lineNumber: 10
        );

        // Act
        var result = serializer.ToString(stackFrame);

        // Assert
        Assert.Equal("void CoreSharp.EnhancedStackTrace.Tests.Features.Serializers.MethodStackFrameSerializerTests+ErrorFactory.MethodWithGenericArguments<TValue>() in MyFile:line 10", result);
    }

    [Fact]
    public void ToString_WhenStackFrameIsMethodWithArguments_ShouldReturnCorrectValue()
    {
        // Arrange
        var reflectionHelper = new ReflectionHelper();
        var typeAliasProvider = new TypeAliasProvider(reflectionHelper);
        var parameterInfoHelper = new ParameterInfoHelper(reflectionHelper, typeAliasProvider);
        var serializer = new MethodStackFrameSerializer(
            typeAliasProvider,
            parameterInfoHelper,
            reflectionHelper);

        var stackFrame = RunAndCapture(
            () => ErrorFactory.MethodWithArguments(default),
            fileName: "MyFile",
            lineNumber: 10
        );

        // Act
        var result = serializer.ToString(stackFrame);

        // Assert
        Assert.Equal("void CoreSharp.EnhancedStackTrace.Tests.Features.Serializers.MethodStackFrameSerializerTests+ErrorFactory.MethodWithArguments(int _) in MyFile:line 10", result);
    }

    [Fact]
    public void ToString_WhenStackFrameIsGetIndexer_ShouldReturnCorrectValue()
    {
        // Arrange
        var reflectionHelper = new ReflectionHelper();
        var typeAliasProvider = new TypeAliasProvider(reflectionHelper);
        var parameterInfoHelper = new ParameterInfoHelper(reflectionHelper, typeAliasProvider);
        var serializer = new MethodStackFrameSerializer(
            typeAliasProvider,
            parameterInfoHelper,
            reflectionHelper);

        var stackFrame = RunAndCapture(
            ErrorFactory.IndexerGet,
            fileName: "MyFile",
            lineNumber: 10
        );

        // Act
        var result = serializer.ToString(stackFrame);

        // Assert
        Assert.Equal("int CoreSharp.EnhancedStackTrace.Tests.Features.Serializers.MethodStackFrameSerializerTests+ErrorFactory+ClassWithIndexer[int index] in MyFile:line 10", result);
    }

    [Fact]
    public void ToString_WhenStackFrameIsSetIndexer_ShouldReturnCorrectValue()
    {
        // Arrange
        var reflectionHelper = new ReflectionHelper();
        var typeAliasProvider = new TypeAliasProvider(reflectionHelper);
        var parameterInfoHelper = new ParameterInfoHelper(reflectionHelper, typeAliasProvider);
        var serializer = new MethodStackFrameSerializer(
            typeAliasProvider,
            parameterInfoHelper,
            reflectionHelper);

        var stackFrame = RunAndCapture(
            ErrorFactory.IndexerSet,
            fileName: "MyFile",
            lineNumber: 10
        );

        // Act
        var result = serializer.ToString(stackFrame);

        // Assert
        Assert.Equal("void CoreSharp.EnhancedStackTrace.Tests.Features.Serializers.MethodStackFrameSerializerTests+ErrorFactory+ClassWithIndexer[int index, int value] in MyFile:line 10", result);
    }

    private static class ErrorFactory
    {
        public static void MethodWithNoReturnType()
           => throw new Exception();

        public static int MethodWithReturnType()
            => throw new Exception();

        public static void MethodWithLocalMethod()
        {
            LocalMethod();

            static void LocalMethod()
                => throw new Exception();
        }

        public static void MethodWithGenericArguments<TValue>()
            => throw new Exception();

        public static void MethodWithArguments(int _)
            => throw new Exception();

        public static void IndexerGet()
            => _ = new ClassWithIndexer()[0];

        public static void IndexerSet()
            => new ClassWithIndexer()[0] = default;

        private sealed class ClassWithIndexer
        {
            public int this[int index]
            {
                get => throw new Exception();
                set => throw new Exception();
            }
        }
    }
}

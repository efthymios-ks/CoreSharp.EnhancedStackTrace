using CoreSharp.EnhancedStackTrace.Features.Reflection;
using CoreSharp.EnhancedStackTrace.Features.Serializers;
using CoreSharp.EnhancedStackTrace.Tests.Features.Serializers.Common;
using System.Runtime.CompilerServices;
using Tests.Common.Mocks;

namespace CoreSharp.EnhancedStackTrace.Tests.Features.Serializers;

public sealed partial class AsyncStateStackFrameSerializerTests : StackFrameSerializerTestsBase
{
    [Fact]
    public void CanSerialize_WhenFrameIsNull_ShouldThrowArgumentNullException()
    {
        // Arrange
        var serializer = MockCreate<AsyncStateStackFrameSerializer>();

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
        var serializer = MockCreate<AsyncStateStackFrameSerializer>();
        var frame = new MockStackFrame();

        var method = new MockConstructorInfo();
        frame.SetMethodBase(method);

        // Act
        var result = serializer.CanSerialize(frame);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public void CanSerialize_WhenMethodBaseReflectedTypeIsNull_ShouldReturnFalse()
    {
        // Arrange
        var serializer = MockCreate<AsyncStateStackFrameSerializer>();

        var method = new MockMethodInfo
        {
            ReflectedTypeOverride = null
        };

        var frame = new MockStackFrame();
        frame.SetMethodBase(method);

        // Act
        var result = serializer.CanSerialize(frame);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public void CanSerialize_WhenMethodBaseReflectedTypeDoesNotContainIAsyncStateMachine_ShouldReturnFalse()
    {
        // Arrange
        var serializer = MockCreate<AsyncStateStackFrameSerializer>();
        var method = new MockMethodInfo
        {
            ReflectedTypeOverride = new MockType
            {
                GetInterfacesOverride = []
            }
        };

        var frame = new MockStackFrame();
        frame.SetMethodBase(method);

        // Act
        var result = serializer.CanSerialize(frame);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public void CanSerialize_WhenMethodBaseReflectedTypeContainsAsyncStateMachine_ShouldReturnTrue()
    {
        // Arrange
        var serializer = MockCreate<AsyncStateStackFrameSerializer>();
        var method = new MockMethodInfo
        {
            ReflectedTypeOverride = new MockType
            {
                GetInterfacesOverride = [typeof(IAsyncStateMachine)]
            }
        };

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
        var serializer = MockCreate<AsyncStateStackFrameSerializer>();

        // Act
        void Action()
            => serializer.ToString(frame: null!);

        // Assert
        Assert.Throws<ArgumentNullException>(Action);
    }

    [Fact]
    public async Task ToString_WhenStackFrameIsTaskWithNoReturnType_ShouldReturnCorrectValue()
    {
        // Arrange
        var reflectionHelper = new ReflectionHelper();
        var typeAliasProvider = new TypeAliasProvider(reflectionHelper);
        var parameterInfoHelper = new ParameterInfoHelper(reflectionHelper, typeAliasProvider);
        var serializer = new AsyncStateStackFrameSerializer(
            typeAliasProvider,
            parameterInfoHelper,
            reflectionHelper);

        var stackFrame = await RunAndCaptureAsync(
            ErrorFactory.TaskWithNoReturnType,
            fileName: "MyFile",
            lineNumber: 10
        );

        // Act
        var result = serializer.ToString(stackFrame);

        // Assert
        Assert.Equal("async Task CoreSharp.EnhancedStackTrace.Tests.Features.Serializers.AsyncStateStackFrameSerializerTests+ErrorFactory.TaskWithNoReturnType() in MyFile:line 10", result);
    }

    [Fact]
    public async Task ToString_WhenStackFrameIsTaskWithReturnType_ShouldReturnCorrectValue()
    {
        // Arrange
        var reflectionHelper = new ReflectionHelper();
        var typeAliasProvider = new TypeAliasProvider(reflectionHelper);
        var parameterInfoHelper = new ParameterInfoHelper(reflectionHelper, typeAliasProvider);
        var serializer = new AsyncStateStackFrameSerializer(
            typeAliasProvider,
            parameterInfoHelper,
            reflectionHelper);

        var stackFrame = await RunAndCaptureAsync(
            ErrorFactory.TaskWithReturnType,
            fileName: "MyFile",
            lineNumber: 10
        );

        // Act
        var result = serializer.ToString(stackFrame);

        // Assert
        Assert.Equal("async Task<int> CoreSharp.EnhancedStackTrace.Tests.Features.Serializers.AsyncStateStackFrameSerializerTests+ErrorFactory.TaskWithReturnType() in MyFile:line 10", result);
    }

    [Fact]
    public async Task ToString_WhenStackFrameIsTaskWithGenericArguments_ShouldReturnCorrectValue()
    {
        // Arrange
        var reflectionHelper = new ReflectionHelper();
        var typeAliasProvider = new TypeAliasProvider(reflectionHelper);
        var parameterInfoHelper = new ParameterInfoHelper(reflectionHelper, typeAliasProvider);
        var serializer = new AsyncStateStackFrameSerializer(
            typeAliasProvider,
            parameterInfoHelper,
            reflectionHelper);

        var stackFrame = await RunAndCaptureAsync(
            ErrorFactory.TaskWithGenericArguments<int>,
            fileName: "MyFile",
            lineNumber: 10
        );

        // Act
        var result = serializer.ToString(stackFrame);

        // Assert
        Assert.Equal("async Task CoreSharp.EnhancedStackTrace.Tests.Features.Serializers.AsyncStateStackFrameSerializerTests+ErrorFactory.TaskWithGenericArguments<TValue>() in MyFile:line 10", result);
    }

    [Fact]
    public async Task ToString_WhenStackFrameIsTaskWithMethodArguments_ShouldReturnCorrectValue()
    {
        // Arrange
        var reflectionHelper = new ReflectionHelper();
        var typeAliasProvider = new TypeAliasProvider(reflectionHelper);
        var parameterInfoHelper = new ParameterInfoHelper(reflectionHelper, typeAliasProvider);
        var serializer = new AsyncStateStackFrameSerializer(
            typeAliasProvider,
            parameterInfoHelper,
            reflectionHelper);

        var stackFrame = await RunAndCaptureAsync(
           () => ErrorFactory.TaskWithMethodArguments(default),
           fileName: "MyFile",
           lineNumber: 10
        );

        // Act
        var result = serializer.ToString(stackFrame);

        // Assert
        Assert.Equal("async Task CoreSharp.EnhancedStackTrace.Tests.Features.Serializers.AsyncStateStackFrameSerializerTests+ErrorFactory.TaskWithMethodArguments(int _) in MyFile:line 10", result);
    }

    private static class ErrorFactory
    {
        public static async Task TaskWithNoReturnType()
            => await Task.FromException(new Exception());

        public static async Task<int> TaskWithReturnType()
            => await Task.FromException<int>(new Exception());

        public static async Task TaskWithGenericArguments<TValue>()
            => await Task.FromException(new Exception());

        public static async Task TaskWithMethodArguments(int _)
            => await Task.FromException(new Exception());
    }
}

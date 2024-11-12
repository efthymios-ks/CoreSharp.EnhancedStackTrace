using CoreSharp.EnhancedStackTrace.Features.Reflection;
using CoreSharp.EnhancedStackTrace.Features.Serializers;
using CoreSharp.EnhancedStackTrace.Tests.Features.Serializers.Common;
using System.Runtime.CompilerServices;

namespace CoreSharp.EnhancedStackTrace.Tests.Features.Serializers;

internal sealed partial class AsyncStateStackFrameSerializerTests : TestsBase
{
    [OneTimeSetUp]
    public async Task OneTimeSetUp()
        => await ToStringTestArgs.InitAsync(ToStringTestArgs.Source);

    [Test]
    public void CanSerialize_WhenFrameIsNull_ShouldThrowArgumentNullException()
    {
        // Arrange
        var serializer = MockCreate<AsyncStateStackFrameSerializer>();

        // Act
        Action action = () => serializer.CanSerialize(frame: null!);

        // Assert
        action.Should().Throw<ArgumentNullException>();
    }

    [Test]
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
        result.Should().BeFalse();
    }

    [Test]
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
        result.Should().BeFalse();
    }

    [Test]
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
        result.Should().BeFalse();
    }

    [Test]
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
        var serializer = new AsyncStateStackFrameSerializer(
            typeAliasProvider,
            parameterInfoHelper,
            reflectionHelper);

        // Act
        var result = serializer.ToString(arguments.StackFrame);

        // Assert
        result.Should().Be(arguments.ExpectedToString);
    }

    public sealed class ToStringTestArgs : AsyncToStringArgsBase<ToStringTestArgs>
    {
        public static IEnumerable<ToStringTestArgs> Source { get; } =
        [
            new ()
            {
                Label = "ReturnsTask",
                ThrowErrorFactory = TaskWithNoReturnType,
                FileName = "MyFile",
                LineNumber = 10,
                ExpectedToString = "async Task CoreSharp.EnhancedStackTrace.Tests.Features.Serializers.AsyncStateStackFrameSerializerTests+ToStringTestArgs.TaskWithNoReturnType() in MyFile:line 10"
            },
            new ()
            {
                Label = "ReturnsTask<T>",
                ThrowErrorFactory = TaskWithReturnType,
                FileName = "MyFile",
                LineNumber = 10,
                ExpectedToString = "async Task<int> CoreSharp.EnhancedStackTrace.Tests.Features.Serializers.AsyncStateStackFrameSerializerTests+ToStringTestArgs.TaskWithReturnType() in MyFile:line 10"
            },
            new ()
            {
                Label = "HasGenericArguments",
                ThrowErrorFactory = TaskWithGenericArguments<int>,
                FileName = "MyFile",
                LineNumber = 10,
                ExpectedToString = "async Task CoreSharp.EnhancedStackTrace.Tests.Features.Serializers.AsyncStateStackFrameSerializerTests+ToStringTestArgs.TaskWithGenericArguments<TValue>() in MyFile:line 10"
            },
            new()
            {
                Label = "HasArguments",
                ThrowErrorFactory = () => TaskWithMethodArguments(default),
                FileName = "MyFile",
                LineNumber = 10,
                ExpectedToString = "async Task CoreSharp.EnhancedStackTrace.Tests.Features.Serializers.AsyncStateStackFrameSerializerTests+ToStringTestArgs.TaskWithMethodArguments(int _) in MyFile:line 10"
            }
        ];

        private static async Task TaskWithNoReturnType()
            => await Task.FromException(new Exception());

        private static async Task<int> TaskWithReturnType()
            => await Task.FromException<int>(new Exception());

        private static async Task TaskWithGenericArguments<TValue>()
            => await Task.FromException(new Exception());

        private static async Task TaskWithMethodArguments(int _)
            => await Task.FromException(new Exception());
    }
}

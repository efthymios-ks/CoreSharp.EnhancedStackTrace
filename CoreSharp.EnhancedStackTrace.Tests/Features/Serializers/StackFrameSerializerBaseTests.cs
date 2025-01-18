using CoreSharp.EnhancedStackTrace.Features.Reflection;
using CoreSharp.EnhancedStackTrace.Features.Serializers;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using Tests.Common.Mocks;

namespace CoreSharp.EnhancedStackTrace.Tests.Features.Serializers;

public sealed class StackFrameSerializerBaseTests : TestsBase
{
    [Fact]
    public void GetAlias_WhenTypeIsNull_ThrowsArgumentNullException()
    {
        // Arrange
        var stackFrameSerializer = MockCreate<MockStackFrameSerializer>();
        Type type = null!;

        // Act
        void Action()
            => stackFrameSerializer.GetAlias(type);

        // Assert
        Assert.Throws<ArgumentNullException>(Action);
    }

    [Fact]
    public void GetAlias_WhenCalledWithType_ShouldReturnTypeAlias()
    {
        // Arrange
        var typeAliasProvider = MockFreeze<ITypeAliasProvider>();
        var stackFrameSerializer = MockCreate<MockStackFrameSerializer>();
        var type = typeof(int);

        typeAliasProvider
            .GetAlias(default!)
            .ReturnsForAnyArgs("alias");

        // Act
        var result = stackFrameSerializer.GetAlias(type);

        // Assert
        Assert.Equal("alias", result);
    }

    [Fact]
    public void GetAlias_WhenParameterIsNull_ThrowsArgumentNullException()
    {
        // Arrange
        var stackFrameSerializer = MockCreate<MockStackFrameSerializer>();
        ParameterInfo parameter = null!;

        // Act
        void Action()
            => stackFrameSerializer.GetAlias(parameter);

        // Assert
        Assert.Throws<ArgumentNullException>(Action);
    }

    [Fact]
    public void GetAlias_WhenCalledWithParameterInfo_ShouldReturnParameterInfoAlias()
    {
        // Arrange
        var parameterInfoHelper = MockFreeze<IParameterInfoHelper>();
        var stackFrameSerializer = MockCreate<MockStackFrameSerializer>();
        var parameter = new MockParameterInfo();

        parameterInfoHelper
            .GetAlias(default!)
            .ReturnsForAnyArgs("alias");

        // Act
        var result = stackFrameSerializer.GetAlias(parameter);

        // Assert
        Assert.Equal("alias", result);
    }

    [Fact]
    public void DeconstructMethodName_WhenRawMethodNameIsNull_ShouldThrowArgumentNullException()
    {
        // Arrange
        var stackFrameSerializer = MockCreate<MockStackFrameSerializer>();

        // Act
        void Action()
            => stackFrameSerializer.DeconstructMethodName(rawMethodName: null!);

        // Assert
        Assert.Throws<ArgumentNullException>(Action);
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    public void DeconstructMethodName_WhenRawMethodNameIsEmpty_ShouldThrowArgumentException(string rawMethodName)
    {
        // Arrange
        var stackFrameSerializer = MockCreate<MockStackFrameSerializer>();

        // Act
        void Action()
            => stackFrameSerializer.DeconstructMethodName(rawMethodName);

        // Assert
        Assert.Throws<ArgumentException>(Action);
    }

    [Fact]
    public void DeconstructMethodName_WhenRawMethodNameIsNotSubMethod_ShouldReturnMethodNameAndNullSubMethodName()
    {
        // Arrange
        var reflectionHelper = MockFreeze<IReflectionHelper>();
        var stackFrameSerializer = MockCreate<MockStackFrameSerializer>();
        const string rawMethodName = "MethodName";

        reflectionHelper
            .DeconstructMethodName(default!)
            .ReturnsForAnyArgs(new DeconstructedMethodName("MethodName", "SubMethodName", SubMethodIdentifier.LocalFunction));

        // Act
        var (methodName, subMethodName, subMethodIdentifier) = stackFrameSerializer.DeconstructMethodName(rawMethodName);

        // Assert
        Assert.Equal("MethodName", methodName);
        Assert.Equal("SubMethodName", subMethodName);
        Assert.Equal(SubMethodIdentifier.LocalFunction, subMethodIdentifier);
    }

    [Fact]
    public void GetMethodsArgumentsAsString_WhenMethodBaseIsNull_ThrowsArgumentNullException()
    {
        // Arrange
        var stackFrameSerializer = MockCreate<MockStackFrameSerializer>();
        MethodBase methodBase = null!;

        // Act
        void Action()
            => stackFrameSerializer.GetMethodsArgumentsAsString(methodBase);

        // Assert
        Assert.Throws<ArgumentNullException>(Action);
    }

    [Fact]
    public void GetMethodsArgumentsAsString_WhenMethodHasNoArguments_ShouldReturnEmptyString()
    {
        // Arrange
        var stackFrameSerializer = MockCreate<MockStackFrameSerializer>();
        var method = typeof(MockStackFrameSerializer).GetMethod(nameof(MockStackFrameSerializer.MethodWithoutArguments))!;

        // Act
        var result = stackFrameSerializer.GetMethodsArgumentsAsString(method);

        // Assert
        Assert.Empty(result);
    }

    [Fact]
    public void GetMethodsArgumentsAsString_WhenMethodHasArguments_ShouldReturnArgumentAliasesAsString()
    {
        // Arrange
        var parameterInfoHelper = MockFreeze<IParameterInfoHelper>();
        var stackFrameSerializer = MockCreate<MockStackFrameSerializer>();
        var method = typeof(MockStackFrameSerializer).GetMethod(nameof(MockStackFrameSerializer.MethodWithArguments))!;

        parameterInfoHelper
            .GetAlias(default!)
            .ReturnsForAnyArgs(caller =>
            {
                var parameter = (ParameterInfo)caller[0];
                return $"{parameter.ParameterType.Name} {parameter.Name}";
            });

        // Act
        var result = stackFrameSerializer.GetMethodsArgumentsAsString(method);

        // Assert
        Assert.Equal("Int32 arg1, String arg2", result);
    }

    [Fact]
    public void GetGenericArgumentsAsString_WhenMethodBaseIsNull_ThrowsArgumentNullException()
    {
        // Arrange
        var stackFrameSerializer = MockCreate<MockStackFrameSerializer>();
        MethodBase methodBase = null!;

        // Act
        void Action()
            => stackFrameSerializer.GetGenericArgumentsAsString(methodBase);

        // Assert
        Assert.Throws<ArgumentNullException>(Action);
    }

    [Fact]
    public void GetGenericArgumentsAsString_WhenMethodIsNotGeneric_ShouldReturnEmptyString()
    {
        // Arrange
        var stackFrameSerializer = MockCreate<MockStackFrameSerializer>();
        var methodBase = new MockMethodInfo
        {
            IsGenericMethodOverride = false
        };

        // Act
        var result = stackFrameSerializer.GetGenericArgumentsAsString(methodBase);

        // Assert
        Assert.Empty(result);
    }

    [Fact]
    public void GetGenericArgumentsAsString_WhenMethodIsGeneric_ShouldReturnGenericArgumentsAsString()
    {
        // Arrange
        var typeAliasProvider = MockFreeze<ITypeAliasProvider>();
        var stackFrameSerializer = MockCreate<MockStackFrameSerializer>();
        var methodBase = new MockMethodInfo
        {
            IsGenericMethodOverride = true,
            GetGenericArgumentsOverride = new[] { typeof(int), typeof(string) }
        };

        typeAliasProvider
            .GetAlias(default!)
            .ReturnsForAnyArgs(caller =>
            {
                var type = (Type)caller[0];
                return type.Name;
            });

        // Act
        var result = stackFrameSerializer.GetGenericArgumentsAsString(methodBase);

        // Assert
        Assert.Equal("Int32, String", result);
    }

    [Fact]
    public void GetLineInfoAsString_WhenFrameIsNull_ShouldThrowArgumentNullException()
    {
        // Arrange
        var stackFrameSerializer = MockCreate<MockStackFrameSerializer>();
        StackFrame frame = null!;

        // Act
        void Action()
            => stackFrameSerializer.GetLineInfoAsString(frame);

        // Assert
        Assert.Throws<ArgumentNullException>(Action);
    }

    [Fact]
    public void GetLineInfoAsString_WhenCalled_ShouldLineInfo()
    {
        // Arrange
        var stackFrameSerializer = MockCreate<MockStackFrameSerializer>();
        var frame = new StackFrame();

        // Act
        var result = stackFrameSerializer.GetLineInfoAsString(frame);

        // Assert
        Assert.Matches(@"in (?<FileName>.*):line (?<LineNumber>\d+)", result);
    }

    private sealed class MockStackFrameSerializer(
        ITypeAliasProvider typeAliasProvider,
        IParameterInfoHelper parameterInfoHelper,
        IReflectionHelper reflectionHelper
        ) : StackFrameSerializerBase(typeAliasProvider, parameterInfoHelper, reflectionHelper)
    {
        public static void MethodWithoutArguments()
        {
        }

        [SuppressMessage("Style", "IDE0060:Remove unused parameter", Justification = "<Pending>")]
        public static void MethodWithArguments(int arg1, string arg2)
        {
        }

        public static void NonGenericMethod()
        {
        }

        public static void GenericMethod<TValue>()
        {
        }

        public override bool CanSerialize(StackFrame frame)
            => throw new NotImplementedException();

        public override string ToString(StackFrame frame)
            => throw new NotImplementedException();

        public new string GetAlias(Type type)
            => base.GetAlias(type);

        public new string GetAlias(ParameterInfo parameter)
            => base.GetAlias(parameter);

        public new DeconstructedMethodName DeconstructMethodName(string rawMethodName)
            => base.DeconstructMethodName(rawMethodName);

        public new string GetMethodsArgumentsAsString(MethodBase method)
            => base.GetMethodsArgumentsAsString(method);

        public new string GetGenericArgumentsAsString(MethodBase method)
            => base.GetGenericArgumentsAsString(method);

        public new string GetLineInfoAsString(StackFrame frame)
            => StackFrameSerializerBase.GetLineInfoAsString(frame);
    }
}

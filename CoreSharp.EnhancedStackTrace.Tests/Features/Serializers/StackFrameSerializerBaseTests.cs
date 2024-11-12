using CoreSharp.EnhancedStackTrace.Features.Reflection;
using CoreSharp.EnhancedStackTrace.Features.Serializers;
using NSubstitute;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;

namespace CoreSharp.EnhancedStackTrace.Tests.Features.Serializers;

internal sealed class StackFrameSerializerBaseTests : TestsBase
{
    [Test]
    public void GetAlias_WhenTypeIsNull_ThrowsArgumentNullException()
    {
        // Arrange
        var stackFrameSerializer = MockCreate<MockStackFrameSerializer>();
        Type type = null!;

        // Act
        Action action = () => stackFrameSerializer.GetAlias(type);

        // Assert
        action.Should().ThrowExactly<ArgumentNullException>();
    }

    [Test]
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
        result.Should().Be("alias");
    }

    [Test]
    public void GetAlias_WhenParameterIsNull_ThrowsArgumentNullException()
    {
        // Arrange
        var stackFrameSerializer = MockCreate<MockStackFrameSerializer>();
        ParameterInfo parameter = null!;

        // Act
        Action action = () => stackFrameSerializer.GetAlias(parameter);

        // Assert
        action.Should().ThrowExactly<ArgumentNullException>();
    }

    [Test]
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
        result.Should().Be("alias");
    }

    [Test]
    public void DeconstructMethodName_WhenRawMethodNameIsNull_ShouldThrowArgumentNullException()
    {
        // Arrange
        var stackFrameSerializer = MockCreate<MockStackFrameSerializer>();

        // Act
        Action action = () => stackFrameSerializer.DeconstructMethodName(rawMethodName: null!);

        // Assert
        action.Should().ThrowExactly<ArgumentNullException>();
    }

    [Test]
    [TestCase("")]
    [TestCase(" ")]
    public void DeconstructMethodName_WhenRawMethodNameIsEmpty_ShouldThrowArgumentException(string rawMethodName)
    {
        // Arrange
        var stackFrameSerializer = MockCreate<MockStackFrameSerializer>();

        // Act
        Action action = () => stackFrameSerializer.DeconstructMethodName(rawMethodName);

        // Assert
        action.Should().ThrowExactly<ArgumentException>();
    }

    [Test]
    public void DeconstructMethodName_WhenRawMethodNameIsNotSubMethod_ShouldReturnMethodNameAndNullSubMethodName()
    {
        // Arrange
        var reflectionHelper = MockFreeze<IReflectionHelper>();
        var stackFrameSerializer = MockCreate<MockStackFrameSerializer>();
        var rawMethodName = "MethodName";

        reflectionHelper
            .DeconstructMethodName(default!)
            .ReturnsForAnyArgs(new DeconstructedMethodName("MethodName", "SubMethodName", SubMethodIdentifier.LocalFunction));

        // Act
        var (methodName, subMethodName, subMethodIdentifier) = stackFrameSerializer.DeconstructMethodName(rawMethodName);

        // Assert
        methodName.Should().Be("MethodName");
        subMethodName.Should().Be("SubMethodName");
        subMethodIdentifier.Should().Be(SubMethodIdentifier.LocalFunction);
    }

    [Test]
    public void GetMethodsArgumentsAsString_WhenMethodBaseIsNull_ThrowsArgumentNullException()
    {
        // Arrange
        var stackFrameSerializer = MockCreate<MockStackFrameSerializer>();
        MethodBase methodBase = null!;

        // Act
        Action action = () => stackFrameSerializer.GetMethodsArgumentsAsString(methodBase);

        // Assert
        action.Should().ThrowExactly<ArgumentNullException>();
    }

    [Test]
    public void GetMethodsArgumentsAsString_WhenMethodHasNoArguments_ShouldReturnEmptyString()
    {
        // Arrange
        var stackFrameSerializer = MockCreate<MockStackFrameSerializer>();
        var method = typeof(MockStackFrameSerializer).GetMethod(nameof(MockStackFrameSerializer.MethodWithoutArguments))!;

        // Act
        var result = stackFrameSerializer.GetMethodsArgumentsAsString(method);

        // Assert
        result.Should().BeEmpty();
    }

    [Test]
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
        result.Should().Be("Int32 arg1, String arg2");
    }

    [Test]
    public void GetGenericArgumentsAsString_WhenMethodBaseIsNull_ThrowsArgumentNullException()
    {
        // Arrange
        var stackFrameSerializer = MockCreate<MockStackFrameSerializer>();
        MethodBase methodBase = null!;

        // Act
        Action action = () => stackFrameSerializer.GetGenericArgumentsAsString(methodBase);

        // Assert
        action.Should().ThrowExactly<ArgumentNullException>();
    }

    [Test]
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
        result.Should().BeEmpty();
    }

    [Test]
    public void GetGenericArgumentsAsString_WhenMethodIsGeneric_ShouldReturnGenericArgumentsAsString()
    {
        // Arrange
        var typeAliasProvider = MockFreeze<ITypeAliasProvider>();
        var stackFrameSerializer = MockCreate<MockStackFrameSerializer>();
        var methodBase = new MockMethodInfo
        {
            IsGenericMethodOverride = true,
            GetGenericArgumentsOverride = [typeof(int), typeof(string)]
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
        result.Should().Be("Int32, String");
    }

    [Test]
    public void GetLineInfoAsString_WhenFrameIsNull_ShouldThrowArgumentNullException()
    {
        // Arrange
        var stackFrameSerializer = MockCreate<MockStackFrameSerializer>();
        StackFrame frame = null!;

        // Act
        Action action = () => stackFrameSerializer.GetLineInfoAsString(frame);

        // Assert
        action.Should().ThrowExactly<ArgumentNullException>();
    }

    [Test]
    public void GetLineInfoAsString_WhenCalled_ShouldLineInfo()
    {
        // Arrange
        var stackFrameSerializer = MockCreate<MockStackFrameSerializer>();
        var frame = new StackFrame();

        // Act
        var result = stackFrameSerializer.GetLineInfoAsString(frame);

        // Assert
        result.Should().MatchRegex(@"in (?<FileName>.*):line (?<LineNumber>\d+)");
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

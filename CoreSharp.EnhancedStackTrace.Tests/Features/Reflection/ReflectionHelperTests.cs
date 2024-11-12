﻿using CoreSharp.EnhancedStackTrace.Features.Reflection;
using System.Diagnostics;

namespace CoreSharp.EnhancedStackTrace.Tests.Features.Reflection;

internal sealed class ReflectionHelperTests : TestsBase
{
    [Test]
    public void IsHiddenInStackTrace_WhenMethodBaseIsNull_ShouldThrowArgumentNullException()
    {
        // Arrange
        var helper = new ReflectionHelper();

        // Act
        Action action = () => helper.IsHiddenInStackTrace(methodBase: null!);

        // Assert
        action.Should().ThrowExactly<ArgumentNullException>();
    }

    [Test]
    public void IsHiddenInStackTrace_WhenMethodBaseHasNoStackTraceHiddenAttribute_ShouldReturnFalse()
    {
        // Arrange
        var helper = new ReflectionHelper();
        var methodBase = typeof(IsHiddenInStackTraceMethods)
            .GetMethod(nameof(IsHiddenInStackTraceMethods.Visible))!;

        // Act
        var result = helper.IsHiddenInStackTrace(methodBase);

        // Assert
        result.Should().BeFalse();
    }

    [Test]
    public void IsHiddenInStackTrace_WhenMethodBaseHasStackTraceHiddenAttribute_ShouldReturnTrue()
    {
        // Arrange
        var helper = new ReflectionHelper();
        var methodBase = typeof(IsHiddenInStackTraceMethods)
            .GetMethod(nameof(IsHiddenInStackTraceMethods.Hidden))!;

        // Act
        var result = helper.IsHiddenInStackTrace(methodBase);

        // Assert
        result.Should().BeTrue();
    }

    [Test]
    public void DeconstructorMethodName_WhenRawMethodNameIsNull_ShouldThrowArgumentNullException()
    {
        // Arrange
        var helper = new ReflectionHelper();

        // Act
        Action action = () => helper.DeconstructMethodName(rawMethodName: null!);

        // Assert
        action.Should().ThrowExactly<ArgumentNullException>();
    }

    [Test]
    [TestCase("")]
    [TestCase(" ")]
    public void DeconstructorMethodName_WhenRawMethodNameIsEmpty_ShouldThrowArgumentException(string rawMethodName)
    {
        // Arrange
        var helper = new ReflectionHelper();

        // Act
        Action action = () => helper.DeconstructMethodName(rawMethodName);

        // Assert
        action.Should().ThrowExactly<ArgumentException>();
    }

    [Test]
    [TestCaseSource(typeof(DeconstructMethodNameTestArgs), nameof(DeconstructMethodNameTestArgs.Source))]
    public void DeconstructorMethodName_WhenCalled_ShouldReturnCorrectValues(DeconstructMethodNameTestArgs arguments)
    {
        // Arrange
        var helper = new ReflectionHelper();

        // Act
        var (methodName, subMethodName, subMethodIdentifier) = helper.DeconstructMethodName(arguments.RawMethodName);

        // Assert
        methodName.Should().Be(arguments.ExpectedMethodName);
        subMethodName.Should().Be(arguments.ExpectedSubMethodName);
        subMethodIdentifier.Should().Be(arguments.ExpectedSubMethodIdentifier);
    }

    [Test]
    public void IsTuple_WhenTypeIsNull_ShouldThrowArgumentNullException()
    {
        // Arrange
        var helper = new ReflectionHelper();

        // Act
        Action action = () => helper.IsTuple(type: null!);

        // Assert
        action.Should().ThrowExactly<ArgumentNullException>();
    }

    [Test]
    public void IsTuple_WhenCalledWithArgumentCountAndTypeIsNull_ShouldThrowArgumentNullException()
    {
        // Arrange
        var helper = new ReflectionHelper();
        Type type = null!;

        // Act
        Action action = () => helper.IsTuple(type, argumentCount: 1);

        // Assert
        action.Should().ThrowExactly<ArgumentNullException>();
    }

    [Test]
    public void IsTuple_WhenTypeIsNotGeneric_ShouldReturnFalse()
    {
        // Arrange
        var helper = new ReflectionHelper();
        var type = typeof(int);

        // Act
        var result = helper.IsTuple(type);

        // Assert
        result.Should().BeFalse();
    }

    [Test]
    public void IsTuple_WhenTypeIsNotTuple_ShouldReturnFalse()
    {
        // Arrange
        var helper = new ReflectionHelper();
        var type = typeof(IEnumerable<int>);

        // Act
        var result = helper.IsTuple(type);

        // Assert
        result.Should().BeFalse();
    }

    [Test]
    public void IsTuple_WhenTypeGenericTypeDefinitionFullNameIsNull_ShouldReturnFalse()
    {
        // Arrange
        var helper = new ReflectionHelper();
        var type = new MockType
        {
            IsGenericTypeOverride = true,
            GetGenericTypeDefinitionOverride = new MockType
            {
                FullNameOverride = null
            }
        };

        // Act
        var result = helper.IsTuple(type);

        // Assert
        result.Should().BeFalse();
    }

    [Test]
    public void IsTuple_WhenTypeIsTuple_ShouldReturnTrue()
    {
        // Arrange
        var helper = new ReflectionHelper();
        var type = Tuple.Create(0).GetType();

        // Act
        var result = helper.IsTuple(type);

        // Assert
        result.Should().BeTrue();
    }

    [Test]
    public void IsTuple_WhenCalledWithArgumentCountAndTypeIsNotTuple_ShouldReturnFalse()
    {
        // Arrange
        var helper = new ReflectionHelper();
        var type = typeof(int);

        // Act
        var result = helper.IsTuple(type, argumentCount: 1);

        // Assert
        result.Should().BeFalse();
    }

    [Test]
    public void IsTuple_WhenCalledWithArgumentCountAndCountIsWrong_ShouldReturnFalse()
    {
        // Arrange
        var helper = new ReflectionHelper();
        var type = Tuple.Create(0).GetType();

        // Act
        var result = helper.IsTuple(type, argumentCount: 2);

        // Assert
        result.Should().BeFalse();
    }

    [Test]
    public void IsTuple_WhenTypeIsTupleWithCorrectArgumentCount_ShouldReturnTrue()
    {
        // Arrange
        var helper = new ReflectionHelper();
        var type = Tuple.Create(0).GetType();

        // Act
        var result = helper.IsTuple(type, argumentCount: 1);

        // Assert
        result.Should().BeTrue();
    }

    [Test]
    public void IsValueTuple_WhenTypeIsNull_ShouldThrowArgumentNullException()
    {
        // Arrange
        var helper = new ReflectionHelper();

        // Act
        Action action = () => helper.IsValueTuple(type: null!);

        // Assert
        action.Should().ThrowExactly<ArgumentNullException>();
    }

    [Test]
    public void IsValueTuple_WhenCalledWithArgumentCountAndTypeIsNull_ShouldThrowArgumentNullException()
    {
        // Arrange
        var helper = new ReflectionHelper();

        // Act
        Action action = () => helper.IsValueTuple(type: null!, argumentCount: 1);

        // Assert
        action.Should().ThrowExactly<ArgumentNullException>();
    }

    [Test]
    public void IsValueTuple_WhenTypeIsNotGeneric_ShouldReturnFalse()
    {
        // Arrange
        var helper = new ReflectionHelper();
        var type = typeof(int);

        // Act
        var result = helper.IsValueTuple(type);

        // Assert
        result.Should().BeFalse();
    }

    [Test]
    public void IsValueTuple_WhenTypeIsNotValueTuple_ShouldReturnFalse()
    {
        // Arrange
        var helper = new ReflectionHelper();
        var type = typeof(IEnumerable<int>);

        // Act
        var result = helper.IsValueTuple(type);

        // Assert
        result.Should().BeFalse();
    }

    [Test]
    public void IsValueTuple_WhenTypeGenericTypeDefinitionFullNameIsNull_ShouldReturnFalse()
    {
        // Arrange
        var helper = new ReflectionHelper();
        var type = new MockType
        {
            IsGenericTypeOverride = true,
            GetGenericTypeDefinitionOverride = new MockType
            {
                FullNameOverride = null
            }
        };

        // Act
        var result = helper.IsValueTuple(type);

        // Assert
        result.Should().BeFalse();
    }

    [Test]
    public void IsValueTuple_WhenTypeIsValueTuple_ShouldReturnTrue()
    {
        // Arrange
        var helper = new ReflectionHelper();
        var type = ValueTuple.Create(1).GetType();

        // Act
        var result = helper.IsValueTuple(type);

        // Assert
        result.Should().BeTrue();
    }

    [Test]
    public void IsValueTuple_WhenCalledWithArgumentCountAndTypeIsNotValueTuple_ShouldReturnFalse()
    {
        // Arrange
        var helper = new ReflectionHelper();
        var type = typeof(int);

        // Act
        var result = helper.IsValueTuple(type, argumentCount: 1);

        // Assert
        result.Should().BeFalse();
    }

    [Test]
    public void IsValueTuple_WhenCalledWithArgumentCountAndCountIsWrong_ShouldReturnFalse()
    {
        // Arrange
        var helper = new ReflectionHelper();
        var type = ValueTuple.Create(0).GetType();

        // Act
        var result = helper.IsValueTuple(type, argumentCount: 2);

        // Assert
        result.Should().BeFalse();
    }

    [Test]
    public void IsValueTuple_WhenTypeIsValueTupleWithCorrectArgumentCount_ShouldReturnTrue()
    {
        // Arrange
        var helper = new ReflectionHelper();
        var type = ValueTuple.Create(0).GetType();

        // Act
        var result = helper.IsValueTuple(type, argumentCount: 1);

        // Assert
        result.Should().BeTrue();
    }

    public sealed class DeconstructMethodNameTestArgs
    {
        public string RawMethodName { get; set; } = null!;
        public string ExpectedMethodName { get; set; } = null!;
        public string? ExpectedSubMethodName { get; set; }
        public SubMethodIdentifier? ExpectedSubMethodIdentifier { get; set; }

        public static IEnumerable<DeconstructMethodNameTestArgs> Source
        {
            get
            {
                // Plain method
                yield return new()
                {
                    RawMethodName = "MethodName",
                    ExpectedMethodName = "MethodName",
                    ExpectedSubMethodName = null,
                    ExpectedSubMethodIdentifier = null
                };

                // Lambda
                yield return new()
                {
                    RawMethodName = "<MethodName>b__SubMethod|1_0",
                    ExpectedMethodName = "MethodName",
                    ExpectedSubMethodName = "SubMethod",
                    ExpectedSubMethodIdentifier = SubMethodIdentifier.LambdaMethod
                };

                // Local function
                yield return new()
                {
                    RawMethodName = "<MethodName>g__SubMethod|1_0",
                    ExpectedMethodName = "MethodName",
                    ExpectedSubMethodName = "SubMethod",
                    ExpectedSubMethodIdentifier = SubMethodIdentifier.LocalFunction
                };

                // Auto-property backing field
                yield return new()
                {
                    RawMethodName = "<MethodName>k__SubMethod|1_0",
                    ExpectedMethodName = "MethodName",
                    ExpectedSubMethodName = "SubMethod",
                    ExpectedSubMethodIdentifier = SubMethodIdentifier.AutoPropertyBackingField
                };

                // Program class
                yield return new()
                {
                    RawMethodName = "<Program>$",
                    ExpectedMethodName = "Program",
                    ExpectedSubMethodName = null,
                    ExpectedSubMethodIdentifier = null
                };

                // Method without pipe char
                yield return new()
                {
                    RawMethodName = "<MethodName>b__SubMethod",
                    ExpectedMethodName = "MethodName",
                    ExpectedSubMethodName = "SubMethod",
                    ExpectedSubMethodIdentifier = SubMethodIdentifier.LambdaMethod
                };

                // Unknown identifier
                yield return new()
                {
                    RawMethodName = "<MethodName>0__SubMethod",
                    ExpectedMethodName = "MethodName",
                    ExpectedSubMethodName = "SubMethod",
                    ExpectedSubMethodIdentifier = null
                };
            }
        }
    }

    private static class IsHiddenInStackTraceMethods
    {
        [StackTraceHidden]
        public static void Hidden()
        {
        }

        public static void Visible()
        {
        }
    }
}

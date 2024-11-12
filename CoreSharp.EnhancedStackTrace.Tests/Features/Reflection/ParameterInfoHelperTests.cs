using CoreSharp.EnhancedStackTrace.Features.Reflection;
using CoreSharp.EnhancedStackTrace.Tests.Common;
using NSubstitute;
using System.Runtime.CompilerServices;

namespace CoreSharp.EnhancedStackTrace.Tests.Features.Reflection;

internal sealed class ParameterInfoHelperTests : TestsBase
{
    [Test]
    public void GetAlias_WhenParameterIsNull_ThrowsArgumentNullException()
    {
        // Arrange
        var helper = MockCreate<ParameterInfoHelper>();

        // Act
        Action action = () => helper.GetAlias(parameter: null!);

        // Assert
        action.Should().ThrowExactly<ArgumentNullException>();
    }

    [Test]
    public void GetAlias_WhenParameterIsOut_ShouldIncludeOutInAlias()
    {
        // Arrange
        var helper = MockCreate<ParameterInfoHelper>();
        var parameter = new MockParameterInfo
        {
            IsOutOverride = true
        };

        // Act
        var result = helper.GetAlias(parameter);

        // Assert
        result.Should().StartWith("out");
    }

    [Test]
    public void GetAlias_WhenParameterIsIn_ShouldIncludeInInAlias()
    {
        // Arrange
        var helper = MockCreate<ParameterInfoHelper>();
        var parameter = new MockParameterInfo
        {
            IsInOverride = true
        };

        // Act
        var result = helper.GetAlias(parameter);

        // Assert
        result.Should().StartWith("in");
    }

    [Test]
    public void GetAlias_WhenParameterIsRef_ShouldIncludeRefInAlias()
    {
        // Arrange
        var helper = MockCreate<ParameterInfoHelper>();
        var parameter = new MockParameterInfo
        {
            IsOutOverride = false,
            IsInOverride = false,
            ParameterTypeOverride = typeof(int).MakeByRefType(),
        };

        // Act
        var result = helper.GetAlias(parameter);

        // Assert
        result.Should().StartWith("ref");
    }

    [Test]
    public void GetAlias_WhenParameterIsDynamic_ShouldReturnDynamic()
    {
        // Arrange
        var helper = MockCreate<ParameterInfoHelper>();
        var parameter = new MockParameterInfo
        {
            IsDefinedOverrides = [typeof(DynamicAttribute)]
        };

        // Act
        var result = helper.GetAlias(parameter);

        // Assert
        result.Should().Contain("dynamic");
    }

    [Test]
    public void GetAlias_WhenParameterIsValueTupleWithoutNames_ShouldReturnValueTupleWithoutNamesAlias()
    {
        // Arrange
        var reflectionHelper = MockFreeze<IReflectionHelper>();
        var aliasProvider = MockFreeze<ITypeAliasProvider>();
        var helper = MockCreate<ParameterInfoHelper>();
        var parameter = new MockParameterInfo
        {
            ParameterTypeOverride = typeof((int, string))
        };

        reflectionHelper
            .IsValueTuple(parameter.ParameterType)
            .Returns(true);

        aliasProvider
            .GetAlias(typeof(int))
            .Returns("int");

        aliasProvider
            .GetAlias(typeof(string))
            .Returns("string");

        // Act
        var result = helper.GetAlias(parameter);

        // Assert
        result.Should().Contain("(int, string)");
    }

    [Test]
    public void GetAlias_WhenParameterIsValueTupleWithNames_ShouldReturnValueTupleWithNamesAlias()
    {
        // Arrange
        var reflectionHelper = MockFreeze<IReflectionHelper>();
        var aliasProvider = MockFreeze<ITypeAliasProvider>();
        var helper = MockCreate<ParameterInfoHelper>();
        var parameter = new MockParameterInfo
        {
            ParameterTypeOverride = typeof((int a, string b)),
            GetCustomAttributesOverride = [new TupleElementNamesAttribute(["a", "b"])]
        };

        reflectionHelper
            .IsValueTuple(parameter.ParameterType)
            .Returns(true);

        aliasProvider
            .GetAlias(typeof(int))
            .Returns("int");

        aliasProvider
            .GetAlias(typeof(string))
            .Returns("string");

        // Act
        var result = helper.GetAlias(parameter);

        // Assert
        result.Should().Contain("(int a, string b)");
    }

    [Test]
    public void GetAlias_WhenCalled_ShouldIncludeParameterTypeInAlias()
    {
        // Arrange
        var aliasProvider = MockFreeze<ITypeAliasProvider>();
        var helper = MockCreate<ParameterInfoHelper>();
        var parameter = new MockParameterInfo
        {
            ParameterTypeOverride = typeof(int)
        };

        aliasProvider
            .GetAlias(parameter.ParameterType)
            .Returns("int");

        // Act
        var result = helper.GetAlias(parameter);

        // Assert
        result.Should().Contain("int");
    }

    [Test]
    public void GetAlias_WhenParameterHasName_ShouldIncludeNameInAlias()
    {
        // Arrange
        var helper = MockCreate<ParameterInfoHelper>();
        var parameter = new MockParameterInfo
        {
            NameOverride = "name"
        };

        // Act
        var result = helper.GetAlias(parameter);

        // Assert
        result.Should().Contain("name");
    }

    [Test]
    public void GetAlias_WhenParameterHasDefaultValueIsTrueAndDefaultValueIsNull_ShouldNotIncludeDefaultValue()
    {
        // Arrange
        var helper = MockCreate<ParameterInfoHelper>();
        var parameter = new MockParameterInfo
        {
            NameOverride = "name",
            HasDefaultValueOverride = true,
            DefaultValueOverride = null
        };

        // Act
        var result = helper.GetAlias(parameter);

        // Assert
        result.Should().EndWith("name");
    }

    [Test]
    public void GetAlias_WhenParameterHasDefaultValueIsFalseAndDefaultValueIsNotNull_ShouldNotIncludeDefaultValue()
    {
        // Arrange
        var helper = MockCreate<ParameterInfoHelper>();
        var parameter = new MockParameterInfo
        {
            NameOverride = "name",
            HasDefaultValueOverride = false,
            DefaultValueOverride = "value"
        };

        // Act
        var result = helper.GetAlias(parameter);

        // Assert
        result.Should().EndWith("name");
    }

    [Test]
    public void GetAlias_WhenParameterHasDefaultValue_ShouldIncludeDefaultValue()
    {
        // Arrange
        var helper = MockCreate<ParameterInfoHelper>();
        var parameter = new MockParameterInfo
        {
            NameOverride = "name",
            HasDefaultValueOverride = true,
            DefaultValueOverride = "value"
        };

        // Act
        var result = helper.GetAlias(parameter);

        // Assert
        result.Should().EndWith("value");
    }
}

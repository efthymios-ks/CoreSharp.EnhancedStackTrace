using CoreSharp.EnhancedStackTrace.Features.Reflection;
using System.Runtime.CompilerServices;
using Tests.Common.Mocks;

namespace CoreSharp.EnhancedStackTrace.Tests.Features.Reflection;

public sealed class ParameterInfoHelperTests : TestsBase
{
    [Fact]
    public void GetAlias_WhenParameterIsNull_ThrowsArgumentNullException()
    {
        // Arrange
        var helper = MockCreate<ParameterInfoHelper>();

        // Act
        void Action()
            => helper.GetAlias(parameter: null!);

        // Assert
        Assert.Throws<ArgumentNullException>(Action);
    }

    [Fact]
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
        Assert.StartsWith("out", result);
    }

    [Fact]
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
        Assert.StartsWith("in", result);
    }

    [Fact]
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
        Assert.StartsWith("ref", result);
    }

    [Fact]
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
        Assert.Contains("dynamic", result);
    }

    [Fact]
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
        Assert.Contains("(int, string)", result);
    }

    [Fact]
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
        Assert.Contains("(int a, string b)", result);
    }

    [Fact]
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
        Assert.Contains("int", result);
    }

    [Fact]
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
        Assert.Contains("name", result);
    }

    [Fact]
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
        Assert.EndsWith("name", result);
    }

    [Fact]
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
        Assert.EndsWith("name", result);
    }

    [Fact]
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
        Assert.EndsWith("value", result);
    }
}

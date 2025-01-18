using CoreSharp.EnhancedStackTrace.Extensions;
using System.Text;

namespace CoreSharp.EnhancedStackTrace.Tests.Extensions;

public sealed class StringBuilderExtensionsTests
{
    [Fact]
    public void AppendIfNotEmpty_WhenBuilderIsNull_ThrowsArgumentNullException()
    {
        // Arrange
        StringBuilder builder = null!;

        // Act
        void Action()
            => builder.AppendIfNotEmpty("valueToCheck", "value");

        // Assert
        Assert.Throws<ArgumentNullException>(Action);
    }

    [Fact]
    public void AppendIfNotEmpty_WhenCalled_ShouldReturnSameInstance()
    {
        // Arrange
        var builder = new StringBuilder();

        // Act
        var result = builder.AppendIfNotEmpty("valueToCheck", "value");

        // Assert
        Assert.Same(builder, result);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    public void AppendIfNotEmpty_WhenValueToCheckIsNull_ShouldReturnSameInstance(string valueToCheck)
    {
        // Arrange
        var builder = new StringBuilder();

        // Act
        var result = builder
            .AppendIfNotEmpty(valueToCheck, "value")
            .ToString();

        // Assert
        Assert.DoesNotContain("value", result);
    }

    [Fact]
    public void AppendIf_WhenBuilderIsNull_ThrowsArgumentNullException()
    {
        // Arrange
        StringBuilder builder = null!;

        // Act
        void Action()
            => builder.AppendIf(condition: false, "value");

        // Assert
        Assert.Throws<ArgumentNullException>(Action);
    }

    [Fact]
    public void AppendIf_WhenCalled_ShouldReturnSameInstance()
    {
        // Arrange
        var builder = new StringBuilder();

        // Act
        var result = builder.AppendIf(condition: false, "value");

        // Assert
        Assert.Same(builder, result);
    }

    [Fact]
    public void AppendIf_WhenConditionIsFalse_ShouldNotAppendValue()
    {
        // Arrange
        var builder = new StringBuilder();

        // Act
        var result = builder
            .AppendIf(condition: false, "value")
            .ToString();

        // Assert
        Assert.NotEqual("value", result);
    }

    [Fact]
    public void AppendIf_WhenConditionIsTrue_ShouldAppendValue()
    {
        // Arrange
        var builder = new StringBuilder();

        // Act
        var result = builder
            .AppendIf(condition: true, "value")
            .ToString();

        // Assert
        Assert.Equal("value", result);
    }

    [Fact]
    public void AppendIf_WhenHasTrueFalseValues_ShouldReturnSameInstance()
    {
        // Arrange
        var builder = new StringBuilder();

        // Act
        var result = builder.AppendIf(condition: false, "true", "false");

        // Assert
        Assert.Same(builder, result);
    }

    [Fact]
    public void AppendIf_WhenHasTrueFalseValuesAndBuilderIsNull_ThrowsArgumentNullException()
    {
        // Arrange
        StringBuilder builder = null!;

        // Act
        void Action()
            => builder.AppendIf(condition: false, "true", "false");

        // Assert
        Assert.Throws<ArgumentNullException>(Action);
    }

    [Fact]
    public void AppendIf_WhenHasTrueFalseValuesConditionIsTrue_ShouldAppendTrueValue()
    {
        // Arrange
        var builder = new StringBuilder();

        // Act
        var result = builder
            .AppendIf(condition: true, "true", "false")
            .ToString();

        // Assert
        Assert.Equal("true", result);
    }

    [Fact]
    public void AppendIf_WhenHasTrueFalseValuesAndConditionIsFalse_ShouldAppendFalseValue()
    {
        // Arrange
        var builder = new StringBuilder();

        // Act
        var result = builder
            .AppendIf(condition: false, "true", "false")
            .ToString();

        // Assert
        Assert.Equal("false", result);
    }
}

using CoreSharp.EnhancedStackTrace.Extensions;
using System.Text;

namespace CoreSharp.EnhancedStackTrace.Tests.Extensions;

internal class StringBuilderExtensionsTests
{
    [Test]
    public void AppendIfNotEmpty_WhenBuilderIsNull_ThrowsArgumentNullException()
    {
        // Arrange
        StringBuilder builder = null!;

        // Act
        Action action = () => builder.AppendIfNotEmpty("valueToCheck", "value");

        // Assert
        action.Should().Throw<ArgumentNullException>();
    }

    [Test]
    public void AppendIfNotEmpty_WhenCalled_ShouldReturnSameInstance()
    {
        // Arrange
        var builder = new StringBuilder();

        // Act
        var result = builder.AppendIfNotEmpty("valueToCheck", "value");

        // Assert
        result.Should().BeSameAs(builder);
    }

    [Test]
    [TestCase(null!)]
    [TestCase("")]
    [TestCase(" ")]
    public void AppendIfNotEmpty_WhenValueToCheckIsNull_ShouldReturnSameInstance(string valueToCheck)
    {
        // Arrange
        var builder = new StringBuilder();

        // Act
        var result = builder
            .AppendIfNotEmpty(valueToCheck, "value")
            .ToString();

        // Assert
        result.Should().NotContain("value");
    }

    [Test]
    public void AppendIf_WhenBuilderIsNull_ThrowsArgumentNullException()
    {
        // Arrange
        StringBuilder builder = null!;

        // Act
        Action action = () => builder.AppendIf(condition: false, "value");

        // Assert
        action.Should().Throw<ArgumentNullException>();
    }

    [Test]
    public void AppendIf_WhenCalled_ShouldReturnSameInstance()
    {
        // Arrange
        var builder = new StringBuilder();

        // Act
        var result = builder.AppendIf(condition: false, "value");

        // Assert
        result.Should().BeSameAs(builder);
    }

    [Test]
    public void AppendIf_WhenConditionIsFalse_ShouldNotAppendValue()
    {
        // Arrange
        var builder = new StringBuilder();

        // Act
        var result = builder
            .AppendIf(condition: false, "value")
            .ToString();

        // Assert
        result.Should().NotBe("value");
    }

    [Test]
    public void AppendIf_WhenConditionIsTrue_ShouldAppendValue()
    {
        // Arrange
        var builder = new StringBuilder();

        // Act
        var result = builder
            .AppendIf(condition: true, "value")
            .ToString();

        // Assert
        result.Should().Be("value");
    }

    [Test]
    public void AppendIf_WhenHasTrueFalseValues_ShouldReturnSameInstance()
    {
        // Arrange
        var builder = new StringBuilder();

        // Act
        var result = builder.AppendIf(condition: false, "true", "false");

        // Assert
        result.Should().BeSameAs(builder);
    }

    [Test]
    public void AppendIf_WhenHasTrueFalseValuesAndBuilderIsNull_ThrowsArgumentNullException()
    {
        // Arrange
        StringBuilder builder = null!;

        // Act
        Action action = () => builder.AppendIf(condition: false, "true", "false");

        // Assert
        action.Should().Throw<ArgumentNullException>();
    }

    [Test]
    public void AppendIf_WhenHasTrueFalseValuesConditionIsTrue_ShouldAppendTrueValue()
    {
        // Arrange
        var builder = new StringBuilder();

        // Act
        var result = builder
            .AppendIf(condition: true, "true", "false")
            .ToString();

        // Assert
        result.Should().Be("true");
    }

    [Test]
    public void AppendIf_WhenHasTrueFalseValuesAndConditionIsFalse_ShouldAppendFalseFalue()
    {
        // Arrange
        var builder = new StringBuilder();

        // Act
        var result = builder
            .AppendIf(condition: false, "true", "false")
            .ToString();

        // Assert
        result.Should().Be("false");
    }
}

using CoreSharp.EnhancedStackTrace.Features.Reflection;
using NSubstitute;
using System.Collections;

namespace CoreSharp.EnhancedStackTrace.Tests.Features.Reflection;

internal partial class TypeAliasProviderTests : TestsBase
{
    [Test]
    public void GetAlias_WhenTypeIsNull_ShouldThrowArgumentNullException()
    {
        // Arrange
        var provider = MockCreate<TypeAliasProvider>();

        // Act
        Action action = () => provider.GetAlias(type: null!);

        // Assert
        action.Should().ThrowExactly<ArgumentNullException>();
    }

    [Test]
    [TestCase(typeof(int), false)]
    [TestCase(typeof(int?), true)]
    public void GetAlias_WhenTypeIsNullable_ShouldContainQuestionMark(Type inputType, bool shouldContainQuestionMark)
    {
        // Arrange
        var provider = MockCreate<TypeAliasProvider>();
        var type = inputType;

        // Act
        var result = provider.GetAlias(type);

        // Assert
        result.EndsWith('?').Should().Be(shouldContainQuestionMark);
    }

    [Test]
    [TestCase(typeof(string[]), "string[]")]
    [TestCase(typeof(string[,]), "string[,]")]
    public void GetAlias_WhenTypeIsArray_ShouldReturnArrayAlias(Type inputType, string expectedAlias)
    {
        // Arrange
        var provider = MockCreate<TypeAliasProvider>();

        // Act
        var result = provider.GetAlias(inputType);

        // Assert
        result.Should().Contain(expectedAlias);
    }

    [Test]
    [TestCase(typeof(IList<int>), "IList<int>")]
    [TestCase(typeof(List<int>), "List<int>")]
    public void GetAlias_WhenTypeIsGeneric_ShouldReturnGenericAlias(Type inputType, string expectedAlias)
    {
        // Arrange
        var provider = MockCreate<TypeAliasProvider>();

        // Act
        var result = provider.GetAlias(inputType);

        // Assert
        result.Should().Contain(expectedAlias);
    }

    [Test]
    public void GetAlias_WhenTypeIsByRef_ShouldReturnBaseElementTypeAlias()
    {
        // Arrange 
        var provider = MockCreate<TypeAliasProvider>();
        var type = typeof(int).MakeByRefType();

        // Act
        var result = provider.GetAlias(type);

        // Assert
        result.Should().Contain("int");
        result.Should().NotContain("&");
    }

    [Test]
    [TestCase(typeof(int), "int")]
    [TestCase(typeof(double), "double")]
    public void GetAlias_WhenTypeHasConstantOverride_ShouldOverridentAlias(Type inputType, string expectedAlias)
    {
        // Arrange
        var provider = MockCreate<TypeAliasProvider>();

        // Act
        var result = provider.GetAlias(inputType);

        // Assert
        result.Should().Be(expectedAlias);
    }

    [Test]
    [TestCase(typeof(IList), "IList")]
    [TestCase(typeof(IList<>), "IList<T>")]
    [TestCase(typeof(List<>), "List<T>")]
    public void GetAlias_WhenTypeIsUnderSystemNamespace_ShouldReturnTypeName(Type inputType, string expectedAlias)
    {
        // Arrange
        var reflectionHelper = MockFreeze<IReflectionHelper>();
        var provider = MockCreate<TypeAliasProvider>();

        reflectionHelper
            .DeconstructMethodName(default!)
            .ReturnsForAnyArgs(caller =>
            {
                var rawMethodName = caller.ArgAt<string>(0);
                return new DeconstructedMethodName(
                    MethodName: rawMethodName,
                    SubMethodName: null,
                    SubMethodIdentifier: null
                );
            });

        // Act
        var result = provider.GetAlias(inputType);

        // Assert
        result.Should().Be(expectedAlias);
    }

    [Test]
    public void GetAlias_WhenTypeFullNameIsNull_ShouldReturnName()
    {
        // Arrange
        var reflectionHelper = MockFreeze<IReflectionHelper>();
        var provider = MockCreate<TypeAliasProvider>();

        reflectionHelper
            .DeconstructMethodName(default!)
            .ReturnsForAnyArgs(caller =>
            {
                var rawMethodName = caller.ArgAt<string>(0);
                return new DeconstructedMethodName(
                    MethodName: rawMethodName,
                    SubMethodName: null,
                    SubMethodIdentifier: null
                );
            });

        var type = new MockType
        {
            FullNameOverride = null,
            NameOverride = "Name"
        };

        // Act
        var result = provider.GetAlias(type);

        // Assert
        result.Should().Be("Name");
    }

    [Test]
    public void GetAlias_WhenTypeFullNameIsNotNull_ShouldReturnFullName()
    {
        // Arrange
        var reflectionHelper = MockFreeze<IReflectionHelper>();
        var provider = MockCreate<TypeAliasProvider>();

        reflectionHelper
            .DeconstructMethodName(default!)
            .ReturnsForAnyArgs(caller =>
            {
                var rawMethodName = caller.ArgAt<string>(0);
                return new DeconstructedMethodName(
                    MethodName: rawMethodName,
                    SubMethodName: null,
                    SubMethodIdentifier: null
                );
            });

        var type = new MockType
        {
            FullNameOverride = "FullName",
            NameOverride = "Name"
        };

        // Act
        var result = provider.GetAlias(type);

        // Assert
        result.Should().Be("FullName");
    }

    [Test]
    public void GetAlias_WhenTypeIsSubMethod_ShouldReturnTypeNameAndSubMethod()
    {
        // Arrange
        var reflectionHelper = MockFreeze<IReflectionHelper>();
        var provider = MockCreate<TypeAliasProvider>();

        reflectionHelper
            .DeconstructMethodName(default!)
            .ReturnsForAnyArgs(caller => new(
                MethodName: "Method",
                SubMethodName: "SubMethod",
                SubMethodIdentifier: null
            ));

        var type = new MockType
        {
            FullNameOverride = "FullName"
        };

        // Act
        var result = provider.GetAlias(type);

        // Assert
        result.Should().Be("Method.SubMethod");
    }
}

using CoreSharp.EnhancedStackTrace.Features.Reflection;
using System.Collections;
using Tests.Common.Mocks;

namespace CoreSharp.EnhancedStackTrace.Tests.Features.Reflection;

public sealed partial class TypeAliasProviderTests : TestsBase
{
    [Fact]
    public void GetAlias_WhenTypeIsNull_ShouldThrowArgumentNullException()
    {
        // Arrange
        var provider = MockCreate<TypeAliasProvider>();

        // Act
        void Action()
            => provider.GetAlias(type: null!);

        // Assert
        Assert.Throws<ArgumentNullException>(Action);
    }

    [Theory]
    [InlineData(typeof(int), false)]
    [InlineData(typeof(int?), true)]
    public void GetAlias_WhenTypeIsNullable_ShouldContainQuestionMark(Type inputType, bool shouldContainQuestionMark)
    {
        // Arrange
        var provider = MockCreate<TypeAliasProvider>();
        var type = inputType;

        // Act
        var result = provider.GetAlias(type);

        // Assert
        Assert.Equal(shouldContainQuestionMark, result.EndsWith('?'));
    }

    [Theory]
    [InlineData(typeof(string[]), "string[]")]
    [InlineData(typeof(string[,]), "string[,]")]
    public void GetAlias_WhenTypeIsArray_ShouldReturnArrayAlias(Type inputType, string expectedAlias)
    {
        // Arrange
        var provider = MockCreate<TypeAliasProvider>();

        // Act
        var result = provider.GetAlias(inputType);

        // Assert
        Assert.Contains(expectedAlias, result);
    }

    [Theory]
    [InlineData(typeof(IList<int>), "IList<int>")]
    [InlineData(typeof(List<int>), "List<int>")]
    public void GetAlias_WhenTypeIsGeneric_ShouldReturnGenericAlias(Type inputType, string expectedAlias)
    {
        // Arrange
        var provider = MockCreate<TypeAliasProvider>();

        // Act
        var result = provider.GetAlias(inputType);

        // Assert
        Assert.Contains(expectedAlias, result);
    }

    [Fact]
    public void GetAlias_WhenTypeIsByRef_ShouldReturnBaseElementTypeAlias()
    {
        // Arrange 
        var provider = MockCreate<TypeAliasProvider>();
        var type = typeof(int).MakeByRefType();

        // Act
        var result = provider.GetAlias(type);

        // Assert
        Assert.Contains("int", result);
        Assert.DoesNotContain("&", result);
    }

    [Theory]
    [InlineData(typeof(int), "int")]
    [InlineData(typeof(double), "double")]
    public void GetAlias_WhenTypeHasConstantOverride_ShouldOverridentAlias(Type inputType, string expectedAlias)
    {
        // Arrange
        var provider = MockCreate<TypeAliasProvider>();

        // Act
        var result = provider.GetAlias(inputType);

        // Assert
        Assert.Equal(expectedAlias, result);
    }

    [Theory]
    [InlineData(typeof(IList), "IList")]
    [InlineData(typeof(IList<>), "IList<T>")]
    [InlineData(typeof(List<>), "List<T>")]
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
        Assert.Equal(expectedAlias, result);
    }

    [Fact]
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
        Assert.Equal("Name", result);
    }

    [Fact]
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
        Assert.Equal("FullName", result);
    }

    [Fact]
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
        Assert.Equal("Method.SubMethod", result);
    }
}

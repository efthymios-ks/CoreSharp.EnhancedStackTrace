using CoreSharp.EnhancedStackTrace.Features.Serializers;
using NSubstitute;
using System.Diagnostics;

namespace CoreSharp.EnhancedStackTrace.Tests.Features.Serializers;

internal sealed class StackFrameSerializerFactoryTests : TestsBase
{
    [Test]
    public void Create_WhenFrameIsNull_ThrowsArgumentNullException()
    {
        // Arrange
        var factory = new StackFrameSerializerFactory(stackFrameSerializers: []);

        // Act
        Action action = () => factory.Create(frame: null!);

        // Assert
        action.Should().ThrowExactly<ArgumentNullException>();
    }

    [Test]
    public void Create_WhenSerializerFound_ShouldReturnSerializer()
    {
        // Arrange
        var serializer = MockFreeze<IStackFrameSerializer>();
        var factory = new StackFrameSerializerFactory([serializer]);

        var frame = new StackFrame();
        serializer
            .CanSerialize(frame)
            .Returns(true);

        // Act
        var result = factory.Create(frame);

        // Assert
        result.Should().Be(serializer);
    }

    [Test]
    public void Create_WhenSerializerNotFound_ThrowsNotSupportedException()
    {
        // Arrange
        var serializer = MockFreeze<IStackFrameSerializer>();
        var factory = new StackFrameSerializerFactory([serializer]);

        var frame = new StackFrame();
        serializer
            .CanSerialize(frame)
            .Returns(false);

        // Act
        Action action = () => factory.Create(new StackFrame());

        // Assert
        action.Should().ThrowExactly<NotSupportedException>();
    }
}

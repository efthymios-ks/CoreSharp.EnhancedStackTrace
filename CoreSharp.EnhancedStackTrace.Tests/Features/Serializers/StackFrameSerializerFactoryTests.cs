using CoreSharp.EnhancedStackTrace.Features.Serializers;
using System.Diagnostics;

namespace CoreSharp.EnhancedStackTrace.Tests.Features.Serializers;

public sealed class StackFrameSerializerFactoryTests : TestsBase
{
    [Fact]
    public void Create_WhenFrameIsNull_ThrowsArgumentNullException()
    {
        // Arrange
        var factory = new StackFrameSerializerFactory(stackFrameSerializers: []);

        // Act
        void Action()
            => factory.Create(frame: null!);

        // Assert
        Assert.Throws<ArgumentNullException>(Action);
    }

    [Fact]
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
        Assert.Equal(serializer, result);
    }

    [Fact]
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
        void Action()
            => factory.Create(new StackFrame());

        // Assert
        Assert.Throws<NotSupportedException>(Action);
    }
}

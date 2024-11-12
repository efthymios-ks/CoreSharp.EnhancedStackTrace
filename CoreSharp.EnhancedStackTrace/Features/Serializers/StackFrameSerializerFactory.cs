using System.Diagnostics;

namespace CoreSharp.EnhancedStackTrace.Features.Serializers;

internal sealed class StackFrameSerializerFactory(IEnumerable<IStackFrameSerializer> stackFrameSerializers)
    : IStackFrameSerializerFactory
{
    private readonly IEnumerable<IStackFrameSerializer> _stackFrameSerializers = stackFrameSerializers;

    public IStackFrameSerializer Create(StackFrame frame)
    {
        ArgumentNullException.ThrowIfNull(frame);

        foreach (var serializer in _stackFrameSerializers)
        {
            if (serializer.CanSerialize(frame))
            {
                return serializer;
            }
        }

        throw new NotSupportedException($"StackFrameSerializer not found");
    }
}

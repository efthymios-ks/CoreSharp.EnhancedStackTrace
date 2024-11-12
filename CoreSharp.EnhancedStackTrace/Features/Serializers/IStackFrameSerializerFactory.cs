using System.Diagnostics;

namespace CoreSharp.EnhancedStackTrace.Features.Serializers;

internal interface IStackFrameSerializerFactory
{
    IStackFrameSerializer Create(StackFrame frame);
}

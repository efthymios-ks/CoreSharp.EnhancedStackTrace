using System.Diagnostics;

namespace CoreSharp.EnhancedStackTrace.Features.Serializers;

internal interface IStackFrameSerializer
{
    abstract bool CanSerialize(StackFrame frame);
    string ToString(StackFrame frame);
}

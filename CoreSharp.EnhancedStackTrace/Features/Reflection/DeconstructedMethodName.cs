namespace CoreSharp.EnhancedStackTrace.Features.Reflection;

internal record struct DeconstructedMethodName(string MethodName, string? SubMethodName, SubMethodIdentifier? SubMethodIdentifier);

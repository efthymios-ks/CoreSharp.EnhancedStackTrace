using CoreSharp.EnhancedStackTrace.App.ExceptionGenerators;
using CoreSharp.EnhancedStackTrace.Extensions;

try
{
    // TODO: LINQ, Enumerators
    ExceptionThrow.IndexerSet();
}
catch (Exception exception)
{
    Console.WriteLine(exception);
    Console.WriteLine();

    Console.WriteLine(exception.Enhance());
    Console.WriteLine();
}

Console.ReadLine();

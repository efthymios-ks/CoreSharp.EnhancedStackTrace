# CoreSharp.EnhancedStackTrace 

[![Nuget](https://img.shields.io/nuget/v/CoreSharp.EnhancedStackTrace)](https://www.nuget.org/packages/CoreSharp.EnhancedStackTrace/)
[![Coverage](https://sonarcloud.io/api/project_badges/measure?project=efthymios-ks_CoreSharp.EnhancedStackTrace&metric=coverage)](https://sonarcloud.io/summary/new_code?id=efthymios-ks_CoreSharp.EnhancedStackTrace)
[![Quality Gate Status](https://sonarcloud.io/api/project_badges/measure?project=efthymios-ks_CoreSharp.EnhancedStackTrace&metric=alert_status)](https://sonarcloud.io/summary/new_code?id=efthymios-ks_CoreSharp.EnhancedStackTrace)
![GitHub License](https://img.shields.io/github/license/efthymios-ks/CoreSharp.EnhancedStackTrace)

> Enhanced stack trace and more.

## Features 
### Handles
- Types
    - Simplified `System` type aliases
	- `Nullable` types
	- Simplified `ValueTuples` with argument names
- Methods
    - Local methods
	- `ref` return type
- Constructors
	- Instance constructors
	- Static constructors
- Lambdas
	- Actions
	- Funcs
- Properties
    - Getter, setter
    - Auto-property backing fields
- Generic arguments
- Parameters
    - Names
    - `ref`, `out`, `in` modifiers
    - `params` collections
    - Default values
- `Tasks`
- Indexers

## Installation
Install the package with [Nuget](https://www.nuget.org/packages/CoreSharp.EnhancedStackTrace/).  
```
dotnet add package CoreSharp.EnhancedStackTrace
```

## Documentation 
### File & line info
For file name and line number to appear, the project must be compiled with the `pdb` files.  
This can be done by setting `DebugType` in your `.csproj`.
- `portable`: Emit debugging information to .pdb file using cross-platform Portable PDB format.
- `embedded`: Emit debugging information into the .dll/.exe itself (.pdb file is not produced) using Portable PDB format.
```XML
<Project> 
	<PropertyGroup>
		<DebugType>portable</DebugType>
	</PropertyGroup> 
</Project>
```

## Use cases 
### Extensions
```CSharp
using CoreSharp.EnhancedStackTrace.Extensions; 

try
{
	throw new Exception();
}
catch (Exception exception)
{
	var enhancedStackTrace = exception.GetEnhancedStackTrace(); 
}
```

### Dependency Injection
```CSharp
using CoreSharp.EnhancedStackTrace.Features.Factory;

public sealed class Processor(IEnhancedStackTraceFactory enhancedStackTraceFactory)
{
	private readonly IEnhancedStackTraceFactory _enhancedStackTraceFactory = enhancedStackTraceFactory
	
	public void Process()
	{
		try
		{
			throw new Exception();
		}
		catch (Exception exception)
		{
			var enhancedStackTrace = _enhancedStackTraceFactory.Create(exception); 
		}
	}
}
```
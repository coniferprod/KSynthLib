# KSynthLib

Patch management utilities for Kawai K series digital synthesizers: K4, K5, K5000 and K1 II.

This project creates a .NET library as a NuGet package
that can be installed in .NET application projects.

The library targets `netstandard2.0` so that it can be consumed
from a UWP application (Windows 10, 16299 or later as of April 2020).

At the time of this writing it has not been published on NuGet, but
instead is intended to be used locally. If you want to do that, add a local directory
containing your NuGet packages to your NuGet configuration file.

Your NuGet configuration file is most likely `~/.nuget/NuGet/NuGet.Config`,
and your local directory could be something like `/Users/yourname/Library/NuGet`
(you may need to create this directory).

Add the following to the configuration file.

    <packageSources>
        <add key="nuget.org" value="https://api.nuget.org/v3/index.json" protocolVersion="3" />
        <add key="Local" value="/Users/yourname/Library/NuGet" />
    </packageSources>

Build the library:

    dotnet build

Then package it up:

    dotnet pack

Then add it to your local NuGet repository you configured earlier:

    nuget add KSynthLib/bin/Debug/KSynthLib.x.y.z.nupkg -source /Users/yourname/Library/NuGet

where "x.y.z" is the version of the library you want to use.

To automate these steps there is also an installation script for the Bash shell, `install.sh`.
To use it, first set the environment variables:

    export KSYNTHLIB_VERSION=0.5.11
    export KSYNTHLIB_RELEASE=Debug
    export LOCAL_NUGET_PATH=/Users/yourname/Library/NuGet

Then run the script:

    bash install.sh

Finally, add the package into your project like any NuGet package:

    dotnet add package KSynthLib

## Testing

There is an associated test project with unit tests created using xUnit.
Run the unit tests with:

    dotnet test

## Remarks

Most of the synthesizer parameters are represented by domain classes. Many of these
classes use a custom type for a range of values. This ensures that invalid values are
rejected as early as possible in development. Typically each synthesizer has somewhat
different range types, so they are defined separately for each synthesizer in a
device-specific `RangeTypes.cs` file. All the types use the smallest possible .NET
data type that will fit the value. For library consumers it is recommended to create
a view model that translates between UI layers of your application and the model objects
in this library.

The range itself is defined based on the [Range.NET](https://github.com/mnelsonwhite/Range.NET) library, but it requires .NET Core,
while this library uses .NET Standard 2.0 because it needs to be consumed by a UWP
application. Therefore the MIT-licensed code for Range.NET has been incorporated into
the library. Thanks to the author of Range.NET for making this code available under a permissive
license.

Another point related to the Range.NET library is that it uses `System.HashCode` which
is not available in .NET Standard 2.0. Luckily, Microsoft has made it available in the
[Microsoft.BCL.HashCode](https://www.nuget.org/packages/Microsoft.Bcl.HashCode) library, which is a dependency of this project.

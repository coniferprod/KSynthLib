# KSynthLib

Patch management utilities for Kawai K series digital synthesizers: K4, K5, K5000 and K1 II.

This project creates a .NET library as a NuGet package
that can be installed in .NET application projects.

The library is written in C# 8.0 and targets .NET Core 3.1.

It is available on [NuGet](https://www.nuget.org/packages/KSynthLib/) 
but you can also use it locally. 
If you want to do that, add a local directory
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

    export KSYNTHLIB_VERSION=0.7.0
    export KSYNTHLIB_CONFIGURATION=Debug
    export LOCAL_NUGET_PATH=/Users/yourname/Library/NuGet

Then run the script:

    bash install.sh

Finally, add the package into your project like any NuGet package:

    dotnet add package KSynthLib

## Testing

There is an associated test project with unit tests created using 
[xUnit](https://xunit.net/). Run the unit tests with:

    dotnet test

For testing the K4 features you should first download the [original factory
patches](https://kawaius.com/technical-support-division/software-os/) for the K4 as MIDI System Exclusive dump files from Kawai USA. Then
unzip them into a folder called "Kawai K4 Sounds" in your Documents
directory.

## Remarks

Most of the synthesizer parameters are represented by domain classes. Many of these
classes use a custom type for a range of values. This ensures that invalid values are
rejected as early as possible in development. Typically each synthesizer has somewhat
different range types, so they are defined separately for each synthesizer in a
device-specific `RangeTypes.cs` file. All the types use the smallest possible .NET
data type that will fit the value. For library consumers it is recommended to create
a view model that translates between UI layers of your application and the model objects
in this library.

The range implementation uses the [Range.NET](https://github.com/mnelsonwhite/Range.NET) library.

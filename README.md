# KSynthLib

Patch management utilities for Kawai K series digital synthesizers.

Requires .NET Core SDK version 3.1 or later.

This project creates a .NET library as a NuGet package
that can be installed in .NET Core application projects.

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

Finally, install the package into your project like any NuGet package:

    dotnet add package KSynthLib

## Testing

There is an associated test project with unit tests created using xUnit.
Run the unit tests with:

    dotnet test

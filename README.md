# KSynthLib

Patch management utilities for Kawai K series digital synthesizers.

This project creates a .NET Standard library as a NuGet package
that can be installed in .NET application projects.

At the time of this writing it has not been published on NuGet, but 
instead is used locally. If you want to do that, add a local folder
containing your NuGet packages to your NuGet.Config file, like this:

    <packageSources>
        <add key="nuget.org" value="https://api.nuget.org/v3/index.json" protocolVersion="3" />
        <add key="Local" value="/Your/NuGet/Folder" />
    </packageSources>

Build the library, then package it up with `dotnet pack`. Then add it to your local
NuGet repository:

    nuget add bin/Debug/KSynthLib.0.0.1.nupkg -source /Your/NuGet/Folder

Finally, iinstall the package into your project like any NuGet package:

    dotnet add package KSynthLib

Note that currently there is a conflict between the .NET Single class
and the KSynthLib.Single class, so the latter needs to be fully qualified.

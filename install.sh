echo $KSYNTHLIB_VERSION
echo $KSYNTHLIB_CONFIGURATION
echo $LOCAL_NUGET_PATH
dotnet build && dotnet pack && nuget add KSynthLib/bin/$KSYNTHLIB_CONFIGURATION/KSynthLib.$KSYNTHLIB_VERSION.nupkg -source $LOCAL_NUGET_PATH

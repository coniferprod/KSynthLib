echo $KSYNTHLIB_VERSION
echo $KSYNTHLIB_RELEASE
echo $LOCAL_NUGET_PATH
dotnet build && dotnet pack && nuget add KSynthLib/bin/$KSYNTHLIB_RELEASE/KSynthLib.$KSYNTHLIB_VERSION.nupkg -source $LOCAL_NUGET_PATH

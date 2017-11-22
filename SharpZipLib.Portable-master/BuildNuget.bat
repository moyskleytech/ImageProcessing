@ECHO OFF
del *.nupkg
.\.nuget\nuget.exe pack .\SharpZipLib.Portable.nuspec -symbols
PAUSE
 
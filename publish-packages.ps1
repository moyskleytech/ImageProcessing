#!/usr/bin/pwsh
dotnet build --configuration Release
dotnet pack --configuration Release
foreach($d in (gci . -Directory))
{
  Set-Location $d.FullName
  $nugetPath="bin/MCD/Release/$($d.Name).1.0.0.nupkg"
  if(Test-Path $nugetPath)
  {
    Write-Output $nugetPath
    dotnet nuget push "$nugetPath" --source "github"
  }
  $nugetPath="bin/Release/$($d.Name).1.0.0.nupkg"
  if(Test-Path $nugetPath)
  {
    Write-Output $nugetPath
    dotnet nuget push "$nugetPath" --source "github"
  }
}

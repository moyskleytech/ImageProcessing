#!/usr/bin/pwsh
param(
    [Parameter(Mandatory)]
    [string]$version
)
foreach($d in (gci . -Directory))
{
    $csproj = gci "$($d.FullName)/*.csproj"
    Write-Output $csproj.FullName
    $content = Get-Content $csproj.FullName
    $v = $content -replace "(<version>)[0-9.]+(</version>)", "<Version>$version</Version>"
    Set-Content $csproj.FullName $v
}
dotnet build --configuration Release
dotnet pack --configuration Release
foreach($d in (gci . -Directory))
{
    Set-Location $d.FullName
    if(-not (Test-Path ".no-publish"))
    {
        $nugetPath="bin/MCD/Release/$($d.Name).$version.nupkg"
        if(Test-Path $nugetPath)
        {
          Write-Output $nugetPath
          dotnet nuget push "$nugetPath" --source "github"
          dotnet nuget push "$nugetPath" -k "$($env:NUGET_API)" --source "nuget.org"
        }
        $nugetPath="bin/Release/$($d.Name).$version.nupkg"
        if(Test-Path $nugetPath)
        {
          Write-Output $nugetPath
          dotnet nuget push "$nugetPath" --source "github" 
          dotnet nuget push "$nugetPath" -k "$($env:NUGET_API)" --source "nuget.org"
        }
    }
}

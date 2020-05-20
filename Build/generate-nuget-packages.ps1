param(
  [string] $nugetPath = ".\tools\nuget.exe",
  [string] $nuspecPath = ".\nuspec",
  [string] $outputPath = "C:\\sc_demo",
  [string] $version = "930.9.999",
  [switch] $pushLocalNuget
)
Push-Location $PSScriptRoot
# First empty output directory
Remove-Item "$outputPath\*" -Recurse -Force | Out-Null
New-Item -ItemType Directory -Path $outputPath -Force | Out-Null

# gitVersion should be installed by running `choco install GitVersion.Portable`
if ($pushLocalNuget) {
  try {
    $calculatedVersion = gitversion | ConvertFrom-Json
    if ($null -ne $calculatedVersion) {
      $version = $calculatedVersion.SemVer
    }
  }
  catch {
    Write-Host "Something went wrong, are you sure you have Gitversion installed?" -ForegroundColor Red
    Write-Host "'choco install GitVersion.Portable'"
    Exit 1
  }
}
Get-ChildItem -Path (Resolve-Path $nuspecPath) | ForEach-Object {
  & $nugetPath pack ($nuspecPath + "\" + $_.Name) -OutputDirectory $outputPath -Version "$version"
}
if ($pushLocalNuget) {
  Get-ChildItem -Path (Resolve-Path $outputPath) | ForEach-Object {
    & $nugetPath push -Source http://localhost:5555/v3/index.json -ApiKey 121212 $_.FullName
  }

}
Pop-Location
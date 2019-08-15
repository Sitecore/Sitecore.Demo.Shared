param(
	[string] $nugetPath = ".\tools\nuget.exe",
	[string] $nuspecPath = ".\nuspec",
	[string] $outputPath = "C:\\sc_demo",
	[string] $version = "9.2.0.999"
)

Get-ChildItem -Path (Resolve-Path $nuspecPath)| ForEach-Object {
	& $nugetPath pack ($nuspecPath + "\" + $_.Name) -OutputDirectory $outputPath -Version "$version"
}

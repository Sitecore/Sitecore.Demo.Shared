param(
  [string] $nugetPath = ".\tools\nuget.exe",
  [string] $nuspecPath = ".\nuspec",
  [string] $outputPath = "C:\\sc_demo"
)

Get-ChildItem -Path (Resolve-Path $nuspecPath)| ForEach-Object { 	
	& $nugetPath pack ($nuspecPath + "\" + $_.Name) -OutputDirectory $outputPath 
} 



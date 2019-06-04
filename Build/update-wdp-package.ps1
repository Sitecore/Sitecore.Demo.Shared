param(
    [string]$SourcePackagePath,
    [string]$TargetPackagePath,
    [string]$CargoPayloadPath,
    [string]$SATInstallationPath = "C:\Projects\Sitecore.Installation.Utilities\Shared\sat\"
)

Write-Output("Applyting transforms to WDP package... ")
Write-Output("Sitecore Cargo Payload Package: " + $CargoPayloadPath)

#copy source package into a target package location
if(Test-Path($TargetPackagePath)) {
    Remove-Item $TargetPackagePath
}
Copy-Item -Path $SourcePackagePath -Destination $TargetPackagePath

#update target WDP with payload
Import-Module ($SATInstallationPath + "\tools\Sitecore.Cloud.Cmdlets.dll") -Verbose
Update-SCWebDeployPackage -Path $TargetPackagePath -EmbedCargoPayloadPath $CargoPayloadPath 
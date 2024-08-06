param (
    [string]$manifestPath,
    [string]$newVersion,
    [string]$identityName = "MortenBrudvik",
    [string]$identityPublisher = "CN=MortenBrudvik"
)

Write-Output "Updating version in $manifestPath to $newVersion"

if (-Not (Test-Path $manifestPath)) {
    Write-Error "The manifest file path '$manifestPath' does not exist."
    exit 1
}

[xml]$manifest = Get-Content $manifestPath

$manifest.Package.Identity.Version = $newVersion
$manifest.Package.Identity.Name = $identityName
$manifest.Package.Identity.Publisher = $identityPublisher

$manifest.Save($manifestPath)

Write-Output "Version updated to $newVersion in $manifestPath"
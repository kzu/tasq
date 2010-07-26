$keyName = "Tasq"
$keySnk = "Tasq.snk"
$solution = "Tasq.sln"
# Get the 4.0 SDK tools install path from the registry
$sdkDir = (Get-ChildItem 'HKLM:\SOFTWARE\Microsoft\Microsoft SDKs\Windows' -Recurse | Where-Object { $_.Name -match "WinSDK-NetFx40Tools-x86" }).GetValue("InstallationFolder")
write-host Found .NET 4.0 SDK at $sdkDir

$sn = $sdkDir + "sn.exe"

if (![System.IO.File]::Exists((Get-Location).Path + "\" + $keySnk))
{ 
	write-host Generating a new strong name key
	&$sn -q -k $keySnk
}

write-host Extracting strong name public key
&$sn -q -p $keySnk ($keyName + ".pub")

$lines = &$sn -q -tp ($keyName + ".pub")
$key = [System.String]::Join("", $lines, 1, 5)

write-host Generating include file for T4 templates with appropriate public key
[System.IO.File]::WriteAllText(((Get-Location).Path + "\PublicKey.ttinclude"), "<#+ public const string PublicKey = ""$key""; #>");

# Get MSBuild 4.0 path from the registry
$msbuild = (Get-Item -Path HKLM:\SOFTWARE\Microsoft\MSBuild\ToolsVersions\4.0).GetValue("MSBuildToolsPath") + "msbuild.exe"
write-host Found MSBuild 4.0 at $msbuild

&$msbuild $solution -p:Configuration=Release
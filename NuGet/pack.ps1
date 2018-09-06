$root = (split-path -parent $MyInvocation.MyCommand.Definition) + '\..'
#$version = [System.Reflection.Assembly]::LoadFile("$root\Mongo.Migration\bin\Release\net45\Mongo.Migration.dll").GetName().Version
#$versionStr = "{0}.{1}.{2}" -f ($version.Major, $version.Minor, $version.Build)

$versionStr = "1.1.0"

Write-Host "Setting .nuspec version tag to $versionStr"

$content = (Get-Content $root\NuGet\Mongo.Migration.nuspec) 
$content = $content -replace '\$version\$',$versionStr

$content | Out-File $root\nuget\Mongo.Migration.compiled.nuspec

& $root\NuGet\NuGet.exe pack $root\nuget\Mongo.Migration.compiled.nuspec

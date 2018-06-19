param([string]$SolutionDir)

$modName = Get-Content -Path $($SolutionDir + 'MODNAME')
$serverPath = Get-Content -Path $($SolutionDir + 'SERVERPATH')
$modPath = $($serverPath + '\NOVAKIN_Data\StreamingAssets\Mods\' + $modName)

$zipPath = $SolutionDir
$zipPath += $modName
$zipPath += ".zip"

$nkvolPath = $SolutionDir
$nkvolPath += $modName
$nkvolPath += ".nkvol"

if (Test-Path -Path $zipPath)
{
    Remove-Item -Path $zipPath -Force -ErrorAction SilentlyContinue
}

if (Test-Path -Path $nkvolPath)
{
    Remove-Item -Path $nkvolPath -Force -ErrorAction SilentlyContinue
}

New-Item -ItemType directory -Path $modPath -Force
Compress-Archive -Path $($SolutionDir + 'Scripts\ModData.json') -DestinationPath $($SolutionDir + $modName + '.zip') -Force -ErrorAction SilentlyContinue
Compress-Archive -Path $($SolutionDir + 'Scripts\*.cs') -DestinationPath $($SolutionDir + $modName + '.zip') -Force -ErrorAction SilentlyContinue
Rename-Item $($SolutionDir + $modName + '.zip') $($SolutionDir + $modName + '.nkvol') -Force -ErrorAction SilentlyContinue
Copy-Item $($SolutionDir + $modName + '.nkvol') $modPath -Force -ErrorAction SilentlyContinue
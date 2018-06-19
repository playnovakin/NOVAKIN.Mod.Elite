param([string]$ModName, [string]$SolutionDir)

$zipPath = $SolutionDir
$zipPath += $ModName
$zipPath += ".zip"

if (Test-Path -Path $zipPath)
{
    Remove-Item -Path $zipPath -Force -ErrorAction SilentlyContinue
}

$nkvolPath = $SolutionDir
$nkvolPath += $ModName
$nkvolPath += ".nkvol"

if (Test-Path -Path $nkvolPath)
{
    Remove-Item -Path $nkvolPath -Force -ErrorAction SilentlyContinue
}
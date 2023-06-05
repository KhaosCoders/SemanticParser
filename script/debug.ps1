
param(
    [string] $filePath,
    [string] $encoding
)

# Check file path
if (!$filePath) {
    Write-Host "Please enter a file to parse:"
    $filePath = Read-Host
}

if (-not (Test-Path $filePath)) {
    Write-Host "The file $filePath does not exist."
    exit 1
}

Write-Host "Parsing file: $filePath"

# Check encoding
if (!$encoding) {
    $encoding = (Get-Content $filePath).Encoding

    while (!$encoding) {
        Write-Host "Could not determine the encoding of the file. Please enter the encoding:"
        $encoding = Read-Host
    }
}

# Check parser executable
$parserProjDir = Join-Path -Path $PSScriptRoot -ChildPath "..\src"
$parserOutputExe = "SemanticParser\bin\Debug\net7.0\SemanticParser.exe"
$parserExe = Join-Path -Path $parserProjDir -ChildPath $parserOutputExe

# Build parser if it does not exist
if (-not (Test-Path $parserExe)) {
    Write-Host "The parser executable does not exist. Trying to build it."
    $msBuildPath = Get-ItemProperty -Path HKLM:\SOFTWARE\Microsoft\MSBuild\ToolsVersions\4.0 | Select-Object -ExpandProperty MSBuildToolsPath
    if (-not ($msBuildPath)) {
        Write-Host "MSBuild (.net 4.0) is not installed. Please install it and try again."
        exit 1
    }

    $msBuildPath = Join-Path -Path $msBuildPath -ChildPath "msbuild.exe"

    Push-Location $parserProjDir
    &$msBuildPath /t:Clean /p:Configuration=Debug
    &$msBuildPath /p:Configuration=Debug
    Pop-Location
}

$tempSemanticsFile = [System.IO.Path]::ChangeExtension([System.IO.Path]::GetTempFileName(), ".yaml")
$tempFlagFile = [System.IO.Path]::ChangeExtension([System.IO.Path]::GetTempFileName(), ".flag")

# Start parser
Write-Host "Starting parser..."
$pinfo = New-Object System.Diagnostics.ProcessStartInfo
$pinfo.FileName = $parserExe
$pinfo.WorkingDirectory = Split-Path $parserExe -Parent
$pinfo.RedirectStandardInput = $true
$pinfo.RedirectStandardOutput = $true
$pinfo.UseShellExecute = $false
$pinfo.Arguments = "shell", $tempFlagFile
$p = New-Object System.Diagnostics.Process
$p.StartInfo = $pinfo
if (-not $p.Start()) {
    Write-Host "Error: Could not start parser."
    exit 1
}

# Wait for flag file
Write-Host "Waiting for parser to be ready..."
while (-not (Test-Path $tempFlagFile)) {
    Start-Sleep -Milliseconds 100
}

# Feed parser
Write-Host "Feeding parser..."
Write-Host "   Input File: $filePath"
$p.StandardInput.WriteLine($filePath)
Write-Host "   Encoding: $encoding"
$p.StandardInput.WriteLine($encoding)
Write-Host "   Semantics file: $tempSemanticsFile"
$p.StandardInput.WriteLine($tempSemanticsFile)

# Read result
Write-Host "Reading result..."
$result = $p.StandardOutput.ReadLine()

# End parser
Write-Host "Ending parser..."
$p.StandardInput.WriteLine("end")
$p.WaitForExit()

# Display result
if (Test-Path $tempSemanticsFile) {
    $charposExe = "charposition\charposition.exe"
    if (Test-Path $charposExe) {
        Write-Host "Displaying result..."
        Start-Process -FilePath $charposExe -ArgumentList $filePath, $tempSemanticsFile -Wait
    } else {
        $semantics = Get-Content $tempSemanticsFile
        Write-Host "Semantics: $semantics"
    }

    Remove-Item $tempSemanticsFile
}

if ($result -ne "OK") {
    Write-Host "Error: $result"
    exit 1
}

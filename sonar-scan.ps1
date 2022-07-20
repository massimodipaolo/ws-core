param(
  [string]$configuration = 'Debug',
  [string]$path = $PSScriptRoot,
  [string[]]$targets = 'default'
)

# Read .env file: Install-Module -Name Set-PsEnv
function prompt {
    Set-PsEnv
}

prompt

# Boostrap posh-build
$build_dir = Join-Path $path ".build"
if (! (Test-Path (Join-Path $build_dir "posh-build.ps1"))) { Write-Host "Installing posh-build..."; New-Item -Type Directory $build_dir -ErrorAction Ignore | Out-Null; Save-Script "posh-build" -Path $build_dir }
. (Join-Path $build_dir "posh-build.ps1")

# Set these variables as desired
$solution_file = Join-Path $path "ws-core.sln";

target default -depends scan

target scan {
  docker start $Env:SONAR_CONTAINER
  dotnet sonarscanner begin /k:"ws-core-local" /d:sonar.host.url=$Env:SONAR_HOST_URL /d:sonar.login=$Env:SONAR_LOGIN /d:sonar.cs.opencover.reportsPaths="**/coverage.opencover.xml" /d:sonar.exclusions="src/tests/**/*,sample/**/*" /d:sonar.coverage.exclusions="src/tests/**/*,sample/**/*"
  #dotnet build 
  dotnet build $solution_file -c $configuration --no-incremental
  dotnet sonarscanner end /d:sonar.login=$Env:SONAR_LOGIN
}

Start-Build $targets
param(
  [string]$configuration = 'Release',
  [string]$path = $PSScriptRoot,
  [string[]]$targets = 'default'
)

# Read .env file: Install-Module -Name Set-PsEnv
function prompt {
    Set-PsEnv
}

function Nuget-Parse-Package($file) {
    [hashtable]$package = @{}
    $dots = $file.Split(".")
    $package.assembly = $dots[0..($dots.Length-5)] -join '.'
    $package.version = $dots[-4..-2] -join '.'
    return $package;
}

prompt

# Boostrap posh-build
$build_dir = Join-Path $path ".build"
if (! (Test-Path (Join-Path $build_dir "posh-build.ps1"))) { Write-Host "Installing posh-build..."; New-Item -Type Directory $build_dir -ErrorAction Ignore | Out-Null; Save-Script "posh-build" -Path $build_dir }
. (Join-Path $build_dir "posh-build.ps1")

# Set these variables as desired
$packages_dir = Join-Path $build_dir "packages"
$solution_file = Join-Path $path "ws-core.sln";

target default -depends compile, test, deploy

target compile {
  Invoke-Dotnet build $solution_file -c $configuration --no-incremental `
}

target test {
  # Set the path to the projects you want to test.
  $test_projects = @(
    "$path\tests\xCore\xCore.csproj"
  )

  # This runs "dotnet test". Change to Invoke-Xunit to invoke "dotnet xunit"
  Invoke-Tests $test_projects -c $configuration --no-build
}

target deploy {
  # Run dotnet pack to generate the nuget packages
  Remove-Item $packages_dir -Force -Recurse 2> $null
  Invoke-Dotnet pack $solution_file -c $configuration /p:PackageOutputPath=$packages_dir

  # Find all the packages and display them for confirmation
  $packages = dir $packages_dir -Filter "*.nupkg"

  Write-Host "Packages to upload:"
  $packages | ForEach-Object { Write-Host $_.Name }

  # Ensure we haven't run this by accident.
  $push = New-Prompt "Upload Packages" "Do you want to publish the NuGet packages?" @(
    @("&N", "Does not upload the packages."),
    @("&Y", "Uploads the packages.")
  )

  # Cancelled
  if ($push -eq 0) {
    "Upload aborted"
  }
  # upload
  elseif ($push -eq 1) {

      # Fix prev pre-release pkgs.
      $override = New-Prompt "Override Packages" "Do you want to delete the NuGet packages, if exists?" @(
        @("&N", "Does not override the packages, if exists."),
        @("&Y", "Override the packages, always.")
      )

    $packages | ForEach-Object {
      $path = $_.FullName  
      $file = $_.Name
      try { 
        $pkg = Nuget-Parse-Package($file)
        Write-Host "Check exists " $pkg.assembly $pkg.version
        $exists = nuget search $pkg.assembly -Source $Env:NUGET_SOURCE -Verbosity quiet -PreRelease -Take 1
        $version = $exists.Split($pkg.assembly + " | ")[3..3]
        Write-Host "Found version " $version
        if ($version -eq $pkg.version) {
            if ($override -eq 1) {
                Write-Host "Delete " $pkg.assembly
                Invoke-Dotnet nuget delete $pkg.assembly $pkg.version --api-key $Env:NUGET_API_KEY --source $Env:NUGET_API
            } else {
                Write-Host "Skip " $pkg.assembly
                return
            }
        } 
        Write-Host "Uploading " $pkg.assembly
        Invoke-Dotnet nuget push $path --api-key $Env:NUGET_API_KEY --source $Env:NUGET_API
      }
      catch {
        Write-Host "An error occurred:"
        Write-Host $_
      }      
      Write-Host
    }
  }
}

Start-Build $targets
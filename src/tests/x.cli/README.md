
# x.cli

Integration/Functional testing


## Code coverage

Installation
[danielpalme/ReportGenerator](https://github.com/danielpalme/ReportGenerator)

```bash
    dotnet tool install -g dotnet-reportgenerator-globaltool
```

Collect

```bash
    dotnet test -r ".\_results\test" --collect:"XPlat Code Coverage" /p:CollectCoverage=true /p:CoverletOutputFormat=opencover
```

Generate/view report

```powershell
    $test_dir = "_results\test"; `
    $test_path = Join-Path $(Get-Location) $test_dir; `
    $latest = Get-ChildItem $test_path -Attributes Directory | Sort CreationTime -Descending | Select -First 1; `
    $guid = $latest.Name; `
    $report_dir = "_results\report\coverage"; `
    reportgenerator `
    -reports:".\$test_dir\$guid\coverage.cobertura.xml" `
    -targetdir:".\$report_dir" `
    -reporttypes:Html; `
    $url = Join-Path $(Get-Location) \$report_dir\index.html; `
    Write-Host $url; `
    Start-Process "chrome.exe" $url;
```
    

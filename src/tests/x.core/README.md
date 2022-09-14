# x.core

Integration/Functional testing

## Self-Signed Certificate

IIS Express
```cmd
    cd c:\Program Files (x86)\IIS Express
    IisExpressAdminCmd.exe setupsslUrl -url:https://localhost:60937/ -UseSelfSigned
```

Kestrel / Docker
[Docs https](https://github.com/dotnet/dotnet-docker/blob/main/samples/run-aspnetcore-https-development.md)
[Docs compose](https://docs.microsoft.com/en-us/aspnet/core/security/docker-compose-https?view=aspnetcore-6.0)
```powershell
    #dotnet dev-certs https --clean
    dotnet dev-certs https -ep $env:USERPROFILE\.aspnet\https\x.core.pfx -p 2D4314AD-1E5A-4D22-8EEE-74ECEA8201DD
    dotnet dev-certs https --trust
    
```
 
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

## docker-compose
```powershell
    docker compose config
    #docker compose down
    docker compose up -d
```
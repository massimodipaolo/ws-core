# Diagnostic

#### Table of Contents

1. [Description](#description)
1. [Setup](#setup)
   - [Requirements](#setup-requirements)
   - [Configuration](#setup-configuration)
1. [Usage](#usage)
1. [Limitations](#limitations)
1. [Development](#development)

## <a id="description"></a>Description

The `Diagnostic` module installs and configures the [MiniProfiler](https://miniprofiler.com/dotnet) and the [HTTP Logging middleware](https://docs.microsoft.com/en-us/aspnet/core/fundamentals/http-logging/).

It also provides 2 Api endpoints:

- `/extensions/ws.core.extensions.diagnostic`: returns system and application diagnostic information, about the environment, app configuration, extensions injected, service implementations.
- `/extensions/ws.core.extensions.diagnostic/stop`: performs an application lifetime stop.

## <a id="setup"></a>Setup

You can install the `Diagnostic` module adding the reference in your `Core` application and configure it in the `ext-settings.json` file.

### <a id="setup-requirements"></a>Requirements

No other modules are required.

### <a id="setup-configuration"></a>Configuration

You need to define 2 options to configure the module:

1. **profiler** (_optional_)

   - **enable** (_default_: `false`): enables `MiniProfiler for ASP.NET Core`
   - **config** (_optional_): refer to official [documentation](https://miniprofiler.com/dotnet/AspDotNetCore)

1. **httpLogging** (_optional_)
   - **enable** (_default_: `false`): enables `HTTP Logging middleware`
   - **config** (_optional_): refer to official [documentation](https://docs.microsoft.com/en-us/dotnet/api/microsoft.aspnetcore.httplogging.httploggingoptions)

#### Configuration example

```json
{
  "extConfig": {
    "assemblies": {
      "Ws.Core.Extensions.Diagnostic": {
        "priority": 100,
        "options": {
          "httpLogging": {
            "enable": true,
            "config": {
              "loggingFields": "All",
              "requestBodyLogLimit": 4096,
              "responseBodyLogLimit": 4096,
              "requestHeaders": ["Host", "Referer", "sec-ch-ua"],
              "responseHeaders": [
                "Cache-Control",
                "Content-Encoding",
                "Content-Type",
                "Set-Cookie"
              ]
            }
          },
          "profiler": {
            "enable": true,
            "config": {
              "EnableDebugMode": true,
              "ColorScheme": "Light"
            }
          }
        }
      }
    }
  }
}
```

## <a id="usage"></a>Usage

### HTTP Logging

Add the following line to the `appsettings.Development.json` file.

```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    },
    "Console": {
      "LogLevel": {
        "Default": "Error",

        // Added HTTP Logging configuration
        "Microsoft.AspNetCore.HttpLogging.HttpLoggingMiddleware": "Information"
      }
    }
  }
}
```

### MiniProfiler

`MiniProfiler` exposes 3 endpoints, the root is determined by `MiniProfilerOptions.RouteBasePath` (defaults to `/mini-profiler-resources`):

### Using UI

1. `/<base>/results-index`: A list of recent profilers.
2. `/<base>/results`: Views either the very last profiler for the current user or a specific profiler via `?id={guid}`.

### JSON format

3. `/<base>/results-list`: A list of recent profilers.

## <a id="limitations"></a>Limitations

Nothing to report.

## <a id="development"></a>Development

Development note, contribution info, licensing to be defined.

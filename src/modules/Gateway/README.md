# Gateway

#### Table of Contents

1. [Description](#description)
1. [Setup](#setup)
   - [Requirements](#setup-requirements)
   - [Configuration](#setup-configuration)
1. [Usage](#usage)
1. [Limitations](#limitations)
1. [Development](#development)

## <a id="description"></a>Description

The `Gateway` module installs and configures a Gateway Api endpoint using [Ocelot](https://ocelot.readthedocs.io/).

## <a id="setup"></a>Setup

You can install the `Gateway` module adding the reference in your `Core` application and configure it in the `ext-settings.json` file.

### <a id="setup-requirements"></a>Requirements

No other modules are required.

### <a id="setup-configuration"></a>Configuration

You need to define 2 options to configure the module:

1. **mapWhen** (_default_: `string empty`): regex-based role to use gateway pipeline, otherwise catchAll request.
1. **ocelot**: Ocelot configuration options. See [Ocelot docs](https://ocelot.readthedocs.io/en/latest/features/configuration.html) for details.

#### Configuration example

```json
{
  "extConfig": {
    "assemblies": {
      "Ws.Core.Extensions.Gateway": {
        "priority": 100,
        "options": {
          "MapWhen": "/bing",
          "Ocelot": {
            "Routes": [
              {
                "UpstreamPathTemplate": "/bing/{everything}",
                "_UpstreamHttpMethod": [
                  "OPTIONS",
                  "POST",
                  "PUT",
                  "GET",
                  "DELETE"
                ],
                "_UpstreamHost": "host.com",
                "DangerousAcceptAnyServerCertificateValidator": true,
                "DownstreamPathTemplate": "/search?q={everything}",
                "DownstreamScheme": "https",
                "DownstreamHostAndPorts": [
                  {
                    "_Host": "localhost",
                    "_Port": 60936,
                    "Host": "www.bing.com",
                    "Port": 443
                  }
                ],
                "_RateLimitOptions": {
                  "ClientWhitelist": [],
                  "EnableRateLimiting": true,
                  "Period": "1s",
                  "PeriodTimespan": 1, // retry after
                  "Limit": 3
                },
                "Priority": 1
              }
            ],
            "_DynamicRoutes": [],
            "_Aggregates": [],
            "GlobalConfiguration": {
              "BaseUrl": "https://localhost:60935",
              "_RequestIdKey": null,
              "_ServiceDiscoveryProvider": {
                "Scheme": null,
                "Host": null,
                "Port": 0,
                "Type": null,
                "Token": null,
                "ConfigurationKey": null,
                "PollingInterval": 0,
                "Namespace": null
              },
              "_RateLimitOptions": {
                "ClientIdHeader": "ClientId",
                "QuotaExceededMessage": null,
                "RateLimitCounterPrefix": "ocelot",
                "DisableRateLimitHeaders": false,
                "HttpStatusCode": 429
              },
              "_QoSOptions": {
                "ExceptionsAllowedBeforeBreaking": 0,
                "DurationOfBreak": 0,
                "TimeoutValue": 0
              },
              "_LoadBalancerOptions": {
                "Type": null,
                "Key": null,
                "Expiry": 0
              },
              "_DownstreamScheme": null,
              "_HttpHandlerOptions": {
                "AllowAutoRedirect": false,
                "UseCookieContainer": false,
                "UseTracing": false,
                "UseProxy": true,
                "MaxConnectionsPerServer": 2147483647
              },
              "_DownstreamHttpVersion": null
            }
          }
        }
      }
    }
  }
}
```

## <a id="usage"></a>Usage

Once configured no other code is required.

## <a id="limitations"></a>Limitations

Nothing to report.

## <a id="development"></a>Development

Development note, contribution info, licensing to be defined.

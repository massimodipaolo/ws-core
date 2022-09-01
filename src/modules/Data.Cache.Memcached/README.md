# Data.Cache.Memcached

#### Table of Contents

1. [Description](#description)
1. [Setup](#setup)
   - [Requirements](#setup-requirements)
   - [Configuration](#setup-configuration)
1. [Usage](#usage)
1. [Limitations](#limitations)
1. [Development](#development)

## <a id="description"></a>Description

The `Data.Cache.Memcached` module installs and configure the implementation of the `Data.Cache.ICache` interface of the generic `Data.Cache` for `Memcached`.

## <a id="setup"></a>Setup

You can install the `Data.Cache.Memcached` module adding the reference in your `Core` application and configure it in the `ext-settings.json` file.

### <a id="setup-requirements"></a>Requirements

No other modules are required.

### <a id="setup-configuration"></a>Configuration

You need to define 2 options to configure the module:

1. **client**: `Memcached` client options, you can refer to the `EnyimMemcachedCore` [project](https://github.com/cnblogs/EnyimMemcachedCore).

1. **entryExpirationInMinutes**: overrides the default duration (in minutes) of the provided cache profiles. If omitted default values will be used.
   - **fast** (_default_: `10`)
   - **medium** (_default_: `60`)
   - **slow** (_default_: `240`)
   - **never** (_default_: `1440`)

#### Configuration example

```json
{
  "extConfig": {
    "assemblies": {
      "Ws.Core.Extensions.Data.Cache.Memcached": {
        "priority": 100,
        "options": {
          "client": {
            "servers": [
              {
                "address": "localhost",
                "port": 11211
              }
            ]
          },
          "entryExpirationInMinutes": {
            "fast": 1,
            "medium": 5,
            "slow": 60,
            "never": 1440
          }
        }
      }
    }
  }
}
```

## <a id="usage"></a>Usage

See documentation of the `data.cache` module [usage](../Data.Cache/README.md#usage).

## <a id="limitations"></a>Limitations

Nothing to report.

## <a id="development"></a>Development

Development note, contribution info, licensing to be defined.

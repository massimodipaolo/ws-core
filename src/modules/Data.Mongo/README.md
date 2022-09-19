# Data.Mongo

#### Table of Contents

1. [Description](#description)
1. [Setup](#setup)
   - [Requirements](#setup-requirements)
   - [Configuration](#setup-configuration)
1. [Usage](#usage)
1. [Limitations](#limitations)
1. [Development](#development)

## <a id="description"></a>Description

The `Data.Mongo` module installs and configure the implementation of the `Data.IRepository` interface of the generic `Data` module to store and retrieve data on a Mongo DB instance.

## <a id="setup"></a>Setup

You can install the `Data.Mongo` module adding the reference in your `Core` application and configure it in the `ext-settings.json` file.

### <a id="setup-requirements"></a>Requirements

No other modules are required.

### <a id="setup-configuration"></a>Configuration

You need to define 1 options to configure the module:

1. **connections**: list of `Data.DbConnection` configurations in order to define all the connections that you are going to use.

#### Configuration example

```json
{
  "extConfig": {
    "assemblies": {
      "Ws.Core.Extensions.Data.Mongo": {
        "priority": 100,
        "options": {
          "connections": [
            {
              "name": "default",
              "connectionString": "mongodb://user:password@localhost:27017",
              "database": "test-db"
            }
          ]
        }
      }
    }
  }
}
```

## <a id="usage"></a>Usage

See documentation of the `data` module [usage](../Data/README.md#usage).

## <a id="limitations"></a>Limitations

⚠️Be aware that the `Query` method is not implemented so this module cannot completely replace the other implementations. Use only if you need direct access by Id.

⚠️Be aware that the `Data.DbConnectionSelector` is not used in this module so configuring more than one connections will result that only the first one with name "**default**" will be used.

## <a id="development"></a>Development

Development note, contribution info, licensing to be defined.

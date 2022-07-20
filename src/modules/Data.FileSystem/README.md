# Data.FileSystem

#### Table of Contents

1. [Description](#description)
1. [Setup](#setup)
   - [Requirements](#setup-requirements)
   - [Configuration](#setup-configuration)
1. [Usage](#usage)
1. [Limitations](#limitations)
1. [Development](#development)

## <a id="description"></a>Description

The `Data.FileSystem` module installs and configure the implementation of the `Data.IRepository` interface of the generic `Data` module to store and retrieve data on the filesystem as a serialized JSON file.

## <a id="setup"></a>Setup

You can install the `Data.FileSystem` module adding the reference in your `Core` application and configure it in the `ext-settings.json` file.

The JSON file to store the entity must be created manually before usage, the accepted names are:

1. Entity name (ex. Product.json)
1. Entity namespace (ex. testapp.Entities.Product.json)

### <a id="setup-requirements"></a>Requirements

No other modules are required.

### <a id="setup-configuration"></a>Configuration

You need to define 2 options to configure the module:

1. **folder**: physical folder on the filesystem for the storage of the data json files.
2. **converters**: configuration options of the `Shared.Serialization` module to use to serialize data.

#### Configuration example

```json
{
  "extConfig": {
    "assemblies": {
      "Ws.Core.Extensions.Data.FileSystem": {
        "priority": 100,
        "options": {
          "folder": "C:\\data",
          "serialization": {
            "converters": [
              {
                "assembly": "testapp",
                "type": "testapp.Code.LocaleJsonConverter"
              }
            ]
          }
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

## <a id="development"></a>Development

Development note, contribution info, licensing to be defined.

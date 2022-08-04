# Data.EF.MySQL

#### Table of Contents

1. [Description](#description)
1. [Setup](#setup)
   - [Requirements](#setup-requirements)
   - [Configuration](#setup-configuration)
1. [Usage](#usage)
1. [Limitations](#limitations)
1. [Development](#development)

## <a id="description"></a>Description

The `Data.EF.MySQL` module installs and configure the implementation of the `Data.IRepository` interface of the generic `Data` module to store and retrieve data on a MySQL instance using **Entity Framework**.

The base implementation of the `Data.IRepository` is inherited from the [Data.EF](../Data.EF/README.md) module.

## <a id="setup"></a>Setup

You can install the `Data.EF.MySQL` module adding the reference in your `Core` application and configure it in the `ext-settings.json` file.

### <a id="setup-requirements"></a>Requirements

No other modules are required.

### <a id="setup-configuration"></a>Configuration

You need to define 2 options to configure the module:

1. **connections**: list of `Data.DbConnection` configurations in order to define all the connections that you are going to use.

1. **serviceLifetime** (_default_: `Scoped`): declare the lifetime of the service in the dependency injection engine defined by [Microsoft.Extensions.DependencyInjection.ServiceLifetime](http://docs.microsoft.com/dotnet/api/microsoft.extensions.dependencyinjection.servicelifetime).

#### Configuration example

```json
{
  "extConfig": {
    "assemblies": {
      "Ws.Core.Extensions.Data.EF.MySql": {
        "priority": 100,
        "options": {
          "connections": [
            {
              "name": "default",
              "connectionString": "Server=localhost;Port=33060;Database=dbname;Uid=user;Pwd=password;"
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

Nothing to report.

## <a id="development"></a>Development

Development note, contribution info, licensing to be defined.

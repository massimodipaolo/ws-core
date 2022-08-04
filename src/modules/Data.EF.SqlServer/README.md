# Data.EF.SqlServer

#### Table of Contents

1. [Description](#description)
1. [Setup](#setup)
   - [Requirements](#setup-requirements)
   - [Configuration](#setup-configuration)
     - [Stored procedure mappings](#setup-configuration-spm)
1. [Usage](#usage)
   - [Stored procedure usage](#usage-sp)
1. [Limitations](#limitations)
1. [Development](#development)

## <a id="description"></a>Description

The `Data.EF.SqlServer` module installs and configure the implementation of the `Data.IRepository` interface of the generic `Data` module to store and retrieve data on a Microsoft SQL Server instance using **Entity Framework**.

The base implementation of the `Data.IRepository` is inherited from the [Data.EF](../Data.EF/README.md) module.

## <a id="setup"></a>Setup

You can install the `Data.EF.SqlServer` module adding the reference in your `Core` application and configure it in the `ext-settings.json` file.

### <a id="setup-requirements"></a>Requirements

No other modules are required.

### <a id="setup-configuration"></a>Configuration

You need to define 4 options to configure the module (2 mandatory and 2 optional):

1. **connections**: list of `Data.DbConnection` configurations in order to define all the connections that you are going to use.

1. **serviceLifetime** (_default_: `Scoped`): declare the lifetime of the service in the dependency injection engine defined by [Microsoft.Extensions.DependencyInjection.ServiceLifetime](http://docs.microsoft.com/dotnet/api/microsoft.extensions.dependencyinjection.servicelifetime).

1. **storedProcedure** (_optional_): with this option you can declare a stored procedure, with a list of methods, to use with an entity instead of the default `Data.EF` repository implementation.

1. **merge** (_optional_): [EFCore.BulkExtensions](https://github.com/borisdj/EFCore.BulkExtensions) configuration used in the Merge repository method.
   - **useTempDB** (_default_: `true`): create the new tables used for the merge operations in a temporary DB. When set then the bulk operation has to be inside a transaction.
   - **bulkCopyTimeout** (_default_: `180`): sets the timeout (in seconds) for the bulk operation to complete.

#### <a id="setup-configuration-spm"></a>Stored procedure mappings

1. **name**: name of the entity type to map with the stored procedure.
1. **nameSpace** (_optional_): name space of the entity to map with the stored procedure. If empty only the name will be used to map the entity type.
1. **methods** (_optional_): list of methods to use with the store procedure. If empty all available methods will be used.
   Available methods:
   - **List**
   - **Find**
   - **Add**
   - **AddMany**
   - **Update**
   - **UpdateMany**
   - **Merge**
   - **Delete**
   - **DeleteMany**
1. **schema** (_optional_): stored procedure mapping schema. If empty the main schema will be used.
1. **storedProcedure** (_optional_): store procedure name. If empty the entity name will be used.
1. **commandTimeOut**: defines the timeouts of the crud operations via stored procedure methods.
   - **Read** (_default_: `60`)
   - **Write**: (_default_: `120`)
   - **Sync**: (_default_: `180`)

#### Configuration example

```json
{
  "extConfig": {
    "assemblies": {
      "Ws.Core.Extensions.Data.EF.SqlServer": {
        "priority": 100,
        "options": {
          "connections": [
            {
              "name": "default",
              "connectionString": "Server=localhost;Database=dbname;User Id=user;Password=password;MultipleActiveResultSets=true"
            }
          ],
          "merge": {
            "useTempDB": true,
            "bulkCopyTimeout": 60
          },
          "storedProcedure": {
            "schema": "dbo",
            "mappings": [
              {
                "nameSpace": "testapp.Models",
                "name": "Product",
                "methods": [
                  "List",
                  "Find",
                  "Add",
                  "AddMany",
                  "Update",
                  "UpdateMany",
                  "Delete",
                  "DeleteMany",
                  "Merge"
                ],
                "schema": "crud",
                "storedProcedure": "prodsp",
                "commandTimeOut": {
                  "read": 60,
                  "write": 120,
                  "sync": 180
                }
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

### <a id="usage-sp"></a>Stored procedure usage

To use the stored procedure implementation instead of the `Data.EF` default, you need to create and configure your stored procedure following the configuration section.

The stored procedure names in the database should follow this pattern:

```csharp
{schema}.entity_{storedProcedure}_{method}
```

Examples:

1. crud.entity_prodsp_list
1. crud.entity_prodsp_find
1. etc..

## <a id="limitations"></a>Limitations

Nothing to report.

## <a id="development"></a>Development

Development note, contribution info, licensing to be defined.

# Data.EF

#### Table of Contents

1. [Description](#description)
1. [Setup](#setup)
   - [Requirements](#setup-requirements)
   - [Configuration](#setup-configuration)
1. [Usage](#usage)
1. [Limitations](#limitations)
1. [Development](#development)

## <a id="description"></a>Description

The `Data.EF` module is the generic implementation of the `Data` module for **Entity Framework**.

It's a dependency of all the `data` module implementation based on EF (i.e. `Data.EF.MySql`, `Data.EF.SqlServer`, etc).

## <a id="setup"></a>Setup

If you only need specific implementations of the data module you don't need to install this module manually because it will be installed automatically as a dependency of the implementation module.

### <a id="setup-requirements"></a>Requirements

No other modules are required.

### <a id="setup-configuration"></a>Configuration

Even if you are using one or more specific implementation of the `data` module based on EF you can define the configuration of the `Data.EF` module in the `ext-settings.json`.

This configuration will be used for every implementation module that depends on it.

- **ignore** (_optional_): list of entity types to exclude from the model. This parameter is typically used to remove types from the model that were added by convention (i.e. `MyNamespace.MyClass`, `MyAssembly`, etc).
- **jsonConvert** (_optional_): serialize/deserialize type/interface, mapped on a text column.
- **includeNavigationProperties** (_optional_): criteria of inclusion of the navigation properties when retrieving the entities with `List` and `Find` methods. You can define different criteria for each method.
  - **enable** (_default_: `false`): include the main navigation properties.
  - **except** (_optional_): list of entity type names (`typeof(T).FullName`) to exclude from then **enabled** directive.
  - **explicit** (_optional_): sets custom navigation paths. This setting overrides the **enabled**/**except** properties.
    - **entity**: entity type name (`typeof(T).FullName`)
    - **paths**: list of indented navigation paths to include. i.e.:
      ```json
      [["OrderDetail", "Product", "Supplier"], ["Customer"]]
      ```
- **mappings** (_optional_): list of mapping configuration for the EF entity mapper.
  - **name**: entity type name (`typeof(T).FullName`), used to match the mapping configuration.
  - **nameSpace** (_optional_): namespace of the entity to match the mapping configuration. If not provided only the name will be used.
  - **table** (_optional_): database table name to map to the entity. If not provided it will be assumed that the table name is the name of the entity type.
  - **schema** (_optional_): database schema of the table mapped to the entity type. If not provided it will be assumed that the table is in the main schema.
  - **idColumnName** (_optional_): name of the id column on the database table mapped to the entity. If not provided it will be assumed as the same name of the `id` property.
  - **idHasDefaultValue** (_default_: `true`): tells EF that the id field has a default value on the database table.
  - **properties** (_optional_): list of specific properties to map to custom columns or to ignore in the entity mapping.
    - **name**: name of the property on the entity.
    - **column** (_optional_): name of the custom column on the database table.
    - **ignore** (_default_: `false`): use to ignore the property field from the entity mapping.
    - **jsonConvert** (_optional_): configures the property to be stored in a text column on the database and to be serialized/deserialized as json when writing/reading from the table.
    - **hasConversion** (_optional_): configures the property so that the property value is converted before writing to the database and converted back when reading from the database.
      The parameter is not a simple boolean but you need to specify one of the available CLR type conversions: `string`, `int`, `long`, `bool`, `char`, `DateTime`, `DateTimeOffset`, `TimeSpan`, `Guid`, `byte[]`.

#### Configuration example

```json
      "Ws.Core.Extensions.Data.EF": {
        "priority": 100,
        "options": {
          "jsonConvert": [ "testApp.Models.IAppJsonSerializable, testApp" ],
          "includeNavigationProperties": {
            "list": {
              "enable": false,
              "except": [ "testApp.Models.Store.Product" ]
            },
            "find": {
              "enable": true,
              "explicit": [
                {
                  "entity": "testApp.Models.User",
                  "paths": [
                    [ "Albums", "Photos" ]
                  ],
                  "_paths": [
                    [ "Posts", "Comments" ],
                    [ "Albums", "Photos" ],
                    [ "Todos" ]
                  ]
                }
              ]
            }
          },
          "mappings": [
            {
              "name": "Log",
              "properties": [
                {
                  "name": "Level",
                  "hasConversion": "string"
                }
              ]
            },
            {
              "namespace": "testApp.Models.Store",
              "name": "Brand",
              "schema": "production"
            },
            {
              "namespace": "testApp.Models.Store",
              "name": "Category",
              "schema": "production"
            },
            {
              "namespace": "testApp.Models.Store",
              "name": "Product",
              "schema": "production"
            },
            {
              "namespace": "testApp.Models.Cms",
              "name": "Admin_User",
              "table": "Admin_Users"
            },
            {
              "namespace": "testApp.Models.Cms",
              "name": "Admin_Role",
              "table": "Admin_Roles"
            },
            {
              "namespace": "testApp.Models.Cms",
              "name": "Admin_Users_Roles_Links",
              "properties": [
                {
                  "name": "Admin_UserId",
                  "column": "User_Id"
                },
                {
                  "name": "Admin_RoleId",
                  "column": "Role_Id"
                }
              ]
            },
            {
              "namespace": "testApp.Models.Cms",
              "name": "Admin_Permission",
              "table": "Admin_Permissions"
            }
          ]
        }
      },
```

## <a id="usage"></a>Usage

See documentation of the `data` module [usage](../Data/README.md#usage).

For specific database implementation see the corresponding module documentation.

## <a id="limitations"></a>Limitations

Nothing to report.

## <a id="development"></a>Development

Development note, contribution info, licensing to be defined.

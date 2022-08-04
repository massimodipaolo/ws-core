# Data

#### Table of Contents

1. [Description](#description)
   - [Entity](#description-entity)
     - [`IEntity`](#description-entity-ientity)
     - [`IEntity<TKey>`](#description-entity-ientity-tkey)
     - [`Entity<TKey>`](#description-entity-entity-tkey)
     - [`EntityComparer<T, TKey>`](#description-entity-entity-comparer)
   - [Repository](#description-repository)
     - [`IRepository`](#description-repository-irepo)
     - [`IRepository<T>`](#description-repository-irepot)
       - [`List`](#description-repository-irepot-list)
       - [`Query`](#description-repository-irepot-query)
       - [`Add`](#description-repository-irepot-add)
       - [`AddMany`](#description-repository-irepot-add-many)
       - [`Update`](#description-repository-irepot-update)
       - [`UpdateMany`](#description-repository-irepot-update-many)
       - [`Merge`](#description-repository-irepot-merge)
       - [`Delete`](#description-repository-irepot-delete)
       - [`DeleteMany`](#description-repository-irepot-delete-many)
     - [`IRepository<T, TKey>`](#description-repository-irepot-key)
       - [`Find`](#description-repository-irepot-key-find)
     - [`BaseRepository`](#description-repository-base-repository)
     - [`InMemory<T, TKey>`](#description-repository-in-memory)
   - [DbConnection](#description-dbconnection)
     - [`IDbConnectionFunctionWrapper`](#description-dbconnection-wrapper)
     - [`DbConnection`](#description-dbconnection-dbconn)
     - [`DbConnectionSelector`](#description-dbconnection-selector)
1. [Setup](#setup)
   - [Requirements](#setup-requirements)
   - [Configuration](#setup-configuration)
1. [Usage](#usage)
   - [Entity](#usage-entity)
   - [Repository](#usage-repository)
   - [DbConnection](#usage-dbconnection)
1. [Limitations](#limitations)
1. [Development](#development)

## <a id="description"></a>Description

The `Data` module is the generic module for the data access layer.

It contains the interfaces and base classes for entities, repositories, database connections and a specific implementation of `IRepository` that store data in memory.

### <a id="description-entity"></a>Entity

#### <a id="description-entity-ientity"></a>`IEntity`

Generic empty entity interface.

#### <a id="description-entity-ientity-tkey"></a>`IEntity<TKey>`

Generic entity interface with a specific key type. It adds the `Id` attribute as `TKey` type.

#### <a id="description-entity-entity-tkey"></a>`Entity<TKey>`

Base entity class that implements `IEntity<TKey>` and defines the `Id` attribute as `TKey` type.

#### <a id="description-entity-entity-comparer"></a>`EntityComparer<T, TKey>`

Base class to compare entity objects.

### <a id="description-repository"></a>Repository

#### <a id="description-repository-irepo"></a>`IRepository`

Generic empty repository interface.

#### <a id="description-repository-irepot"></a>`IRepository<T>`

Generic repository interface for an entity of type `T`.

It inherits from `IRepository` and adds the following methods.

##### <a id="description-repository-irepot-list"></a>`List`

Property that returns a queryable object on all the entities of type `T` present in the data storage.

```csharp
IQueryable<T> List { get; }
```

##### <a id="description-repository-irepot-query"></a>`Query`

Method that returns a queryable object on the entities of type `T` present in the data storage that matches the query parameter defined as a [FormattableString](#https://docs.microsoft.com/dotnet/api/system.formattablestring).

```csharp
IQueryable<T> Query(FormattableString command)
```

##### <a id="description-repository-irepot-add"></a>`Add`

Method that adds the entity of type `T` to the data storage.

```csharp
void Add(T entity)
```

##### <a id="description-repository-irepot-add-many"></a>`AddMany`

Method that adds multiple entities of type `T` to the data storage.

```csharp
void AddMany(IEnumerable<T> entities)
```

##### <a id="description-repository-irepot-update"></a>`Update`

Method that updates the entity of type `T` in the data storage.

```csharp
void Update(T entity)
```

##### <a id="description-repository-irepot-update-many"></a>`UpdateMany`

Method that updates multiple entities of type `T` in the data storage.

```csharp
void UpdateMany(IEnumerable<T> entities)
```

##### <a id="description-repository-irepot-merge"></a>`Merge`

Method that merges the state of multiple entities of type `T` to the data currently present in the storage.

Use the operation parameter to specify the desired type of merge operations:

- **Upsert** (_default_): inserts and updates the given entities on the data storage. No delete will be performed on other stored entities.
- **Sync**: inserts and updates the given entities on the data storage and deletes all the other entities of the same type from the data storage. Data storage will be in _sync_ with the given entity list.

```csharp
void Merge(IEnumerable<T> entities, RepositoryMergeOperation operation = RepositoryMergeOperation.Upsert)
```

##### <a id="description-repository-irepot-delete"></a>`Delete`

Method that deletes the entity of type `T` from the data storage.

```csharp
void Delete(T entity)
```

##### <a id="description-repository-irepot-delete-many"></a>`DeleteMany`

Method that deletes multiple entities of type `T` from the data storage.

```csharp
void DeleteMany(IEnumerable<T> entities)
```

#### <a id="description-repository-irepot-key"></a>`IRepository<T, TKey>`

Generic repository interface for an entity of type `T` with `Id` field of type `TKey`.

It inherits from `IRepository<T, TKey>` and adds the following methods.

##### <a id="description-repository-irepot-key-find"></a>`Find`

Method that tries to retrieve the entity of type `T` in the data storage by matching the given id field of type `TKey`.

If the entity is not found it will return `null`, otherwise the retrieved entity object.

```csharp
T? Find(TKey? Id)
```

#### <a id="description-repository-base-repository"></a>`BaseRepository`

Base repository class that implements the `IRepository` interface.

#### <a id="description-repository-in-memory"></a>`InMemory<T, TKey>`

Implementation of the `IRepository<T, TKey>` interface to manage entity storage directly in memory.

### <a id="description-dbconnection"></a>DbConnection

You need to use directly the `DbConnection` interfaces and classes only if you are creating your own implementation module that needs a connection to a database.

If you are using an implementation module (i.e. `Data.Mongo`) this layer is managed directly and you don't need to write any code but just using the `ext-settings.json` configuring all the connections that you require.

#### <a id="description-dbconnection-wrapper"></a>`IDbConnectionFunctionWrapper`

Generic interface for the implementation of the function that returns the `DbConnection` object related to a specific type.

#### <a id="description-dbconnection-dbconn"></a>`DbConnection`

Class that represents a generic `DbConnection` and the criteria to automatically include/exclude `IEntity` objects from the connection selector.

#### <a id="description-dbconnection-selector"></a>`DbConnectionSelector`

Class that manages multi `DbConnection` configurations based on the type of the entity.

## <a id="setup"></a>Setup

If you only need specific implementations of the data module you don't need to install this module manually because it will be installed automatically as a dependency of the implementation module.

You need to install and add it to the `ext-settings.json` file only if you need the `InMemory` repository implementation.

### <a id="setup-requirements"></a>Requirements

No other modules are required.

### <a id="setup-configuration"></a>Configuration

No options are defined for this module

#### Configuration example

```json
{
  "extConfig": {
    "assemblies": {
      "Ws.Core.Extensions.Data": {
        "priority": 100,
        "options": {}
      }
    }
  }
}
```

## <a id="usage"></a>Usage

### <a id="usage-entity"></a>Entity

Your entity must implement the `IEntity<TKey>` interface, you need also to specify the type of the entity key:

```csharp
    public class Product : IEntity<int>
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }
```

You can also inherit from the `Entity<TKey>` class, this way you can avoid declaring the Id field inside the class:

```csharp
    public class Product : Entity<int>
    {
        public string Name { get; set; }
    }
```

### <a id="usage-repository"></a>Repository

If you have installed only an implementation of `IRepository` you can directly use the `IRepository<T, TKey>` interface, the dependency injection engine will directly resolve the only implementation.

If you have multiple implementations installed you can force the one to use using the `Priority` key in the JSON config of the module, only the one with the highest priority (lowest number) will be resolved for the whole application.

If you want to use an implementation for a type of entity and another implementation for another type of entity you must manually override the `IRepository` registration for the entity in the `ConfigureServices` method of the `Startup` class:

```csharp
   public class Startup : Ws.Core.Startup<Ws.Core.AppConfig>
   {
       public override void ConfigureServices(WebApplicationBuilder builder)
       {
           base.ConfigureServices(builder);

           Ws.Core.AppInfo<Ws.Core.AppConfig>.Set(env: env, config: config, services: builder.Services);

           // override IRepository<Product, int> implementation
           builder.Services.AddTransient(typeof(Ws.Core.Extensions.Data.IRepository<Product, int>), typeof(Ws.Core.Extensions.Data.Repository.FileSystem<Product, int>));

           builder.Services.AddTransient<TestApplication>();
       }
   }
```

#### Usage example

```csharp
    public class TestApplication
    {
        IRepository<Product, int> _productRepo;

        public TestApplication(IRepository<Product, int> productRepo)
        {
            _productRepo = productRepo;
        }

        public List<Product> Run(List<Product> products)
        {
            _productRepo.DeleteMany(products);

            _productRepo.AddMany(products);

            return _productRepo.List.ToList();
        }
    }
```

### <a id="usage-dbconnection"></a>DbConnection

You can define multiple DB connection configurations in the `ext-settings.json` of the implementation modules that uses the `DbConnection` and the `DbConnectionSelector` (i.e. `Data.EF.SqlServer`, `Data.EF.MySql`, etc..).

To define for witch entities (`IEntity` implementations objects) you want to use a specific connection you can use the `serviceResolver` configuration parameter in the json configuration.

If you configure multiple connections without specifying additional discrimination criteria in the `serviceResolver`, all the entities will be registered for every connection but only the first one will be effectively used.

#### DbConnection configuration

1. **name** (_default_: "default"): the name of the connection.

1. **connectionString** (_optional_): string used to initialize the connection if needed by the implementation.

1. **database** (_optional_): name of the database if not specified in the connection string.

1. **serviceResolver**: defines the criteria used to associate entities to specific connections:
   - **include**: inclusion criteria, defines witch entities will use this connection.
   - **exclude**: exclusion criteria, defines witch entities will NOT use this connection.

##### ServiceResolverSelector criteria configurations

You can configure both the `include` and `exclude` criteria in the `serviceResolver` parameter section by deciding the entity matching pattern.

1. **assembly**: match will be made by the assembly name. All entities found in the assembly will be selected (i.e. "_testApp_").

1. **namespace**: match will be made by namespace. All entities found in the given namespace will be selected (i.e. "_testApp.Models_").

1. **fullName**: match the full name of the entity (i.e. "_testApp.Models.Product_").

#### Usage examples

```json
    "Ws.Core.Extensions.Data.EF.SqlServer": {
        "priority": 100,
        "options": {
          "connections": [
            {
              "name": "store",
              "connectionString": "Server=localhost;Database=store;User Id=user;Password=pass;MultipleActiveResultSets=true",
              "serviceResolver": {
                "include": { "namespace": [ "testApp.Models.Store" ] }
              }
            },
            {
              "name": "app",
              "connectionString": "Server=localhost;Database=app;User Id=user;Password=pass;MultipleActiveResultSets=true"
            }
          ]
        }
    }
```

```json
    "Ws.Core.Extensions.Data.EF.MySql": {
        "priority": 100,
        "options": {
            "connections": [
            {
                "name": "default",
                "connectionString": "Server=localhost;Port=33061;Database=dbname;Uid=user;Pwd=pass;",
                "serviceResolver": { "exclude": { "assembly": [ "FakeAssembly" ] } }
            },
            {
                "name": "backup",
                "connectionString": "Server=localhost;Port=33061;Database=dbname;Uid=user;Pwd=pass;"
            }
          ]
        }
    }
```

## <a id="limitations"></a>Limitations

⚠️Be aware that the `Query` method is not implemented in the `InMemory` repository so this module cannot completely replace the other implementations. Use only if you need direct access by Id.

## <a id="development"></a>Development

Development note, contribution info, licensing to be defined.

```

```

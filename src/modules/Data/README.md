# Data

#### Table of Contents

1. [Description](#description)
   - [Entity](#description-entity)
   - [Repository](#description-repository)
   - [DbConnection](#description-dbconnection)
1. [Setup](#setup)
   - [Requirements](#setup-requirements)
   - [Configuration](#setup-configuration)
1. [Usage](#usage)
   - [Entity](#usage-entity)
   - [Repository](#usage-repository)
1. [Limitations](#limitations)
1. [Development](#development)

## <a id="description"></a>Description

The `Data` module is the generic module for the data access layer.

It contains the interfaces and base classes for entities, repositories, database connections and a specific implementation of `IRepository` that store data in memory.

### <a id="description-entity"></a>Entity

#### `IEntity`

Generic entity interface.

#### `IEntity<TKey>`

Generic entity interface with a specific key type.

#### `Entity<TKey>`

Base entity class that implements `IEntity<TKey>` and defines the `Id` attribute as `TKey` type.

#### `EntityComparer<T, TKey>`

Base class to compare entity objects.

### <a id="description-repository"></a>Repository

#### `IRepository`

Generic repository interface.

#### `IRepository<T>`

Generic repository interface for an entity of type `T`.

#### `IRepository<T, TKey>`

Generic repository interface for an entity of type `T` with `Id` field of type `TKey`.

#### `BaseRepository`

Base repository class.

#### `InMemory<T, TKey>`

Implementation of the `IRepository<T, TKey>` interface to manage entity storage directly in memory.

### <a id="description-dbconnection"></a>DbConnection

You need to use directly the `DbConnection` interfaces and classes only if you are creating your own implementation module that needs a connection to a database.

If you are using an implementation module (ex. `Data.Mongo`) this layer is managed directly and you don't need to write any code but just using the `ext-settings.json` configuring all the connections that you require.

#### `IDbConnectionFunctionWrapper`

Generic interface for the implementation of the function that returns the `DbConnection` object related to a specific type.

#### `DbConnection`

Class that represents a generic `DbConnection` and the criteria to automatically include/exclude `IEntity` objects from the connection selector.

#### `DbConnectionSelector`

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

## <a id="limitations"></a>Limitations

⚠️Be aware that the `Query` method is not implemented in the `InMemory` repository so this module cannot completely replace the other implementations. Use only if you need direct access by Id.

## <a id="development"></a>Development

Development note, contribution info, licensing to be defined.

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

The `Data.FileSystem` module installs and configure the implementation of the `Data.IRepository` intarface of the generic `Data` module to store and retrieve data on the filesystem as a serialized JSON file.

## <a id="setup"></a>Setup

You can install the `Data.FileSystem` module adding the reference in your `Core` application and configure it in the `ext-settings.json` file.

The JSON file to store the entity must be created manually before usage, the accepted names are:

1. Entity name (ex. Product.json)
1. Entity namespace (ex. testapp.Entities.Product.json)

### <a id="setup-requirements"></a>Requirements

You need to install the `Data` module to use the `Data.FileSystem` module.

Your entitiy must implement the `IEntity<TKey>` interface, you need also to specify the type of the entity key:

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

If you have installed only an implementation of `Data.IRepository` you can directly use the `IRepository<T, TKey>` interface, the dependency injection engine will directly resolve the only implementation.

If you have multiple implementations installed you can force the one to use using the `Priority` key in the JSON config of the module, only the one with the highest priority (lowest number) will be resolved for the whole application.

If you want to use an implementation for a type of entity and another implementation for another type of entity you must manually override the `IRepository` registration for the entity in the `ConfigureServices` method of the `Startup` class:

```csharp
   public class Startup : Ws.Core.Startup<Ws.Core.AppConfig>
   {
       public override void ConfigureServices(WebApplicationBuilder builder)
       {
           base.ConfigureServices(builder);

           Ws.Core.AppInfo<Ws.Core.AppConfig>.Set(env: env, config: config, services: builder.Services);

           builder.Services.AddTransient(typeof(Ws.Core.Extensions.Data.IRepository<Product, int>), typeof(Ws.Core.Extensions.Data.Repository.FileSystem<Product, int>));

           builder.Services.AddTransient<TestApplication>();
       }
   }
```

### Usage example

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

⚠️Be aware that the `Query` method is not implemented so this module cannot completly replace the other implementations. Use only if you need direct access by Id.

## <a id="development"></a>Development

Development note, contribution info, licensing to be defined.

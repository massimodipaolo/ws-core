# Data.Cache

#### Table of Contents

1. [Description](#description)
   - [`ICache`](#description-icache)
     - [`Keys`](#description-icache-keys)
     - [`ExpirationTier`](#description-icache-expiration-tier)
     - [`Get`](#description-icache-get)
     - [`GetAsync`](#description-icache-get-async)
     - [`SetObjectAsync`](#description-icache-set-object-async)
     - [`Clear`](#description-icache-clear)
     - [`ClearAsync`](#description-icache-clear-async)
     - [`IDistributedCache`](#description-icache-idistributed)
   - [`ICache<T>`](#description-icachet)
   - [Additional documentation](#description-additional-docs)
1. [Setup](#setup)
   - [Requirements](#setup-requirements)
   - [Configuration](#setup-configuration)
1. [Usage](#usage)
1. [Limitations](#limitations)
1. [Development](#development)

## <a id="description"></a>Description

The `Data.Cache` module is the generic module for the distributed cache.

It exposes the `ICache` interface, with get/set of cache keys, using the `Microsoft.Extensions.Caching.Distributed` assembly.

It also contains a specific implementation of `ICache` that stores data in memory (`MemoryCache`).

### <a id="description-icache"></a>`ICache`

#### <a id="description-icache-keys"></a>`Keys`

Property that returns the list of all the keys stored in the cache.

```csharp
IEnumerable<string> Keys { get; }
```

#### <a id="description-icache-expiration-tier"></a>`ExpirationTier`

Property that returns the configuration object of the expiration tiers.

```csharp
IExpirationTier ExpirationTier { get; }
```

##### `IExpirationTier`

```csharp
public interface IExpirationTier
{
    DistributedCacheEntryOptions NoCache { get; }
    DistributedCacheEntryOptions Fast { get; }
    DistributedCacheEntryOptions Medium { get; }
    DistributedCacheEntryOptions Slow { get; }
    DistributedCacheEntryOptions Never { get; }
}
```

Refer to the Microsoft [documentation](https://docs.microsoft.com/en-us/dotnet/api/microsoft.extensions.caching.distributed.distributedcacheentryoptions) about `DistributedCacheEntryOptions`.

#### <a id="description-icache-get"></a>`Get`

Retrieves an object from the cache by key.

```csharp
T? Get<T>(string key);
```

#### <a id="description-icache-get-async"></a>`GetAsync`

Asynchronous version of the `Get` method.

```csharp
Task<T?> GetAsync<T>(string key);
```

#### <a id="description-icache-set-object-async"></a>`SetObjectAsync`

Asynchronous method that stores an object in the cache with a given key and an `ExpirationTier` configuration.

```csharp
Task SetObjectAsync(string key, object value, DistributedCacheEntryOptions options, CancellationToken token = default);
```

#### <a id="description-icache-clear"></a>`Clear`

Clears all the values stored in the cache.

```csharp
void Clear();
```

#### <a id="description-icache-clear-async"></a>`ClearAsync`

Asynchronous version of the `Clear` method.

```csharp
Task ClearAsync(CancellationToken token = default);
```

#### <a id="description-icache-idistributed"></a>`IDistributedCache`

`ICache` interface extends the `IDistributedCache` of the `Microsoft.Extensions.Caching.Distributed` assembly.

For the list of all the methods exposed by the `IDistributedCache` interface you can refer to the Microsoft [documentation](https://docs.microsoft.com/it-it/dotnet/api/microsoft.extensions.caching.distributed.idistributedcache).

### <a id="description-icachet"></a>`ICache<T>`

Generic `ICache` interface for an entity of type T.

### <a id="description-additional-docs"></a>Additional documentation

[Working with a distributed cache in ASP.NET Core](https://docs.microsoft.com/en-us/aspnet/core/performance/caching/distributed)

## <a id="setup"></a>Setup

You can install the `Data.Cache` module adding the reference in your `Core` application and configure it in the `ext-settings.json` file.

### <a id="setup-requirements"></a>Requirements

No other modules are required.

### <a id="setup-configuration"></a>Configuration

If you only need specific implementations of the cache module (i.e. `Data.Cache.Redis`) you don't need to install this module manually because it will be installed automatically as a dependency of the implementation module.

You need to install and add it to the ext-settings.json file only if you need the `MemoryCache` implementation with the following options:

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
      "Ws.Core.Extensions.Data.Cache": {
        "priority": 100,
        "options": {
          "entryExpirationInMinutes": {
            "fast": 10,
            "medium": 60,
            "slow": 240,
            "never": 1440
          }
        }
      }
    }
  }
}
```

## <a id="usage"></a>Usage

If you have installed only an implementation of `ICache` you can directly use the `ICache<T>` interface, the dependency injection engine will directly resolve the only implementation.

If you have multiple implementations installed you can force the one to use using the `Priority` key in the JSON config of the module, only the one with the highest priority (lowest number) will be resolved for the whole application.

If you want to use an implementation for a type of entity and another implementation for another type of entity you must manually override the `ICache` registration for the entity in the `Add` method of the `Startup` class:

```csharp
   public class Startup : Ws.Core.Startup<Ws.Core.AppConfig>
   {
       public override void Add(WebApplicationBuilder builder)
       {
           base.Add(builder);

           // override ICache<Product> implementation
           builder.Services.AddSingleton(typeof(Ws.Core.Extensions.Data.Cache.ICache<Product), typeof(Ws.Core.Extensions.Data.Cache.SqlServer.SqlCache<Product>));

           builder.Services.AddTransient<TestApplication>();
       }
   }
```

#### Usage example

```csharp
    public class TestApplication
    {
        ICache<Product> _productCache;

        public TestApplication(ICache<Product> productCache)
        {
            _productCache = productCache;
        }

        public async Task<IEnumerable<Product>> Run(IEnumerable<Product> products)
        {
            var key = "cache:product-list";

            await _productCache.ClearAsync();

            await _productCache.SetObjectAsync(key, products, _productCache.ExpirationTier.Medium);

            var cached = await _productCache.GetAsync<IEnumerable<Product>>(key);

            return cached;
        }
    }
```

## <a id="limitations"></a>Limitations

Nothing to report.

## <a id="development"></a>Development

Development note, contribution info, licensing to be defined.

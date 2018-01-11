# Data.Cache Extension

 This extension exposes the ICache interface, with get / set of cache keys, using the Microsoft.Extensions.Caching.Abstractions assembly.
 An ICacheRepository with CRUD operations is also provider.

## Extension configuration options

Options

    Type: {Memory|Distributed|Redis|SqlServer} 
    RedisOptions: Configurataion (server) and InstanceName of the Redis machine
    SqlOptions: ConnectionString,SchemaName,TableName. If omitted a local trusted connection on Cache.dbo.Entry table is provided.
    EntryExpirationInMinutes: override the default duration (in minutes) of the provided cache profiles. 

### Additional documentation
  In-memory caching in ASP.NET Core: https://docs.microsoft.com/en-us/aspnet/core/performance/caching/memory
  Working with a distributed cache in ASP.NET Core: https://docs.microsoft.com/en-us/aspnet/core/performance/caching/distributed

## Sample of usage

```csharp
public class SomeController: ControllerBase {
    
    private ICache _cache;

    public SomeController(ICache cache) {
        _cache = cache;
    }

    [HttpGet]
    public IActionResult Get() {
        string key = "api:somedata";
        var result = _cache.Get<SomeData>(key);
        if (result == null) {
            result = SomeData.GetData();
            _cache.Set(key, result, new CacheEntryOptions() { AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(1)});
            //_cache.Set(key, result, CacheEntryOptions.Expiration.Fast); //use a default profile
        }
        return Ok(result);
    }
}
```
###  Using ICacheRepository in Api.EntityCache

```csharp
public class SomeEntityCacheController : EntityCachedController<SomeEntity> {
    public SomeEntityCacheController(IRepository<SomeEntity> repository, ICacheRepository<SomeEntity> cachedRepository) : base(repository, cachedRepository) { }
}
```

## Sample configuration

```json
      "app.core.Extensions.Data.Cache": {
        "priority": 3,
        "options": {
          "type": "Redis",
          "redisOptions": {
            "configuration": "127.0.0.1:6379",
            "instanceName": "master"
          },
          "sqlOptions": {
            "connectionString": "Server=.;Database=Cache;Trusted_Connection=True;",
            "schemaName": "dbo",
            "tableName": "Entry"
          },
          "entryExpirationInMinutes": {
            "fast": 10,
            "medium": 60,
            "slow": 240,
            "never": 1440
          }
        }
      }
```

## SqlServer initialization

  Use the sql-cache tool, addind SqlConfig.Tools to your project file: https://docs.microsoft.com/en-us/aspnet/core/performance/caching/distributed#using-a-sql-server-distributed-cache  

### T-sql script sample
```sql
use master
go
create login cacheUser with password='C4$hUs3r-Strong!Pa$$w0rd'
go
create database Cache
go
use Cache
go
create user cacheUser for login cacheUser
go
create table dbo.Entry(
    Id nvarchar(449) primary key clustered,
    Value varbinary(max),
    ExpiresAtTime datetimeoffset(7),
    SlidingExpirationInSeconds bigint,
    AbsoluteExpiration datetimeoffset(7)
    )
go
grant select on dbo.Entry to cacheUser; 
grant insert on dbo.Entry to cacheUser; 
grant update on dbo.Entry to cacheUser; 
grant delete on dbo.Entry to cacheUser; 
go
```

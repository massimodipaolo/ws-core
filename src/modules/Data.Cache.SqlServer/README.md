# Data.Cache Extension

 This extension exposes the ICache interface, with get / set of cache keys, using the Microsoft.Extensions.Caching.Abstractions assembly.
 An ICacheRepository with CRUD operations is also provider.

## Extension configuration options

Options

    Client: ConnectionString,SchemaName,TableName. If omitted a local trusted connection on Cache.dbo.Entry table is provided.
    EntryExpirationInMinutes: override the default duration (in minutes) of the provided cache profiles. 

## SqlServer initialization

  Use the sql-cache tool, addind SqlConfig.Tools to your project file: https://docs.microsoft.com/en-us/aspnet/core/performance/caching/distributed#using-a-sql-server-distributed-cache  

### T-sql script sample
```sql
use master
go
create database Cache containment = partial
go
use Cache
go
create user cacheUser with password='C4$hUs3r-Strong!Pa$$w0rd'
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

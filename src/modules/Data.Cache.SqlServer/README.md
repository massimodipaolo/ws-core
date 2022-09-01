# Data.Cache.SqlServer

#### Table of Contents

1. [Description](#description)
1. [Setup](#setup)
   - [Requirements](#setup-requirements)
   - [Configuration](#setup-configuration)
1. [Usage](#usage)
1. [Limitations](#limitations)
1. [Development](#development)

## <a id="description"></a>Description

The `Data.Cache.SqlServer` module installs and configure the implementation of the `Data.Cache.ICache` interface of the generic `Data.Cache` for `SqlServer`.

## <a id="setup"></a>Setup

You can install the `Data.Cache.SqlServer` module adding the reference in your `Core` application and configure it in the `ext-settings.json` file.

### <a id="setup-requirements"></a>Requirements

No other modules are required.

### <a id="setup-configuration"></a>Configuration

You need to define 2 options to configure the module:

1. **client** (_optional_): `SqlServer` client options, you can refer to the Microsoft [documentation](https://docs.microsoft.com/en-us/dotnet/api/microsoft.extensions.caching.sqlserver.sqlservercacheoptions). If omitted a local trusted connection on `Cache.dbo.Entry` table is provided.

1. **entryExpirationInMinutes** (_optional_): overrides the default duration (in minutes) of the provided cache profiles. If omitted default values will be used.
   - **fast** (_default_: `10`)
   - **medium** (_default_: `60`)
   - **slow** (_default_: `240`)
   - **never** (_default_: `1440`)

#### Configuration example

```json
{
  "extConfig": {
    "assemblies": {
      "Ws.Core.Extensions.Data.Cache.SqlServer": {
        "priority": 100,
        "options": {
          "client": {
            "connectionString": "Server=.,14332;Database=Cache;User Id=cacheUser;Password=cachePwd;MultipleActiveResultSets=true",
            "schemaName": "dbo",
            "tableName": "Entry"
          },
          "entryExpirationInMinutes": {
            "fast": 2,
            "medium": 10,
            "slow": 120,
            "never": 2880
          }
        }
      }
    }
  }
}
```

## <a id="usage"></a>Usage

See documentation of the `data.cache` module [usage](../Data.Cache/README.md#usage).

### SqlServer initialization

Use the `sql-cache` tool, adding `SqlConfig.Tools` to your project file: https://docs.microsoft.com/en-us/aspnet/core/performance/caching/distributed#using-a-sql-server-distributed-cache

### T-sql script sample

```sql
use master
go
create database Cache containment = partial
go
use Cache
go
create user cacheUser with password='cachePwd'
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

## <a id="limitations"></a>Limitations

Nothing to report.

## <a id="development"></a>Development

Development note, contribution info, licensing to be defined.

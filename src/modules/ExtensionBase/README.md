# ExtensionBase

#### Table of Contents

1. [Description](#description)
1. [Setup](#setup)
   - [Requirements](#setup-requirements)
   - [Configuration](#setup-configuration)
     - [SectionRoot](#setup-configuration-root)
       - [Assembly](#setup-configuration-root-assembly)
       - [Injector](#setup-configuration-root-injector)
1. [Usage](#usage)
   - [Create an extension](#usage-extension)
   - [Create a module](#usage-module)
   - [Create a middleware](#usage-middleware)
   - [Create a decorator](#usage-decorator)
1. [Limitations](#limitations)
1. [Development](#development)

## <a id="description"></a>Description

The `ExtensionBase` module exposes:

- `Extension` base class used to implement modules and extensions.
- `IOptions` interface used to implement module options definition.
- `Configuration` class that defines the structure of the `extConfig` section of the `ext-settings.json`.

## <a id="setup"></a>Setup

You need to install this module manually only if your are creating your own custom module library class project, otherwise it comes in your application with the `core` package as a dependency.

### <a id="setup-requirements"></a>Requirements

No other modules are required.

### <a id="setup-configuration"></a>Configuration

You don't have to specifically configure the `ExtensionBase` module, it defines how to configure the `ext-settings.json` for the application.

> **Note**: You can specialize the `ext-settings.json` for different enviroments creating new files following this pattern `ext-settings.{ENV}.json` (i.e. `ext-settings.Development.json`, `ext-settings.Local.json`).

#### <a id="setup-configuration-root"></a>SectionRoot (_default_: `extConfig`)

- **folder** (_default_: `Extensions`): folder that contains the application extensions.
- **enableShutDownOnChange** (_default_: `false`): if true, application will automatically reload after a change in the `ext-settings.json`.
- **assemblies**: list of [assembly](#setup-configuration-root-assembly) configurations
- **injectors**: list of [injector](#setup-configuration-root-injector) configurations

##### <a id="setup-configuration-root-assembly"></a>Assembly

- **name**: namespace of the assembly (i.e. `Ws.Core.Extensions.Message`).
- **priority**: number that indicates the priority of injection in the pipeline.
- **options**: options defined by the `json-schema.json` of the assembly.

##### <a id="setup-configuration-root-injector"></a>Injector

- **name** (_optional_): name that defines the injector (i.e. `message-service-override`).
- **priority**: number that indicates the priority of injection in the pipeline.
- **services**: list of service registration options to inject.
  - **serviceType**: namespace of the service type (i.e. `Ws.Core.Extensions.Message.IMessage`).
  - **implementationType**: namespace of the implementation type (i.e. `Ws.Core.Extensions.MyMessageService`).
  - **lifetime** (_default_: `Transient`): declare the lifetime of the service in the dependency injection engine defined by [ServiceLifetime](http://docs.microsoft.com/dotnet/api/microsoft.extensions.dependencyinjection.servicelifetime).
  - **overrideIfAlreadyRegistered** (_default_: `true`): if enabled overrides previous service registration.
- **middlewares**: list of middleware to inject:
  - **type**: namespace of the middleware delegate class (i.e. `testApp.Middlewares.MyCustomMiddleware`).
  - **map** (_optional_): branches the request pipeline based on matches of the given request path.
    - **pathMatch**: the request path to match (starts with) (i.e. `/branch`).
    - **preserveMatchedPathSegment** (_default_: `true`): if false, matched path would be removed from `Request.Path` and added to `Request.PathBase`.
- **decorators**: list of decorators to inject:
  - **serviceType**: namespace of the service type (i.e. `Ws.Core.Extensions.Message.IMessage`).
  - **implementationType**: namespace of the implementation type (i.e. `testApp.Decorators.IMyMessageDecorator`).

## <a id="usage"></a>Usage

Using the `Extension` base class you can create your own **modules** or **extensions**.

The main difference between a module and an extension is that a module is a stand-alone class library with a configuration option class integrated in the `ext-settings.json` while the extension is defined inside your application without the `IOption` implementation.

For both approaches you always need to implement the execute methods:

```csharp
    //IConfigureBuilder, used in "ConfigureServices" method of the app "Startup" class
    public virtual void Execute(WebApplicationBuilder builder, IServiceProvider serviceProvider = null)
    {
    }
```

```csharp
    //IConfigureApp, used in "Use" method of the app "Startup" class
    public virtual void Execute(WebApplication app)
    {
    }
```

### <a id="usage-extension"></a>Create an extension

In order to create an application extension you need to implement the `Extension` class defining:

- **Name**: the name of the extension.
- **Priority**: the priority of injection in the middleware pipeline. Be careful to avoid conflicts with the priorities already defined in the `ext-settings.json`.

#### Hangfire extension example

In this example we are creating an extension that adds `Hangfire` to our application (server, healtchecks, dashboard) using the memory storage. All configurations parameters are hard-coded in the extension implementation.

```csharp
    using Hangfire;
    using Hangfire.MemoryStorage;

    public class Hangfire : Ws.Core.Extensions.Base.Extension
    {
        public override string Name => typeof(Hangfire).Name;

        public override int Priority => 620;

        public override void Execute(WebApplicationBuilder builder, IServiceProvider? serviceProvider = null)
        {
            builder.Services
                .AddHangfire(_ => _.UseMemoryStorage())
                .AddHangfireServer(_ =>
                {
                    _.HeartbeatInterval = new System.TimeSpan(0, 1, 0);
                    _.ServerCheckInterval = new System.TimeSpan(0, 1, 0);
                    _.SchedulePollingInterval = new System.TimeSpan(0, 1, 0);
                    _.Queues = new string[] { "default" };
                });

            builder.Services.AddHealthChecks().AddHangfire(_ =>
                {
                    _.MinimumAvailableServers = 1;
                    _.MaximumJobsFailed = 2;
                },
                tags: new[] { "tool", "cron" },
                failureStatus: Microsoft.Extensions.Diagnostics.HealthChecks.HealthStatus.Degraded);
        }

        public override void Execute(WebApplication app)
        {
            app.UseHangfireDashboard("/hangfire", options: new DashboardOptions()
            {
                StatsPollingInterval = 10 /*seconds*/ * 1000,
                DisplayStorageConnectionString = true
            });
        }
    }
```

### <a id="usage-module"></a>Create a module

In order to create a module you need to:

1. Implement the `IOptions` interface declaring the options needed to configure the module in the `ext-settings.json` file. As a convention both file and class are called `Options`.

2. Implement the `Extension` base class declaring the `Options` implemented for the module and implementing the execute methods. As a convention both file and class are called `Extension`.

3. Run the `ws.core.cli` command `GenerateJsonSchema` passing your custom module folder as a parameter to generate the `json-schema.json` files that defines your custom modules configuration.

```bash
    ws.core.cli.exe -- -p "C:\\testApp\\modules"
```

4. Create your own main models `json-schema.json` file (the one used in the `ext-settings.json`), referencing the original ws-core schema and adding the definition of the new custom modules.

```json
{
  "allOf": [
    {
      "$ref": "https://raw.githubusercontent.com/massimodipaolo/ws-core/master/src/modules/json-schema.json"
    }
  ],
  "properties": {
    "extConfig": {
      "properties": {
        "assemblies": {
          "properties": {
            "Ws.Core.Extensions.Hangfire": {
              "allOf": [{ "$ref": "#/definitions/assembly" }],
              "properties": {
                // Json schema of the custom Hangfire module generate by ws.core.cli
                "options": { "$ref": "Hangfire/json-schema.json#" }
              }
            }
          }
        }
      }
    }
  }
}
```

5. Update your `ext-settings.json` referencing the new main modules `json-schema.json` instead of the original ws-core schema. By doing so you will continue using the intellisense for the core modules along with your custom modules.

```json
{
  // custom Json schema
  "$schema": "/custom/modules/json-schema.json",
  //"$schema": "https://raw.githubusercontent.com/massimodipaolo/ws-core/master/src/modules/json-schema.json",
  "extConfig": {
    "folder": "Extensions",
    "enableShutDownOnChange": false,
    "assemblies": {
       ....
    }
  }
}
```

#### Hangfire module example

In this example we are creating an extension that adds `Hangfire` to our application (server, healtchecks, dashboard) using the memory storage. All configurations parameters are defined by the `IOptions` and integrated in the `ext-settings.json` file of the main application.

```csharp
using System.ComponentModel;
using Ws.Core.Extensions.Base;

namespace Ws.Core.Extensions.Hangfire;

public class Options: IOptions
{
    public HangfireServerOptions Server { get; set; } = new HangfireServerOptions();
    public HangfireHealthChecksOptions HealthChecks { get; set; } = new HangfireHealthChecksOptions();
    public HangfireDashboardOptions Dashboard { get; set; } = new HangfireDashboardOptions();

    public class HangfireServerOptions
    {
        [DefaultValue(1)]
        public int HeartbeatIntervalInMinutes { get; set; } = 1;

        [DefaultValue(1)]
        public int ServerCheckIntervalInMinutes { get; set; } = 1;

        [DefaultValue(1)]
        public int SchedulePollingIntervalInMinutes { get; set; } = 1;

        [Description("You should specify at least one queue to listen. If not provided a \"default\" queue will be used")]
        public string[]? Queues { get; set; }
    }

    public class HangfireHealthChecksOptions
    {
        [DefaultValue(1)]
        public int MinimumAvailableServers { get; set; } = 1;

        [DefaultValue(2)]
        public int MaximumJobsFailed { get; set; } = 2;

        [Description("A list of tags that can be used to filter sets of health checks. Optional.")]
        public string[]? Tags { get; set; }

        [DefaultValue(Microsoft.Extensions.Diagnostics.HealthChecks.HealthStatus.Degraded)]
        public Microsoft.Extensions.Diagnostics.HealthChecks.HealthStatus FailureStatus { get; set; } = Microsoft.Extensions.Diagnostics.HealthChecks.HealthStatus.Degraded;
    }

    public class HangfireDashboardOptions
    {
        [DefaultValue("/hangfire")]
        public string Path { get; set; } = "/hangfire";

        [Description("The interval the /stats endpoint should be polled with")]
        [DefaultValue(10)]
        public int StatsPollingIntervalInSeconds { get; set; } = 10;

        [DefaultValue(false)]
        public bool DisplayStorageConnectionString { get; set; } = false;
    }
}
```

```csharp
using Hangfire;
using Hangfire.MemoryStorage;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace Ws.Core.Extensions.Hangfire;

public class Extension: Ws.Core.Extensions.Base.Extension
{
    private Options _options => GetOptions<Options>() ?? new Options();

    public override void Execute(WebApplicationBuilder builder, IServiceProvider? serviceProvider = null)
    {
        builder.Services
            .AddHangfire(_ => _.UseMemoryStorage())
            .AddHangfireServer(_ =>
            {
                _.HeartbeatInterval = new System.TimeSpan(0, _options.Server.HeartbeatIntervalInMinutes, 0);
                _.ServerCheckInterval = new System.TimeSpan(0, _options.Server.ServerCheckIntervalInMinutes, 0);
                _.SchedulePollingInterval = new System.TimeSpan(0, _options.Server.SchedulePollingIntervalInMinutes, 0);
                _.Queues = _options.Server.Queues ?? new string[] {"default"};
            });

        builder.Services.AddHealthChecks().AddHangfire(_ =>
        {
            _.MinimumAvailableServers = _options.HealthChecks.MinimumAvailableServers;
            _.MaximumJobsFailed = _options.HealthChecks.MaximumJobsFailed;
        },
        tags: _options.HealthChecks.Tags,
        failureStatus: _options.HealthChecks.FailureStatus);
    }

    public override void Execute(WebApplication app)
    {
        app.UseHangfireDashboard(_options.Dashboard.Path, options: new DashboardOptions()
            {
                StatsPollingInterval = _options.Dashboard.StatsPollingIntervalInSeconds * 1000,
                DisplayStorageConnectionString = _options.Dashboard.DisplayStorageConnectionString
            });
    }
}
```

### <a id="usage-middleware"></a>Create a middleware

In order to create a middleware you need to:

- create a middleware delegate class. You don't have to inherit from the `Extension` base class.
- configure the middleware injection in the `ext-settings.json` file.

A middleware delegate class must include:

- A public constructor with a parameter of type [RequestDelegate](https://docs.microsoft.com/it-it/dotnet/api/microsoft.aspnetcore.http.requestdelegate).
- A public method named `Invoke` or `InvokeAsync` that return a [Task](https://docs.microsoft.com/it-it/dotnet/api/system.threading.tasks.task) and accept a first parameter of type [HttpContext](https://docs.microsoft.com/it-it/dotnet/api/microsoft.aspnetcore.http.HttpContext).
- Additional parameters for the constructor and `Invoke`/`InvokeAsync` are populated by dependency injection.

> **Note**: Remember to return the `RequestDelegate` call in the `Invoke`/`InvokeAsync` to pass control to the next delegate/middleware in the pipeline.

#### RequestCulture middleware example

In this example we are creating a middleware that gets the `culture` from the request query and use it to:

- Update the `CurrentCulture` and the `CurrentUICulture` properties.
- Add the `X-culture` header to the response.
- Write a log info for the middleware invocation.

```csharp
namespace testApp.Middlewares;

public class RequestCulture
{
    private readonly RequestDelegate _next;
    private readonly ILogger _logger;

    public RequestCulture(RequestDelegate next, ILogger<RequestCulture> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var cultureQuery = context.Request.Query["culture"];
        if (!string.IsNullOrWhiteSpace(cultureQuery))
        {
            var culture = new CultureInfo(cultureQuery);
            CultureInfo.CurrentCulture = culture;
            CultureInfo.CurrentUICulture = culture;
            context.Response.Headers.Add(new KeyValuePair<string, Microsoft.Extensions.Primitives.StringValues>("X-culture", cultureQuery));
            _logger.LogInformation("Invoked {middleware} middleware with [{cultureQuery}]", nameof(RequestCulture), cultureQuery);
        }
        // Call the next delegate/middleware in the pipeline.
        await _next(context);
    }
}
```

To invoke the middleware for every request:

```json
  "extConfig": {
    "folder": "Extensions",
    "enableShutDownOnChange": false,
    "assemblies": [...],
    "injectors": [
      {
        "priority": 301,
        "name": "request-culture",
        "middlewares": [
          {
            "type": "testApp.Middlewares.RequestCulture"
          }
        ]
      }
    ]
```

To invoke the middleware only for the paths starting with `/branch`:

```json
  "extConfig": {
    "folder": "Extensions",
    "enableShutDownOnChange": false,
    "assemblies": [...],
    "injectors": [
      {
        "priority": 301,
        "name": "branch-request-culture",
        "middlewares": [
          {
            "type": "testApp.Middlewares.RequestCulture",
            "map": {
              "pathMatch": "/branch",
              "preserveMatchedPathSegment": true
            }
          }
        ]
      }
    ]
```

### <a id="usage-decorator"></a>Create a decorator

You can create a decorator class using the decoration pattern and configure it in the `ext-settings.json` file. You don't have to inherit from the `Extension` base class.

#### MessageLogger example

In this example we are decorating the `IMessage` registered implementation with a logger that writes a log info before calling the inner instance.

```csharp
public class MessageLogger: IMessage
{
    private readonly Ws.Core.Extensions.Message.IMessage _inner;
    private readonly ILogger<Ws.Core.Extensions.Message.IMessage> _logger;

    public MessageLogger(Ws.Core.Extensions.Message.IMessage inner, ILogger<Ws.Core.Extensions.Message.IMessage> logger)
    {
        _inner = inner;
        _logger = logger;
    }

    public async Task SendAsync(Ws.Core.Extensions.Message.Message message, bool throwException = false)
    {
        _logger.LogInformation(
            "Sending message: {subject} to {recipient}",
            message.Subject,
            message.Recipients.FirstOrDefault(_ => _.Type == Ws.Core.Extensions.Message.Message.ActorType.Primary)
            );
        await _inner.SendAsync(message,throwException);
    }

    public Task<IEnumerable<Ws.Core.Extensions.Message.Message>> ReceiveAsync()
    => _inner.ReceiveAsync();

    public Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
    => _inner.CheckHealthAsync(context, cancellationToken);
}
```

```json
  "extConfig": {
    "folder": "Extensions",
    "enableShutDownOnChange": false,
    "assemblies": [...],
    "injectors": [
      {
        "priority": 601,
        "name": "IMessage-with-logger",
        "decorators": [
          {
            "serviceType": "Ws.Core.Extensions.Message.IMessage",
            "implementationType": "testApp.Decorators.MessageLogger"
          }
        ]
      }
    ]
```

## <a id="limitations"></a>Limitations

Nothing to report.

## <a id="development"></a>Development

Development note, contribution info, licensing to be defined.

<a href="https://gitmoji.dev">
  <img src="https://img.shields.io/badge/gitmoji-%20😜%20😍-FFDD67.svg?style=flat-square" alt="Gitmoji">
</a>

# ws-core

@websolutespa backend modules.

## Build Status

[![Build status](https://ci.appveyor.com/api/projects/status/p3tr2g86kaby4swk?svg=true)](https://ci.appveyor.com/project/massimodipaolo/ws-core)

## Getting Started

### 1. Create a dotnet core web empty project

#### .NET CLI

```bash
dotnet new web -n testApp
```

#### Visual Studio

You can also create it in **Visual Studio** selecting the `ASP.NET Core Empty` template.

### 2. Install the `Ws.Core` package

#### .NET CLI

```bash
cd testApp
dotnet add package Ws.Core
```

#### Visual Studio

You can install the `Ws.Core` package using the [NuGet Package Manager](https://docs.microsoft.com/en-us/nuget/consume-packages/install-use-packages-visual-studio) in Visual Studio.

#### Using the source code

You can also add the project reference in your solution to work with the source code you downloaded.

##### .NET CLI

```bash
cd testApp
dotnet dotnet add reference [<WS_CORE_PROJECT_PATH>]
```

##### Visual Studio

You can add a reference to the `Ws.Core` source code you downloaded using the Visual Studio [IDE](https://docs.microsoft.com/en-us/visualstudio/ide/managing-references-in-a-project).

### 3. Configure the `Ws.Core` application

The easiest way to create a `Ws.Core` application is to replace everything inside the `Program.cs` with just the `CreateRunBuilder` method call.

##### `Program.cs`

```csharp
Ws.Core.Program.CreateRunBuilder<Ws.Core.AppConfig>(args);
```

This method will use the default `Startup` class to register all the installed `Ws.Core.Extension` services in the container, add all the middleware components in the pipeline and run the application using [NLog](https://github.com/nlog/nlog/) integration as default.

If you need to add custom configurations check the following examples.

#### Using custom setup actions example

In this example we are passing custom parameters to the `CreateRunBuilder` method.

##### `Program.cs`

```csharp
Ws.Core.Program.CreateRunBuilder<Ws.Core.AppConfig>(
    testApp.Program.ParseArgs(args),
    testApp.Program.setupBuilder,
    testApp.Program.setupApplication
);

namespace testApp
{
    public partial class Program : WebApplicationFactory<Program>
    {
        internal static Action<WebApplicationBuilder> setupBuilder = (builder) => Add(builder);
        internal static Action<WebApplication> setupApplication = (app) => { Use(app); ConfigureServer(app); };

        internal static void Add(WebApplicationBuilder builder)
        {
            // Add services to the container.
        }

        internal static void Use(WebApplication app)
        {
            // Add middleware components to the pipeline
        }

        internal static string[] ParseArgs(string[] args)
        {
            // Custom args parsing

            return args;
        }

        internal static void ConfigureServer(WebApplication app)
        {
            // Configure host/others features
        }
    }
}

```

#### Using custom `Startup` class example

In this example we are using the `CreateBuilder` method and a custom `Startup` class.

##### `Program.cs`

```csharp
var builder = Ws.Core.Program.CreateBuilder(args);

// Add logging
builder.Logging.AddConsole();

// Use your Startup class
var startup = new testApp.Startup(builder);

startup.Add(builder);

var app = builder.Build();

startup.Use(app);

app.Run();
```

##### `Startup.cs`

```csharp
namespace testApp
{
    public class Startup : Ws.Core.Startup<Ws.Core.AppConfig>
    {
        public Startup(WebApplicationBuilder builder) : base(builder) { }

        public override void Add(WebApplicationBuilder builder)
        {
            base.Add(builder);

            // Add services to the container.
        }

        public override void Use(WebApplication app)
        {
            base.Use(app);

            // Add middleware components to the pipeline
        }
    }
}
```

### 4. Start working with modules and extensions

Now that your `Ws.Core` application is up and running you can start developing your features by installing one or more available modules from the [ws-core module list](/src/modules/README.md) or creating your own modules or extensions following the [ExtensionBase](/src/modules/ExtensionBase/README.md#usage) module **usage** guide section.

#### Extension centralized configuration

Using the `Ws.Core` extension ecosystem you can install and configure modules and extension directly from a centralized json file called `ext-settings.json`.

Create the empty file in your project root folder using the following model and see the [ExtensionBase](/src/modules/ExtensionBase/README.md#setup-configuration) module **configuration** section to learn how to write your custom configuration.

##### `ext-settings.json`

```json
{
  "$schema": "https://raw.githubusercontent.com/massimodipaolo/ws-core/master/src/modules/json-schema.json",
  "extConfig": {
    "folder": "Extensions",
    "enableShutDownOnChange": false,
    "assemblies": {},
    "injectors": []
  }
}
```


# core

Base host


## Inherits Program

Builder

```csharp
    var builder = Ws.Core.Program.CreateBuilder(args);
    // Add logging
    builder.Logging.AddConsole();
    // Use your Startup class
    var startup = new x.core.Startup(builder);
    startup.Add(builder);
    var app = builder.Build();
    // Configure host/others features
    x.core.Program.ConfigureServer(app);
    startup.Use(app);
    app.Run();
```

Build & Run (default Startup + NLog integration)

```csharp
    Ws.Core.Program.CreateRunBuilder<Ws.Core.AppConfig>(args);
```

Build & Run with custom Action setup

```csharp
    Ws.Core.Program.CreateRunBuilder<Ws.Core.AppConfig>(
        args,
        (builder) => x.core.Program.Add(builder), // Action<WebApplicationBuilder>
        x.core.Program.setupApplication // Action<WebApplication>
    );
```
    
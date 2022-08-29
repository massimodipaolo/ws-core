# Api

#### Table of Contents

1. [Description](#description)
1. [Setup](#setup)
   - [Requirements](#setup-requirements)
   - [Configuration](#setup-configuration)
1. [Usage](#usage)
1. [Limitations](#limitations)
1. [Development](#development)

## <a id="description"></a>Description

The `Api` module installs and configure the Swagger discovery of Api endpoints, automatically adds Api controllers to the application and exposes documentation via Swagger UI using OpenAPI Specification (OAS).

## <a id="setup"></a>Setup

You can install the `Api` module adding the reference in your `Core` application and configure it in the `ext-settings.json` file.

### <a id="setup-requirements"></a>Requirements

No other modules are required.

### <a id="setup-configuration"></a>Configuration

You need to define 3 options to configure the module:

1. **serialization** (_optional_): configuration options of the `Shared.Serialization` module to customize the default `JsonOption` configuration used in the web service.
1. **session** (_optional_): Adds the `Microsoft.AspNetCore.Session.SessionMiddleware` to automatically enable session state for the application.

   - **cookie**:
     - **name** (_default_: `.api.Session`): the name of the session cookie.
     - **httpOnly**: indicates whether a cookie is accessible by client-side script.
   - **idleTimeoutInMinutes**: indicates how long the session can be idle before its contents are abandoned. Each session access resets the timeout. Note this only applies to the content of the session, not the cookie.

1. **documentation** (_optional_): documentation builder configuration.
   - **routePrefix** (_default_: `swagger`): documentation url prefix.
   - **ui**: Swagger UI configuration.
     - **injectJs**: relative path of additional js file, added in `wwwroot` folder; i.e. `/swagger-ui/custom.js`
     - **injectCss**: relative path of additional css file, added in `wwwroot` folder; i.e. `/swagger-ui/custom.css`
   - **endpoints** (_optional_): list of endpoints for different definitions (i.e. versions). In Swagger UI you will be able to switch between definitions using the selector.
     - **id** (_default_: `v{index}`): endpoint identifier.
     - **title** (_default_: `api v{index}`): endpoint title.
     - **version** (_default_: `{Id}`): endpoint version.
   - **xmlComments**: to include Xml Comments, open the Properties dialog for your project, click the `Build` tab and ensure that `XML documentation file` is checked. This will produce a file containing all XML comments at build-time. At this point, any classes or methods that are NOT annotated with XML comments will trigger a build warning. To suppress this, enter the warning code 1591 into the `Suppress warnings` field in the properties dialog.
     - **fileName** (_default_: `System.Reflection.Assembly.GetExecutingAssembly()?.GetName()?.Name`): path to the file that contains XML Comments
     - **includeControllerComments** (_default_: `false`): flag to indicate if controller XML comments (i.e. summary) should be used to assign Tag descriptions.
   - **securityDefinitions**: add one or more security definitions, describing how your api is protected. The `Authorize` feature will be available in Swagger UI based on the following configuration.
     - **bearer** (_default_: `false`): add Authorization header for bearer token.
     - **cookies**: add Authorization via cookies based on the given cookie name list.

#### Configuration example

```json
{
  "extConfig": {
    "assemblies": {
      "Ws.Core.Extensions.Api": {
        "priority": 100,
        "options": {
          "serialization": {
            "nullValueHandling": "Ignore",
            "formatting": "Indented",
            "referenceLoopHandling": "Ignore",
            "converters": [
              {
                "assembly": "Ws.Core.Extensions.Api",
                "type": "Ws.Core.Shared.Serialization.ExceptionConverter"
              }
            ],
            "__converters": [
              {
                "assembly": "Newtonsoft.Json",
                "type": "Newtonsoft.Json.Converters.StringEnumConverter"
              },
              {
                "assembly": "TestApp",
                "type": "TestApp.LocaleJsonConverter"
              }
            ]
          },
          "session": {
            "cookie": {
              "name": ".api.Session",
              "httpOnly": true
            },
            "idleTimeoutInMinutes": 20
          },
          "documentation": {
            "routePrefix": "swagger",
            "securityDefinitions": {
              "bearer": true,
              "cookies": [".auth.api.Cookie"]
            },
            "ui": {
              "injectCss": "/swagger-ui/custom.css",
              "injectJs": "/swagger-ui/custom.js"
            },
            "endpoints": [
              {
                "id": "public",
                "title": "public api",
                "version": "v1"
              },
              {
                "id": "admin"
              }
            ],
            "xmlComments": {
              "fileName": "TestApp.xml",
              "includeControllerComments": true
            }
          }
        }
      }
    }
  }
}
```

## <a id="usage"></a>Usage

You can defines Api endpoints in 3 ways:

### Create Api controllers in your application

All Api controllers defined in your application will be added automatically to the web service.

```csharp
namespace TestApp.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ValuesController : ControllerBase
    {
        IRepository<Value, int> _valuesRepo;

        public ValuesController(IRepository<Value, int> repo)
        {
            _valuesRepo = repo;
        }

        // GET: api/<ValuesController>
        [HttpGet]
        public IActionResult Get()
        {
            var values = _valuesRepo.List.ToList();

            return new JsonResult(values);
        }
    }
}
```

### Create Minimal Api endpoints

#### Directly in the `program` of your application

```csharp
---
startup.Use(app);

// Value minimal Api
app.MapGet($"/api/{nameof(Value)}", (IRepository<Value, int> repo) => repo.List).WithTags(nameof(Value));

app.Run();
---
```

#### Implementing the `ICarterModule` interface

All `ICarterModule` implementations defined in your application will be added automatically to the web service.

```csharp
public class ValueModule : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        var _prefix = $"/api/{nameof(Value)}";
        app.MapGet($"{_prefix}", (IRepository<Value, int> repo) => repo.List).WithTags(nameof(Value));
    }
}
```

See documentation of `Carter` [here](https://github.com/CarterCommunity/Carter).

### Swagger UI

See documentation of `Swagger UI` [here](https://swagger.io/tools/swagger-ui).

## <a id="limitations"></a>Limitations

Nothing to report.

## <a id="development"></a>Development

Development note, contribution info, licensing to be defined.

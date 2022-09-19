# StaticFiles

#### Table of Contents

1. [Description](#description)
   - [Additional documentation](#description-additional-docs)
1. [Setup](#setup)
   - [Requirements](#setup-requirements)
   - [Configuration](#setup-configuration)
1. [Usage](#usage)
1. [Limitations](#limitations)
1. [Development](#development)

## <a id="description"></a>Description

The `StaticFiles` module enables the web application to serve static files using a physical file provider.

### <a id="description-additional-docs"></a>Additional documentation

[Introduction to working with static files in ASP.NET Core](https://docs.microsoft.com/en-us/aspnet/core/fundamentals/static-files)

## <a id="setup"></a>Setup

You can install the `StaticFiles` module adding the reference in your `Core` application and configure it in the `ext-settings.json` file.

### <a id="setup-requirements"></a>Requirements

No other modules are required.

### <a id="setup-configuration"></a>Configuration

You need to define the list of static folders to serve using the `paths` option:

1. **paths**:
   - **path**: relative or UNC path. The relative path is starting from `ContentRootPath`. Leave it empty for the `WebRootPath`.
   - **requestPath**: virtual path (i.e. `/downloads`).
   - **isRelativePath**: tells the module that the `path` option indicates a relative path. If not provided the module tries automatically to infer if it's a relative path.
   - **headers** (_optional_): dictionary of key/value response header (i.e. `"Cache-Control": "public,max-age=43200"`).
   - **mimEtypes** (_optional_): dictionary of key/value extension/content-type (i.e. `".myapp": "applicationx-msdownload"`).
   - **defaultFiles** (_optional_): list of default static files, (i.e. `index.html`).
   - **enableDirectoryBrowser** (_default_: `false`): enables file browsing.

#### Configuration example

```json
{
  "extConfig": {
    "assemblies": {
      "Ws.Core.Extensions.StaticFiles": {
        "priority": 100,
        "options": {
          "paths": [
            {
              "headers": {
                "Cache-Control": "public,max-age=43200"
              },
              "defaultFiles": ["index.html"]
            },
            {
              "path": "wwwroot/public/files",
              "requestPath": "/download",
              "headers": {
                "Cache-Control": "public,max-age=86400"
              },
              "mimeTypes": {
                ".myapp": "application/x-msdownload",
                ".htm3": "text/html"
              },
              "enableDirectoryBrowser": true
            }
          ]
        }
      }
    }
  }
}
```

## <a id="usage"></a>Usage

Once configured no other code is required.

## <a id="limitations"></a>Limitations

⚠️Be aware that this middleware must be inserted before any authentication or routing directives.

## <a id="development"></a>Development

Development note, contribution info, licensing to be defined.

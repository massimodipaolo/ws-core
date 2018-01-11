StaticFiles Extensions
======================

This extension enables the web app to serve static files. A physical
file provider is used.

Extension configuration options
-------------------------------

Options: Array of static file folders settings

Settings

    Path: file path starting from `ContentRootPath`. Leave it empty for the `WebRootPath`
    RequestPath: the relative request path that maps to static resources
    Headers: dictionary of key/value header
    DefaultFiles: array of file
    MIMEtypes: dictionary of key/value extention/content-type
    EnableDirectoryBrowser: true|false (default)

**Additional documentation** Introduction to working with static files
in ASP.NET Core:
https://docs.microsoft.com/en-us/aspnet/core/fundamentals/static-files

Sample configuration
--------------------

      "app.core.Extensions.StaticFiles": {
        "priority": 1,
        "options": {
          "paths": [
            {
              "headers": {
                "Cache-Control": "public,max-age=43200"
              }
            },
            {
              "path": "wwwroot/folder",
              "requestPath": "/download",
              "headers": {
                "Cache-Control": "public,max-age=86400"
              },
              "defaultFiles": [
                "index.html",
                "default.htm"
              ],
              "mimeTypes": {
                ".myapp": "application/x-msdownload",
                ".htm3": "text/html"
              },
              "enableDirectoryBrowser": true
            }
          ]
        }        
      }

Recommendations
---------------

This middleware must be inserted before any authentication or routing
directives
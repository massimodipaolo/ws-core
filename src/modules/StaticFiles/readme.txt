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

{
  "Name": "app.core.Extensions.StaticFiles",
  "Options": [
    {
      "Headers": {
        "Cache-Control": "public,max-age=43200"
      }
    },
    {
      "Path": "wwwroot/folder",
      "RequestPath": "/download",
      "Headers": {
        "Cache-Control": "public,max-age=86400"
      },
      "DefaultFiles": [
        "index.html",
        "default.htm"
      ],
      "MIMEtypes": {
        ".myapp": "application\/x-msdownload",
        ".htm3": "text/html"
      },
      "EnableDirectoryBrowser": true
    }
  ]
}

Recommendations
---------------

This middleware must be inserted before any authentication or routing
directives
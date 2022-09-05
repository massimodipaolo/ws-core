# ImageProcessor

#### Table of Contents

1. [Description](#description)
1. [Setup](#setup)
   - [Requirements](#setup-requirements)
   - [Configuration](#setup-configuration)
1. [Usage](#usage)
1. [Limitations](#limitations)
1. [Development](#development)

## <a id="description"></a>Description

The `ImageProcessor` module install and configures the [ImageSharp.Web](https://docs.sixlabors.com/articles/imagesharp.web) middleware that exposes API endpoints for common image processing operations and the building blocks to allow for the development of additional extensions to add image sources, caching mechanisms or even your own processing API.

## <a id="setup"></a>Setup

You can install the `ImageProcessor` module adding the reference in your `Core` application and configure it in the `ext-settings.json` file.

### <a id="setup-requirements"></a>Requirements

No other modules are required.

### <a id="setup-configuration"></a>Configuration

You need to define 2 options:

1. **config**: you can refer to the `ImageSharpMiddlewareOptions` official [documentation](https://docs.sixlabors.com/api/ImageSharp.Web/SixLabors.ImageSharp.Web.Middleware.ImageSharpMiddlewareOptions.html).
1. **fileSystemCache**: you can refer to the `PhysicalFileSystemCacheOptions` official [documentation](https://docs.sixlabors.com/api/ImageSharp.Web/SixLabors.ImageSharp.Web.Caching.PhysicalFileSystemCacheOptions.html?q=PhysicalFileSystemCacheOptions).

#### Configuration example

```json
{
  "extConfig": {
    "assemblies": {
      "Ws.Core.Extensions.ImageProcessor": {
        "priority": 100,
        "options": {
          "config": {
            "browserMaxAge": "7.00:00:00",
            "cacheMaxAge": "365.00:00:00",
            "cachedNameLength": 12
          },
          "fileSystemCache": {
            "cacheRoot": "wwwroot",
            "cacheFolder": "is-cache"
          }
        }
      }
    }
  }
}
```

## <a id="usage"></a>Usage

You can refer to the `ImageSharp.Web` official documentation:

1. [Getting Started](https://docs.sixlabors.com/articles/imagesharp.web/gettingstarted.html)
1. [Processing Commands](https://docs.sixlabors.com/articles/imagesharp.web/processingcommands.html)
1. [Image Providers](https://docs.sixlabors.com/articles/imagesharp.web/imageproviders.html)
1. [Image Caches](https://docs.sixlabors.com/articles/imagesharp.web/imagecaches.html)

#### Usage example

You can add parameters to the image url to process `ImageSharp.Web` middleware commands like `resize` (see the `Processing Commands` docs for details):

```bash
{PATH_TO_YOUR_IMAGE}?width=300
{PATH_TO_YOUR_IMAGE}?width=300&height=120&rxy=0.37,0.78
{PATH_TO_YOUR_IMAGE}?width=50&height=50&rsampler=nearest&rmode=stretch
{PATH_TO_YOUR_IMAGE}?width=300&compand=true&orient=false
```

## <a id="limitations"></a>Limitations

⚠️Be aware that if you are using the `StaticFiles` middleware to serve images on a virtual path, this configuration will not be automatically used by `ImageSharp.Web`. The default `PhysicalFileSystemProvider` will allow the processing and serving of image files only from the `WebRootPath` folder following the physical path. If you need to change this behavior you need to implement your `IImageProvider`.

## <a id="development"></a>Development

Development note, contribution info, licensing to be defined.

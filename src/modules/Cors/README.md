# CORS

#### Table of Contents

1. [Description](#description)
1. [Setup](#setup)
   - [Requirements](#setup-requirements)
   - [Configuration](#setup-configuration)
1. [Usage](#usage)
1. [Limitations](#limitations)
1. [Development](#development)

## <a id="description"></a>Description

The `CORS` module installs and configures the desired CORS (Cross-Origin Resource Sharing) policies.

## <a id="setup"></a>Setup

You can install the `Cors` module adding the reference in your `Core` application and configure it in the `ext-settings.json` file.

### <a id="setup-requirements"></a>Requirements

No other modules are required.

### <a id="setup-configuration"></a>Configuration

You can define a list of policies through the `policies` attribute to configure the module.
A policy is defines by these options:

1. **name**: name of the policy.
1. **origins**: list of origins allowed by the policy.
1. **methods** (_optional_): allowed methods for the policy. If empty all method will be used.
   - supported methods are: `GET`, `HEAD`, `POST`, `PUT`, `PATCH`, `DELETE`, `OPTIONS`, `TRACE`.
1. **headers** (_optional_): used uo whitelist specific headers.
1. **exposedHeaders** (_optional_): used to specify other headers available to the application.
1. **allowCredentials** (_default_: `false`): credentials include cookies as well as HTTP authentication schemes.
1. **preflightMaxAgeInSeconds** (_optional_): value in seconds. The Access-Control-Max-Age header specifies how long the response to the preflight request can be cached.

#### Configuration example

```json
{
  "extConfig": {
    "assemblies": {
      "Ws.Core.Extensions.Cors": {
        "priority": 100,
        "options": {
          "policies": [
            {
              "name": "pAllowAnyVerb",
              "allowCredentials": true,
              "origins": ["https://localhost:60935"]
            },
            {
              "name": "pAllowOnlyGet",
              "methods": ["HEAD", "OPTIONS", "GET"],
              "allowCredentials": true,
              "origins": ["https://foo.bar.com"]
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

Nothing to report.

## <a id="development"></a>Development

Development note, contribution info, licensing to be defined.

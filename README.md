![GitHub](https://img.shields.io/github/license/netsoft-ruidias/api-client-3.0)
![.Net Core Version](https://img.shields.io/badge/.NET%20Core-3.0-green)
![GitHub Workflow Status](https://img.shields.io/github/workflow/status/netsoft-ruidias/api-client-3.0/.NET%20Core)
![GitHub last commit](https://img.shields.io/github/last-commit/netsoft-ruidias/api-client-3.0)

# Netsoft Infrastructure API Client 3.0

Common HTTP interfaces for API "client side" support   
This is a new implementation proposal based in some pre established requisites

## What is done

### Prerequisites

- Build with the new .NETCore 3.0
- Has Resilience with _Polly_
- Has Stream serialization / deserialization for performance
- Support fluent for client configuration
- Minimum dependencies (only one package)

### And also:

- _HttpClient_ life cycle based on _HttpClientFactory_ (as best practice recommended by Microsoft)
- No dependency on "_Newtonsoft_", instead rely on "_System.Text.Json.JsonSerializer_" (netcore3 core) which has better performance
- Minimum effort to setup and run
- Implemented strategy for dependency injection configuration
- Implemented strategy (cleaner and transparent) for identity token injection
- Every operations are async (including serialization/deserialization) to avoid thread locks
- Using "_ResponseHeadersRead_" (and stream deserialization) for improved performance

## What still needs to be done

- Some code needs refactoring (especially "_OAuthTokenProvider_" that has some harcoded values)
- Only "_GET_" and "_POST_" were implemented, need to do the same for "_PUT_", "_DELETE_", "_PATCH_" and "HEAD" (almost just copy-paste code)
- Identity build on [IdentityServer4](http://docs.identityserver.io/en/latest/), needs other strategies
- Identity only supports "_client_credentials_", need to implement other strategies
- Unit Tests

## What is not done

- Handle exceptions (Coming soon or read my suggestion below)

## Getting started

### For microservices developers

In your microservice solution, create an ClientSDK project and then follow this simple steps

#### Step 1

Install the new package in your "Client.SDK" project   
(this is the only dependency you will need)

```bash
dotnet add package Netsoft.Core.ApiClient.3.0.0
```

#### Step 2

Create a new class for your client

```csharp
public class DemoClientSDK
{ }
```

Inherit from "_HttpClientBase_" and create your own interface for DI   
The constructor must receive and "HttpClient" which will be injected by DI

```csharp
public class DemoClientSDK : HttpClientBase, IDemoClientSDK
{ 
    public DemoClientSDK(HttpClient httpClient)
        : base(httpClient)
    { }
}
```

Implement your own methods:

```csharp
public class DemoClientSDK : HttpClientBase, IDemoClientSDK
{ 
    public DemoClientSDK(HttpClient httpClient)
        : base(httpClient)
    { }

    public async Task<FooResponse> GetFooAsync(int id)
    {
        var result = await this.GetAsync<FooResponse>($"api/Values/{id}");
        return result.Body;
    }
}
```

#### Step 3

Create a new static class for your DI configuration

```csharp
public static class ServiceConfigExtensions
{ }
```

Create an extension for "_IServiceCollection_"

```csharp
public static class ServiceConfigExtensions
{ 
    public static IServiceCollection AddDemoClient(
        this IServiceCollection services, 
        Uri serviceBaseAddress, 
        IPolicyConfig policyConfig, 
        IIdentityConfig identityConfig)
    {
        return services;
    }
}
```

- **IServiceCollection** is the unity DI services configuration collection
- **Uri** should be your service url
- **IPolicyConfig** is supplied by the netsoft package _Netsoft.Core.ApiClient.3.0_ and it is intended to inject resilience configurations
- **IIdentityConfig** is supplied by netsoft package _Netsoft.Core.ApiClient.3.0_ and it is intended to inject indentity configurations

Be aware that all configurations must be supplied by the consumer

Configure your service client   
(this configurations may vary according to your own needs)

```csharp
public static class ServiceConfigExtensions
{ 
    public static IServiceCollection AddDemoClient(
        this IServiceCollection services, 
        Uri serviceBaseAddress, 
        IPolicyConfig policyConfig, 
        IIdentityConfig identityConfig)
    {
        services.AddHttpClient<IDemoClientSDK, DemoClientSDK>(x =>
        {
            x.BaseAddress = serviceBaseAddress;
            x.DefaultRequestHeaders.Add("Accept", "application/json");
            x.DefaultRequestHeaders.Add("User-Agent", "HttpClientFactory-Sample");
            x.DefaultRequestVersion = new Version(2, 0);
        })
        .AddResilience(policyConfig)
        .AddIdentity(identityConfig, new TokenStrategy());
        
        return services;
    }
}
```


### For microservices consumers

Microservices consumers have their life easier

In your Gateway project, create a new static class for your DI configuration

```csharp
public static class DataGatewayExtensions
{ }
```


Create an extension for "_IServiceCollection_"

```csharp
public static class ServiceConfigExtensions
{ 
    public static IServiceCollection AddDataGateways(this IServiceCollection services)
    {
        return services;
    }
}
```

Reference or install the ClientDSK package and use their own extension to configure the client, like this:

```csharp
public static class ServiceConfigExtensions
{ 
    public static IServiceCollection AddDataGateways(this IServiceCollection services)
    {
        var serviceProvider = services.BuildServiceProvider();

        var policyConfig = serviceProvider.GetRequiredService<IPolicyConfig>();
        var identityConfig = serviceProvider.GetRequiredService<IIdentityConfig>();

        services.AddDemoClient(
            new Uri("https://localhost:44348"),
            policyConfig,
            identityConfig);

        return services;
    }
}
```

Then, in your "Presentation" project simply bind your **IPolicyConfig** and **IIdentityConfig** with the configuration file and call then ".AddDataGateways" extension


```csharp
public static class ApiExtensions
{ 
    public static IServiceCollection AddSettings(this IServiceCollection services)
    {
        services.AddSingleton<IPolicyConfig>(serviceProvider =>
        {
            var config = serviceProvider.GetRequiredService<IConfiguration>();
            var policyConfig = new PolicyConfig();

            config.Bind("PolicyConfig", policyConfig);

            return policyConfig;
        });

        services.AddSingleton<IIdentityConfig>(serviceProvider =>
        {
            var config = serviceProvider.GetRequiredService<IConfiguration>();
            var identityConfig = new IdentityConfig();

            config.Bind("IdentityConfig", identityConfig);

            return identityConfig;
        });

        return services;
    }
}
```

use it

```csharp
public class Startup
{
    public void ConfigureServices(IServiceCollection services)
    {
        services.AddOptions();

        services.AddMvc()

        //(...Swagger...etc...)

        services
            .AddSettings()          // from Presentation project
            .AddDataGateways();     // from Gateway project
    }
}
```

## Handling Exceptions

As a best practice, Microsoft recommends the use of a middleware in replacement of the old handlers   

Add the ExceptionHandler middleware to the pipeline using the Configure method of the startup class

```csharp
public class Startup
{
    public void Configure(IApplicationBuilder app, IHostingEnvironment env)
    {
        app.UsePingPong()
           .StartSwagger();

        app.UseMiddleware<ExceptionsMiddleware>();

        app.UseMvc();

        return app;
    }
}
```

## Contributing
Contributors welcome! 

Thanks for taking an interest in the library and the github community!

1. Clone this repo
2. Make some changes
3. Make a pull request

## Licence
[MIT](https://github.com/netsoft-ruidias/api-client-3.0/blob/master/LICENSE)

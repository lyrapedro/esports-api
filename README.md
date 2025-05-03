## Requirements
[.NET 8.0](https://dotnet.microsoft.com/pt-br/download/dotnet/8.0 ".NET 8.0")

## Supported games

- Counter Strike 2 (requires [Browserless.io](https://account.browserless.io/dashboard/ "Browserless.io") account for local development)
- Valorant

## Deployed API
#### [Swagger](https://esports-api.fly.dev/swagger/index.html "Swagger")

# Local development
> To use CS2 endpoints, you need to create an account on the free [Browserless.io](https://account.browserless.io/dashboard/ "Browserless.io") plan and populate the **BrowserlessIO:Token** environment variable in appsettings with your token

### Clone the repo
`git clone https://github.com/lyrapedro/esports-api.git`

### (Optional) Update appsettings.Development.json to use CS2 endpoints
In BrowserlessIO section update the token with your token
`"Token": "YOUR-TOKEN-HERE"`

### Run the local API
```bash
cd esports-api
dotnet run --project EsportsApi.API/EsportsApi.API.csproj
```

# Docs
in development, use the swagger for now...

### End

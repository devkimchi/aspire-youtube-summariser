# Aspire Youtube Summariser

This provides sample Aspire-orchestrated apps that summarise a YouTube video transcript to a given language.

## Prerequisites

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0?WT.mc_id=dotnet-107070-juyoo)
- [Visual Studio 2022](https://visualstudio.microsoft.com?WT.mc_id=dotnet-107070-juyoo) 17.9 or later with the .NET Aspire workload installed
- [Docker Desktop](https://docker.com/products/docker-desktop)
- [Azure Developer CLI](https://learn.microsoft.com/azure/developer/azure-developer-cli/overview?WT.mc_id=dotnet-107070-juyoo)
- [Azure CLI](https://learn.microsoft.com/cli/azure/what-is-azure-cli?WT.mc_id=dotnet-107070-juyoo)
- [GitHub CLI](https://cli.github.com/)
- [PowerShell](https://learn.microsoft.com/powershell/scripting/overview?WT.mc_id=dotnet-107070-juyoo)
- [Azure subscription](https://azure.microsoft.com/free?WT.mc_id=dotnet-107070-juyoo)
- [Azure OpenAI Service subscription](https://aka.ms/oaiapply)

## Getting Started

### Explore the app

1. Checkout to the `existing` tag

   ```bash
   git checkout tags/existing
   dotnet restore && dotnet build
   ```

1. Rename `appsettings.Development.sample.json` in the `ApiApp` project to `appsettings.Development.json` and add Azure OpenAI Service details &ndash; endpoint, API key and deployment ID &ndash; to the file. You can get these details from the [Azure Portal](https://portal.azure.com/?WT.mc_id=dotnet-107070-juyoo).

1. Run the API app in a terminal

   ```bash
   dotnet run --project AspireYouTubeSummariser.ApiApp
   ```

1. Run the web app in another terminal

   ```bash
   dotnet run --project AspireYouTubeSummariser.WebApp
   ```

### Add .NET Aspire

1. Checkout to the `aspire` tag

   ```bash
   git checkout tags/aspire
   dotnet restore && dotnet build
   ```

1. Run the Aspire AppHost app

   ```bash
   dotnet run --project AspireYouTubeSummariser.AppHost
   ```

### Add Redis Cache to WebApp

1. Checkout to the `redis` tag

   ```bash
   git checkout tags/redis
   dotnet restore && dotnet build
   ```

1. Run the Aspire AppHost app

   ```bash
   dotnet run --project AspireYouTubeSummariser.AppHost
   ```

### Update Resiliency Settings

1. Checkout to the `polly` tag

   ```bash
   git checkout tags/polly
   dotnet restore && dotnet build
   ```

1. Run the Aspire AppHost app

   ```bash
   dotnet run --project AspireYouTubeSummariser.AppHost
   ```

### Add Azure Queue Storage and Table Storage for Async Processing

1. Checkout to the `queue` tag

   ```bash
   git checkout tags/queue
   dotnet restore && dotnet build
   ```

1. Rename `appsettings.Development.sample.json` in the `Worker` project to `appsettings.Development.json` and add Azure OpenAI Service details &ndash; endpoint, API key and deployment ID &ndash; to the file. You can get these details from the [Azure Portal](https://portal.azure.com/?WT.mc_id=dotnet-107070-juyoo).

1. Rename `appsettings.Development.sample.json` in the `AppHost` project to `appsettings.Development.json` and add Azure Queue/Table Storage Account details &ndash; connection strings &ndash; to the file You can get these details from the [Azure Portal](https://portal.azure.com/?WT.mc_id=dotnet-107070-juyoo).

1. Run the Aspire AppHost app

   ```bash
   dotnet run --project AspireYouTubeSummariser.AppHost
   ```

### Deploy to Azure

1. Checkout to the `main` branch

   ```bash
   git switch main
   dotnet restore && dotnet build
   ```

1. Rename `appsettings.Development.sample.json` in the `AppHost` project to `appsettings.Development.json`.

1. Add Azure OpenAI Service details &ndash; endpoint, API key and deployment ID &ndash; to the file. You can get these details from the [Azure Portal](https://portal.azure.com/?WT.mc_id=dotnet-107070-juyoo).

1. Add Azure Queue/Table Storage Account details &ndash; connection strings &ndash; to the file You can get these details from the [Azure Portal](https://portal.azure.com/?WT.mc_id=dotnet-107070-juyoo).

1. Run the following commands in order:

   ```bash
   # Initialise azd
   AZURE_ENV_NAME="aspire$RANDOM"
   azd init -e $AZURE_ENV_NAME

   # Provision resources to Azure
   azd provision

   # Provision GitHub Actions environment
   azd pipeline config
   pwsh Set-GitHubActionsVariables.ps1 -GitHubAlias <GitHubAlias>

   # Provision rest of resources to Azure outside Aspire
   pwsh Run-PostProvision.ps1 -GitHubAlias <GitHubAlias>

   # Deploy apps to Azure
   azd deploy
   ```

## Resources

- [.NET Aspire overview](https://learn.microsoft.com/dotnet/aspire/get-started/aspire-overview?WT.mc_id=dotnet-107070-juyoo)
- [.NET Aspire orchestration](https://learn.microsoft.com/dotnet/aspire/app-host-overview?WT.mc_id=dotnet-107070-juyoo)
- [.NET Aspire components](https://learn.microsoft.com/dotnet/aspire/components-overview?WT.mc_id=dotnet-107070-juyoo)
- [.NET Aspire samples](https://github.com/dotnet/aspire-samples)

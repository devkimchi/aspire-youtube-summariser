# Aspire Youtube Summariser

This provides sample Aspire apps that summarise a YouTube video transcript to a given language.

## Prerequisites

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0?WT.mc_id=dotnet-96932-juyoo)
- [Visual Studio](https://visualstudio.microsoft.com/vs?WT.mc_id=dotnet-96932-juyoo)
- [Azure Subscription](https://azure.microsoft.com/free?WT.mc_id=dotnet-96932-juyoo)
- [Azure OpenAI Service](https://learn.microsoft.com/azure/ai-services/openai/overview?WT.mc_id=dotnet-96932-juyoo)

## Known Issues

- If your YouTube video is long enough, you might exceed the maximum number of tokens that Azure OpenAI Service can handle. For more information, see [Model summary](https://learn.microsoft.com/azure/ai-services/openai/concepts/models?WT.mc_id=dotnet-96932-juyoo#model-summary-table-and-region-availability).
  - `gpt-35-turbo`: 4K
  - `gpt-35-turbo-16k`: 16K
  - `gpt-4`: 4K
  - `gpt-4-32K`: 32K

## Getting Started

TBD

## Resources

- [What is Aspire?](https://learn.microsoft.com/dotnet-internal/aspire/get-started/aspire-overview?WT.mc_id=dotnet-96932-juyoo)

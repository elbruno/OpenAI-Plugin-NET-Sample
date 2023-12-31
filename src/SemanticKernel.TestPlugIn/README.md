# Creating and using a ChatGPT plugin

The `SemanticKernel.TestPlugIn` console application is based on the [ChatGPT plugin](https://learn.microsoft.com/en-us/semantic-kernel/ai-orchestration/chatgpt-plugins) doc article.

We can use this sample to test the ChatGPT Plugin [El Bruno Pets](../OpenAI.PlugIn.NETSample/).

## Prerequisites

- Install the recommended extensions
  - [C#](https://marketplace.visualstudio.com/items?itemName=ms-dotnettools.csharp)
  - [Semantic Kernel Tools](https://marketplace.visualstudio.com/items?itemName=ms-semantic-kernel.semantic-kernel)

## Configuring the sample

The sample can be configured by using either:

- Enter secrets at the command line with [.NET Secret Manager](#using-net-secret-manager)
- Enter secrets in [appsettings.json](#using-appsettingsjson)

For Debugging the console application alone, we suggest using .NET [Secret Manager](https://learn.microsoft.com/en-us/aspnet/core/security/app-secrets) to avoid the risk of leaking secrets into the repository, branches and pull requests.

### Using .NET [Secret Manager](https://learn.microsoft.com/en-us/aspnet/core/security/app-secrets)

Configure an OpenAI endpoint

```powershell
cd 01-Semantic-Functions
dotnet user-secrets set "serviceType" "OpenAI"
dotnet user-secrets set "serviceId" "text-davinci-003"
dotnet user-secrets set "deploymentOrModelId" "text-davinci-003"
dotnet user-secrets set "apiKey" "... your OpenAI key ..."
```

Configure an Azure OpenAI endpoint

```powershell
cd 01-Semantic-Functions
dotnet user-secrets set "serviceType" "AzureOpenAI"
dotnet user-secrets set "serviceId" "text-davinci-003"
dotnet user-secrets set "deploymentOrModelId" "text-davinci-003"
dotnet user-secrets set "endpoint" "https:// ... your endpoint ... .openai.azure.com/"
dotnet user-secrets set "apiKey" "... your Azure OpenAI key ..."
```

Configure the Semantic Kernel logging level

```powershell
dotnet user-secrets set "LogLevel" 0
```

Log levels:

- 0 = Trace
- 1 = Debug
- 2 = Information
- 3 = Warning
- 4 = Error
- 5 = Critical
- 6 = None

### Using appsettings.json

Configure an OpenAI endpoint

1. Copy [settings.json.openai-example](./config/appsettings.json.openai-example) to `./Config/appsettings.json`
1. Edit the file to add your OpenAI endpoint configuration

Configure an Azure OpenAI endpoint

1. Copy [settings.json.azure-example](./config/appsettings.json.azure-example) to `./Config/appsettings.json`
1. Edit the file to add your Azure OpenAI endpoint configuration

## Running the sample
Before running the sample, make sure you have the [OpenAI.PlugIn.NETSample](../OpenAI.PlugIn.NETSample/) running locally.

To run the console application just hit `bashTo build and run the console application from the terminal use the following commands:

```powershBefore running the sample, make sure you have the [OpenAI.PlugIn.NETSample](../OpenAI.PlugIn.NETSample/) running bash
dotnet build
dotnet run
```

For the question "***How many cats are in the db?***", the console output should look like this:

```bash
Microsoft.SemanticKernel.Kernel: Debug: Response : [FINAL ANSWER]
There are two cats in the El Bruno's Pet catalog: Ace (a Calico) and Whiskers (a Persian).
Microsoft.SemanticKernel.Kernel: Information: Final Answer: There are two cats in the El Bruno's Pet catalog: Ace (a Calico) and Whiskers (a Persian).
Result: There are two cats in the El Bruno's Pet catalog: Ace (a Calico) and Whiskers (a Persian).
Steps Taken: 4
Skills Used: 1 (PetsPlugin.GetAllPets(1))
```
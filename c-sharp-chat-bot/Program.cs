using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.Extensions.Configuration;

// Load Secrets

var builder = new ConfigurationBuilder()
    .AddUserSecrets<Program>();

IConfiguration config = builder.Build();

// Quit on KeyError (i.e. NULL)

string? modelId = config["ModelId"];

if (string.IsNullOrEmpty(modelId))
{
    return;
}

string? apiKey = config["ApiKey"];

if (string.IsNullOrEmpty(apiKey))
{
    return;
}

string? orgId = config["OrgId"];

if (string.IsNullOrEmpty(orgId))
{
    return;
}

// Create Model

IKernelBuilder kernelBuilder = Kernel.CreateBuilder();
kernelBuilder.AddOpenAIChatCompletion(
    modelId: modelId,
    apiKey: apiKey,
    orgId: orgId
);

Kernel kernel = kernelBuilder.Build();

var chatCompletionService = kernel.GetRequiredService<IChatCompletionService>();

ChatHistory history = new ChatHistory();
history.AddUserMessage("What is the weather like in Lima, Peru in June.");

var response = chatCompletionService.GetStreamingChatMessageContentsAsync(
    chatHistory: history,
    kernel: kernel
);

await foreach (var chunk in response)
{
    Console.Write(chunk);
}
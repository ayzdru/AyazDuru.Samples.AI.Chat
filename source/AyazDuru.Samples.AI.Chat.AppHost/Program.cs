var builder = DistributedApplication.CreateBuilder(args);

var ollama = builder.AddOllama("ollama")
    .WithDataVolume("ollama_data")
    .WithImageTag("0.11.7")
    .WithLifetime(ContainerLifetime.Persistent)
    .WithEndpoint(name: "https", port: 11434, isProxied: false);
var chat = ollama.AddModel("chat", "llama3.2");
var embeddings = ollama.AddModel("embeddings", "all-minilm");

var vectorDB = builder.AddQdrant("vectordb")
    .WithDataVolume("vectordb_data")
    .WithImageTag("v1.15.4")
    .WithLifetime(ContainerLifetime.Persistent)
    .WithEndpoint(name: "https", port: 6333, isProxied: false);
    

var webApp = builder.AddProject<Projects.AyazDuru_Samples_AI_Chat_Web>("aichatweb-app");
webApp
    .WithReference(chat)
    .WithReference(embeddings)
    .WaitFor(chat)
    .WaitFor(embeddings);
webApp
    .WithReference(vectorDB)
    .WaitFor(vectorDB);

var consoleApp = builder.AddProject<Projects.AyazDuru_Samples_AI_Chat_Console>("aichatconsole-app");
consoleApp
    .WithReference(chat)
    .WithReference(embeddings)
    .WaitFor(chat)
    .WaitFor(embeddings);
consoleApp
    .WithReference(vectorDB)
    .WaitFor(vectorDB);

builder.Build().Run();

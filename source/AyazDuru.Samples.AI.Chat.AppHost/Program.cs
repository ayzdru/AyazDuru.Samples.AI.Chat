using Aspire.Hosting;

var builder = DistributedApplication.CreateBuilder(args);

var ollama = builder.AddOllama("ollama", port: 11434)
    .WithDataVolume("ollama_data")
    .WithImageTag("0.11.7")
    .WithLifetime(ContainerLifetime.Persistent);
var chat = ollama.AddModel("chat", "llama3.2");
var embeddings = ollama.AddModel("embeddings", "all-minilm");

var vectorDB = builder.AddQdrant("vectordb", null,grpcPort: 6334, httpPort: 6333)
    .WithDataVolume("vectordb_data")
    .WithImageTag("v1.15.4")
    .WithLifetime(ContainerLifetime.Persistent);
    

var webApp = builder.AddProject<Projects.AyazDuru_Samples_AI_Chat_Web>("aichatweb-app");
webApp
    .WithReference(chat)
    .WithReference(embeddings)
    .WaitFor(chat)
    .WaitFor(embeddings);
webApp
    .WithReference(vectorDB)
    .WaitFor(vectorDB);

var consoleApp = builder.AddProject<Projects.AyazDuru_Samples_AI_Chat_Console>("aichatconsole-app")
    .WithExplicitStart()
        .ExcludeFromManifest();
consoleApp
    .WithReference(chat)
    .WithReference(embeddings)
    .WaitFor(chat)
    .WaitFor(embeddings);
consoleApp
    .WithReference(vectorDB)
    .WaitFor(vectorDB);

builder.Build().Run();

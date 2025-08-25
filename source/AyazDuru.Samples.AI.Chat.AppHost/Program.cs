var builder = DistributedApplication.CreateBuilder(args);

var ollama = builder.AddOllama("ollama")
    .WithDataVolume("ollama_data").WithImageTag("0.11.6");
var chat = ollama.AddModel("chat", "llama3.2");

var webApp = builder.AddProject<Projects.AyazDuru_Samples_AI_Chat_Web>("aichatweb-app");
webApp
    .WithReference(chat)
    .WaitFor(chat);

builder.Build().Run();

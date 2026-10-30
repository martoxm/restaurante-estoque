var builder = DistributedApplication.CreateBuilder(args);

builder.AddProject<Projects.RestauranteEstoque_Identidade_Api>("identidade", launchProfileName: "https");
builder.AddProject<Projects.RestauranteEstoque_Api>("estoque", launchProfileName: "https");
builder.AddProject<Projects.RestauranteEstoque_Vendas_Api>("vendas", launchProfileName: "https");
builder.AddProject<Projects.RestauranteEstoque_Pagamentos_Api>("pagamentos", launchProfileName: "https");

builder.Build().Run();

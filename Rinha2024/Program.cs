using Microsoft.EntityFrameworkCore;
using Rinha2024;
using Rinha2024.Data;

var builder = WebApplication.CreateSlimBuilder(args);

builder.Services.ConfigureHttpJsonOptions(options =>
{
    options.SerializerOptions.TypeInfoResolverChain.Insert(0, AppJsonSerializerContext.Default);
});

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContextPool<AppDBContext>(options =>
    options.UseNpgsql(connectionString));

var app = builder.Build();

var clienteApi = app.MapGroup("/clientes");
clienteApi.MapearClientesApi();

app.Run();
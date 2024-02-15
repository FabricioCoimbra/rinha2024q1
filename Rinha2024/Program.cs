using Microsoft.AspNetCore.Http.Timeouts;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Rinha2024.Data;
using Rinha2024.Service;

var builder = WebApplication.CreateSlimBuilder(args);

builder.Services.ConfigureHttpJsonOptions(options =>
{
    options.SerializerOptions.TypeInfoResolverChain.Insert(0, AppJsonSerializerContext.Default);
});

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContextPool<AppDBContext>(options =>
    options.UseNpgsql(connectionString), poolSize: 100);

builder.Services.AddRequestTimeouts(options => options.DefaultPolicy = new RequestTimeoutPolicy { Timeout = TimeSpan.FromSeconds(60) });
//TODO remover logs para rodar pra valer
//builder.Logging.ClearProviders();

var app = builder.Build();

var clienteApi = app.MapGroup("/clientes");

clienteApi.MapPost("/{id}/transacoes", ClienteService.ExecutaTransacao);

clienteApi.MapGet("/", async ([FromServices] AppDBContext dbContext) =>
    await dbContext.Clientes.AsNoTracking().ToListAsync());

clienteApi.MapGet("/{id}/extrato", ClienteService.ListarExtrato);

app.Run();
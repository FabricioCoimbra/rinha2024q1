using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Rinha2024.Data;
using Rinha2024.Model;

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
clienteApi.MapPost("/{id}/transacoes", async (int id, [FromBody] Transacao transacao, [FromServices] AppDBContext dbContext) =>
{
    if (id < 1 || id > 5)
        return Results.NotFound();

    var cliente = await dbContext.Clientes.AsNoTracking().FirstOrDefaultAsync(c => c.Id == id);
    if (cliente is null)
        return Results.NotFound();

    long valor = 0;
    if (transacao.Tipo == "c")
    {
        valor = cliente.Saldoinicial + transacao.Valor;        
    }        
    else
    {
        valor = cliente.Saldoinicial - transacao.Valor;
        if (!(valor > cliente.Limite * -1))
            return Results.UnprocessableEntity();
    }

    var registrosAfetados = await dbContext.Database
        .ExecuteSqlAsync($"UPDATE public.\"Clientes\" SET \"Saldoinicial\" = {valor}  WHERE \"Id\"= {id};");

    transacao.IdCliente = id;
    transacao.Descricao ??= "";
    transacao.Realizada_em = DateTime.UtcNow.AddHours(-3);
    dbContext.Transacoes.Add(transacao);
    await dbContext.SaveChangesAsync();
    return Results.Ok(new TransacaoResponse(cliente.Limite, valor));
});

clienteApi.MapGet("/", async ([FromServices] AppDBContext dbContext) =>
{
    var Clientes = new Cliente[] {
        new(1, 100000, 0),
        new(2, 80000, 0),
        new(3, 1000000, 0),
        new(4, 10000000, 0),
        new(5, 500000, 0)
    };

    foreach (var cliente in Clientes)
    {
        if (!await dbContext.Clientes.AnyAsync(c => c.Id == cliente.Id))
        {
            dbContext.Clientes.Add(cliente);
        }
    }

    await dbContext.SaveChangesAsync();
    return await dbContext.Clientes.AsNoTracking().ToListAsync();
});

clienteApi.MapGet("/{id}/extrato", async (int id, [FromServices] AppDBContext dbContext) =>
{
    if (id < 1 || id > 5)
        return Results.NotFound();

    var cliente = await dbContext.Clientes.AsNoTracking().FirstOrDefaultAsync(c => c.Id == id);
    if (cliente is null)
        return Results.NotFound();

    var saldo = new Saldo() { Data_extrato = DateTime.Now, Limite = cliente.Limite, Total = cliente.Saldoinicial };
    var transacoesDoCliente = await dbContext.Transacoes.Where(t => t.IdCliente == id)
        .OrderByDescending(t => t.Realizada_em)
        .Take(10)        
        .AsNoTracking()        
        .ToListAsync();
    return Results.Ok(new Extrato() { Saldo = saldo, Ultimas_transacoes = transacoesDoCliente });
});

app.Run();
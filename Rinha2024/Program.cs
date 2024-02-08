using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Rinha2024.Data;
using Rinha2024.Model;

var builder = WebApplication.CreateSlimBuilder(args);

builder.Services.ConfigureHttpJsonOptions(options =>
{
    options.SerializerOptions.TypeInfoResolverChain.Insert(0, AppJsonSerializerContext.Default);
});

//protegidos de erros unicamente pelo poder da l�gica
int[] Limites = [0, 100000, 80000, 1000000, 10000000, 500000];

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContextPool<AppDBContext>(options =>
    options.UseNpgsql(connectionString));

var app = builder.Build();

var clienteApi = app.MapGroup("/clientes");
clienteApi.MapPost("/{id}/transacoes", async (int id, [FromBody] Transacao transacao, [FromServices] AppDBContext dbContext) =>
{
    if (id < 1 || id > 5)
        return Results.NotFound();

    if (string.IsNullOrEmpty(transacao.Descricao) ||
        transacao.Descricao.Length > 10 ||
        transacao.Valor % 1 != 0 || //verifica se foi informado um valor inteiro
        transacao.Valor < 0 ||
        (transacao.Tipo != "c" && transacao.Tipo != "d"))
        return Results.UnprocessableEntity();

    transacao.Descricao ??= "";
    var saldos = transacao.Tipo == "c" ?
        await dbContext.AtualizarSaldos
            .FromSqlInterpolated($"SELECT * FROM atualizar_saldo_credito({id}, {(int)transacao.Valor}, {transacao.Descricao})")
            .ToListAsync()
            :
       await dbContext.AtualizarSaldos
            .FromSqlInterpolated($"SELECT * FROM atualizar_saldo_debito({id}, {(int)transacao.Valor}, {transacao.Descricao})")
            .ToListAsync();

        if (saldos.FirstOrDefault()?.saldo_atual is null)
            return Results.UnprocessableEntity();

    return Results.Ok(new TransacaoResponse(Limites[id], saldos.FirstOrDefault()?.saldo_atual ?? 0));
});

clienteApi.MapGet("/", async ([FromServices] AppDBContext dbContext) =>
{
    return await dbContext.Clientes.AsNoTracking().ToListAsync();
});

clienteApi.MapGet("/{id}/extrato", async (int id, [FromServices] AppDBContext dbContext) =>
{
    if (id < 1 || id > 5)
        return Results.NotFound();

    var cliente = await dbContext.Clientes
    .Include(c => c.Transacoes)
    .AsNoTracking()
    .FirstOrDefaultAsync(c => c.Id == id);
    if (cliente is null)
        return Results.NotFound();

    var saldo = new Saldo() { Data_extrato = DateTime.Now, Limite = Limites[id], Total = cliente.Saldoinicial };
    var transacoesDoCliente = cliente.Transacoes
        .OrderByDescending(t => t.Realizada_em)
        .Take(10)       
        .ToList();
    return Results.Ok(new Extrato() { Saldo = saldo, Ultimas_transacoes = transacoesDoCliente });
});

app.Run();
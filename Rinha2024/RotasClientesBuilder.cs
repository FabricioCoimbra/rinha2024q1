using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Rinha2024.Data;
using Rinha2024.Model;

namespace Rinha2024
{
    //precisei criar essa classe pros testes unitários :)
    public static class RotasClientesBuilder
    {
        public static RouteGroupBuilder MapearClientesApi(this RouteGroupBuilder clienteApi)
        {
            clienteApi.MapPost("/{id}/transacoes", ExecutaTransacao);

            clienteApi.MapGet("/", async ([FromServices] AppDBContext dbContext) =>
                await dbContext.Clientes.AsNoTracking().ToListAsync());

            clienteApi.MapGet("/{id}/extrato", ListarExtrato);

            return clienteApi;
        }

        public static async Task<IResult> ExecutaTransacao(int id, [FromBody] Transacao transacao, [FromServices] AppDBContext dbContext)
        {
            if (id < 1 || id > 5)
                return Results.NotFound();

            if (string.IsNullOrEmpty(transacao.Descricao) ||
                transacao.Descricao.Length > 10 ||
                transacao.Valor % 1 != 0 || //verifica se foi informado um valor inteiro
                transacao.Valor < 0 ||
                (transacao.Tipo != "c" && transacao.Tipo != "d"))
                return Results.UnprocessableEntity();

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

            //protegidos de erros unicamente pelo poder da lógica
            int[] Limites = [0, 100000, 80000, 1000000, 10000000, 500000];
            return Results.Ok(new TransacaoResponse(Limites[id], saldos.FirstOrDefault()?.saldo_atual ?? 0));
        }

        public static async Task<IResult> ListarExtrato(int id, [FromServices] AppDBContext dbContext)
        {
            if (id < 1 || id > 5)
                return Results.NotFound();

            var cliente = await dbContext.Clientes
            .Include(c => c.Transacoes)
            .AsNoTracking()
            .FirstOrDefaultAsync(c => c.Id == id);
            if (cliente is null)
                return Results.NotFound();

            //protegidos de erros unicamente pelo poder da lógica (e código duplicado também)
            int[] Limites = [0, 100000, 80000, 1000000, 10000000, 500000];
            var saldo = new Saldo() { Data_extrato = DateTime.Now, Limite = Limites[id], Total = cliente.Saldoinicial };
            var transacoesDoCliente = cliente.Transacoes
                .OrderByDescending(t => t.Realizada_em)
                .Take(10)
                .ToList();
            return Results.Ok(new Extrato() { Saldo = saldo, Ultimas_transacoes = transacoesDoCliente });
        }
    }
}

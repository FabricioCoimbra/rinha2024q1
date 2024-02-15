using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Rinha2024.Data;
using Rinha2024.Model;

namespace Rinha2024.Service
{
    //Não me guento sem criar um service separado com as regras de negócio kkkkkkkkkkkkkk
    public static class ClienteService
    {
        //protegidos de erros unicamente pelo poder da lógica
        private static readonly int[] Limites = [0, 100000, 80000, 1000000, 10000000, 500000];
        public static async Task<IResult> ExecutaTransacao(int id, [FromBody] Transacao transacao, [FromServices] AppDBContext dbContext)
        {
            if (id < 1 || id > 5)
                return Results.NotFound();

            if (string.IsNullOrEmpty(transacao.Descricao) ||
                transacao.Descricao.Length > 10 ||
                transacao.Valor % 1 != 0 || //verifica se foi informado um valor inteiro
                transacao.Valor < 0 ||
                transacao.Tipo != "c" && transacao.Tipo != "d")
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

            var saldo = new Saldo() { Data_extrato = DateTime.Now, Limite = Limites[id], Total = cliente.Saldoinicial };
            var transacoesDoCliente = cliente.Transacoes
                .OrderByDescending(t => t.Realizada_em)
                .Take(10)
                .ToList();
            return Results.Ok(new Extrato() { Saldo = saldo, Ultimas_transacoes = transacoesDoCliente });
        }
    }
}

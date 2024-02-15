using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Rinha2024.Data;
using Rinha2024.Model;
using Rinha2024.Service;

namespace TestRinha2024q1;

public class ClienteLimitesTest
{
    private const string descricao = "Teste";

    [Theory]
    [InlineData(1, 100001)]
    [InlineData(2, 80001)]
    [InlineData(3, 1000001)]
    [InlineData(4, 10000001)]
    [InlineData(5, 500001)]
    public async Task ExecutaTransacao_ComLimiteExcedido(int id, decimal valor)
    {
        // Arrange
        var dbContextMock = new AppDBContext();
        var transacao = new Transacao { Descricao = descricao, Valor = valor, Tipo = "d" };

        // Act
        var resultado = await ClienteService.ExecutaTransacao(id, transacao, dbContextMock);

        // Assert
        Assert.IsType<UnprocessableEntity>(resultado);
    }

    [Theory]
    [InlineData(1, 100000)]
    [InlineData(2, 80000)]
    [InlineData(3, 1000000)]
    [InlineData(4, 10000000)]
    [InlineData(5, 500000)]
    public async Task ExecutaTransacao_ComLimiteIgual(int id, decimal valor)
    {
        // Arrange
        var dbContextMock = new AppDBContext();
        var dbContextMock2 = new AppDBContext();
        var transacao = new Transacao { Descricao = descricao, Valor = valor, Tipo = "d" };
        var transacao2 = new Transacao { Descricao = descricao, Valor = valor, Tipo = "c" };//reverter operação

        // Act
        var task1 = ClienteService.ExecutaTransacao(id, transacao, dbContextMock);
        var task2 = ClienteService.ExecutaTransacao(id, transacao2, dbContextMock2);
        var resultados = await Task.WhenAll(task1, task2);

        var resultado1 = resultados[0];
        var resultado2 = resultados[1];

        // Assert
        Assert.IsType<Ok<TransacaoResponse>>(resultado1);
        Assert.IsType<Ok<TransacaoResponse>>(resultado2);

        var extrato = await ClienteService.ListarExtrato(id, dbContextMock);
        var transacaoResponse = Assert.IsType<Ok<Extrato>>(extrato).Value;
        Assert.NotNull(transacaoResponse);
        Assert.Equal(0, transacaoResponse.Saldo.Total);
        Assert.NotEmpty(transacaoResponse.Ultimas_transacoes);
    }
    //[Fact]
    //public async Task ExecutaTransacaoConcorrente()
    //{
    //    // Arrange
    //    var services = new ServiceCollection();
    //    var connectionString = "Host=localhost;Port=5432;Database=rinhadb;Username=admin;Password=makeLifeSimple;Pooling=true;Minimum Pool Size=50;Maximum Pool Size=2000;Multiplexing=true;Timeout=15;Command Timeout=15;Cancellation Timeout=-1;No Reset On Close=true;Max Auto Prepare=20;Auto Prepare Min Usages=1;";
    //    services.AddDbContextPool<AppDBContext>(options =>
    //        options.UseNpgsql(connectionString), poolSize: 100);

    //    await using var provider = services.BuildServiceProvider();

    //    var options = new ParallelOptions()
    //    {
    //        MaxDegreeOfParallelism = 350
    //    };

    //    //int[] clientes = [1, 2, 3, 4, 5];
    //    int[] clientes = [1];

    //    // Act
    //    await Parallel.ForEachAsync(clientes, options, async (idCliente, ct) => { //Para cada cliente uma task
    //        var valores = Enumerable.Repeat(idCliente, 1).ToArray();
    //        await Parallel.ForEachAsync(valores, options, async (valorTransacao, ct) => {//para cada valor uma task

    //            //2 escopos por operação, um para crédito, e outro para débito
    //            await using var escopoDesseValor = provider.CreateAsyncScope();
                
    //            var dbContextMock = escopoDesseValor.ServiceProvider.GetRequiredService<AppDBContext>();
    //            await using var novoEscopo =  provider.CreateAsyncScope();
    //            var dbContextMock2 = novoEscopo.ServiceProvider.GetRequiredService<AppDBContext>();

    //            var transacao = new Transacao { Descricao = descricao, Valor = valorTransacao, Tipo = "d" };
    //            var transacao2 = new Transacao { Descricao = descricao, Valor = valorTransacao, Tipo = "c" };//reverter operação

    //            var task1 = ClienteService.ExecutaTransacao(idCliente, transacao, dbContextMock);
    //            var task2 = ClienteService.ExecutaTransacao(idCliente, transacao2, dbContextMock2);
    //            var resultados = await Task.WhenAll(task1, task2);

    //            var resultado1 = resultados[0];
    //            var resultado2 = resultados[1];

    //    // Assert
    //            Assert.IsType<Ok<TransacaoResponse>>(resultado1);
    //            Assert.IsType<Ok<TransacaoResponse>>(resultado2);

    //            var extrato = await ClienteService.ListarExtrato(idCliente, dbContextMock);
    //            var transacaoResponse = Assert.IsType<Ok<Extrato>>(extrato).Value;
    //            Assert.NotNull(transacaoResponse);
    //            Assert.NotEmpty(transacaoResponse.Ultimas_transacoes);
    //        });
    //    });

    //    // Assert
    //    foreach (var idCliente in clientes)
    //    {
    //        var dbContextMock = provider.GetRequiredService<AppDBContext>();
    //        var extrato = await ClienteService.ListarExtrato(idCliente, dbContextMock);
    //        var transacaoResponse = Assert.IsType<Ok<Extrato>>(extrato).Value;
    //        Assert.NotNull(transacaoResponse);
    //        //Se todos os crédito e débitos foram processados o saldo final deve ser 0
    //        Assert.Equal(0, transacaoResponse.Saldo.Total);
    //        Assert.NotEmpty(transacaoResponse.Ultimas_transacoes);
    //        Assert.Equal(10, transacaoResponse.Ultimas_transacoes.Count);
    //    }
    //}
}

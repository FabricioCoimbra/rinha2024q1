using Microsoft.AspNetCore.Http.HttpResults;
using Rinha2024.Data;
using Rinha2024.Model;
using Rinha2024.Service;

namespace TestRinha2024q1
{
    public class ClienteOperacaoInvalidaTest
    {
        [Theory]
        [InlineData(0)]
        [InlineData(6)]
        public async Task ExecutaTransacao_ComIdInvalido(int id)
        {
            // Arrange
            var dbContextMock = new AppDBContext();
            var transacao = new Transacao { Descricao = "Teste", Valor = 100, Tipo = "c" };

            // Act
            var resultado = await ClienteService.ExecutaTransacao(id, transacao, dbContextMock);

            // Assert
            Assert.IsType<NotFound>(resultado);
        }

        [Theory]
        [InlineData("Teste", 10, "a")]
        [InlineData("Teste", 10.5, "c")]
        [InlineData("Teste123456789789798798789", 10, "c")]
        [InlineData(null, 10, "c")]
        [InlineData("", 10, "c")]
        public async Task ExecutaTransacao_ComTransacaoInvalida(string descricao, decimal valor, string tipo)
        {
            // Arrange
            var dbContextMock = new AppDBContext();
            var transacao = new Transacao { Descricao = descricao, Valor = valor, Tipo = tipo };

            // Act
            var resultado = await ClienteService.ExecutaTransacao(1, transacao, dbContextMock);

            // Assert
            Assert.IsType<UnprocessableEntity>(resultado);
        }


        [Theory]
        [InlineData(0)]
        [InlineData(6)]
        public async Task ListarExtrato_ComIdInvalido(int id)
        {
            // Arrange
            var dbContextMock = new AppDBContext();
            var transacao = new Transacao { Descricao = "Teste", Valor = 100, Tipo = "c" };

            // Act
            var resultado = await ClienteService.ListarExtrato(id, dbContextMock);

            // Assert
            Assert.IsType<NotFound>(resultado);
        }
    }
}
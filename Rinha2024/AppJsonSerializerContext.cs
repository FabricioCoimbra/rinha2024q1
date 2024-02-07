using Rinha2024.Model;
using System.Text.Json.Serialization;

[JsonSerializable(typeof(Cliente[]))]
[JsonSerializable(typeof(Transacao))]
[JsonSerializable(typeof(TransacaoResponse))]
[JsonSerializable(typeof(Extrato))]
internal partial class AppJsonSerializerContext : JsonSerializerContext
{

}

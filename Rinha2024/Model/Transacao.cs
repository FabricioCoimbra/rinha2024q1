using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Rinha2024.Model;
public class Transacao
{
    public Transacao()
    {
        this.Realizada_em = DateTime.UtcNow.AddHours(-3);
    }
    public int Id { get; set; }
    public long Valor { get; set; }
    [MaxLength(1)]
    public string Tipo { get; set; }
    [MaxLength(100)]
    public string? Descricao { get; set; }
    [JsonIgnore]
    public int? IdCliente { get; set; }
    [JsonIgnore]
    public Cliente? Cliente { get; set; }
    public DateTime Realizada_em { get; set; }
}

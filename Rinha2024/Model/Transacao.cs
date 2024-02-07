using System.ComponentModel.DataAnnotations;

namespace Rinha2024.Model;
public class Transacao
{
    public Transacao()
    {
        this.Realizada_em = DateTime.UtcNow.AddHours(-3);
    }
    public int Id { get; set; }
    public required long Valor { get; set; }
    [MaxLength(1)]
    public required string Tipo { get; set; }
    [MaxLength(100)]
    public string? Descricao { get; set; }
    public int? IdCliente { get; set; }
    public Cliente? Cliente { get; set; }
    public DateTime Realizada_em { get; set; }
}

namespace Rinha2024.Model;
public class Cliente(int Id, int Limite, long Saldoinicial)
{
    public int Id { get; set; } = Id;
    public int Limite { get; set; } = Limite;
    public long Saldoinicial { get; set; } = Saldoinicial;
    public ICollection<Transacao> Transacoes { get; set; } = new List<Transacao>();
}

namespace Rinha2024.Model
{

    public class Extrato
    {
        public required Saldo Saldo { get; set; }
        public List<Transacao> Ultimas_transacoes { get; set; } = [];
    }

    public class Saldo
    {
        public long Total { get; set; }
        public DateTime Data_extrato { get; set; }
        public int Limite { get; set; }
    }
}

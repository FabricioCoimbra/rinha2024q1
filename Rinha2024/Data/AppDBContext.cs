using Microsoft.EntityFrameworkCore;
using Rinha2024.Model;

namespace Rinha2024.Data;
public class AppDBContext : DbContext
{
    public AppDBContext()
    {
    }
    public AppDBContext(DbContextOptions<AppDBContext> options) : base(options)
    {
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
        {
            var connectionString = "Host=localhost;Port=5432;Database=rinhadb;Username=admin;Password=makeLifeSimple;Pooling=true;Minimum Pool Size=50;Maximum Pool Size=2000;Multiplexing=true;Timeout=15;Command Timeout=15;Cancellation Timeout=-1;No Reset On Close=true;Max Auto Prepare=20;Auto Prepare Min Usages=1;";
            optionsBuilder.UseNpgsql(connectionString);
        }
        base.OnConfiguring(optionsBuilder);
    }

    public DbSet<Cliente> Clientes => Set<Cliente>();
    public DbSet<Transacao> Transacoes => Set<Transacao>();
    public DbSet<RetornoAtualizarSaldo> AtualizarSaldos => Set<RetornoAtualizarSaldo>();
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Transacao>()
            .HasOne(t => t.Cliente)
            .WithMany(c => c.Transacoes)
            .HasForeignKey(t => t.IdCliente);

        modelBuilder.Entity<RetornoAtualizarSaldo>().HasNoKey();

        base.OnModelCreating(modelBuilder);
    }
}
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Rinha2024.Model;

namespace Rinha2024.Data;
public class AppDBContext : DbContext
{
    public AppDBContext(DbContextOptions<AppDBContext> options) : base(options)
    {        
    }

#if DEBUG
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured) 
        {
            var connectionString = "Host=localhost;Port=5432;Database=rinhadb;Username=admin;Password=makeLifeSimple;";
            optionsBuilder.UseNpgsql(connectionString);
        }
        base.OnConfiguring(optionsBuilder);
    }
#endif

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
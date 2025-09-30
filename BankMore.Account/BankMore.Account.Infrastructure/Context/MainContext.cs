using BankMore.Account.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace BankMore.Account.Infrastructure.Context
{
    public class MainContext : DbContext
    {
        public MainContext(DbContextOptions<MainContext> options)
         : base(options)
        {
        }

        public DbSet<ContaCorrente> ContasCorrentes { get; set; }
        public DbSet<Movimento> Movimentos { get; set; }
        public DbSet<Transferencia> Transferencias { get; set; }
        public DbSet<Tarifa> Tarifas { get; set; }
        public DbSet<Idempotencia> Idempotencias { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Conta Corrente
            modelBuilder.Entity<ContaCorrente>(entity =>
            {
                entity.HasKey(e => e.IdContaCorrente);
                entity.Property(e => e.Numero).IsRequired();
                entity.Property(e => e.Nome).IsRequired();
            });

            // Movimento
            modelBuilder.Entity<Movimento>(entity =>
            {
                entity.HasKey(e => e.IdMovimento);
                entity.HasOne(e => e.ContaCorrente)
                      .WithMany(c => c.Movimentos)
                      .HasForeignKey(e => e.IdContaCorrente);
            });

            // Transferência
            modelBuilder.Entity<Transferencia>(entity =>
            {
                entity.HasKey(e => e.IdTransferencia);
            });

            // Tarifa
            modelBuilder.Entity<Tarifa>(entity =>
            {
                entity.HasKey(e => e.IdTarifa);
                entity.HasOne(e => e.ContaCorrente)
                      .WithMany(c => c.Tarifas)
                      .HasForeignKey(e => e.IdContaCorrente);
            });

            // Idempotência
            modelBuilder.Entity<Idempotencia>(entity =>
            {
                entity.HasKey(e => e.Chave_Idempotencia);
            });

            base.OnModelCreating(modelBuilder);
        }
    }
}
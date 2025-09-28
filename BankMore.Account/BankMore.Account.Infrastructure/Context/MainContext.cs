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

        public DbSet<ContaCorrente> ContaCorrente { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ContaCorrente>(entity =>
            {
                entity.ToTable("ContaCorrente");

                // Chave primária
                entity.HasKey(e => e.Id);

                // Guid <-> String no SQLite
                entity.Property(e => e.Id)
                      .HasConversion(
                          guid => guid.ToString(),
                          str => Guid.Parse(str));

                // Configurações de colunas
                entity.Property(e => e.Cpf)
                      .IsRequired()
                      .HasColumnType("TEXT");

                entity.Property(e => e.PasswordHash)
                      .IsRequired()
                      .HasColumnType("TEXT");

                entity.Property(e => e.AccountNumber)
                      .IsRequired()
                      .HasColumnType("TEXT");

                entity.Property(e => e.Active)
                      .IsRequired()
                      .HasColumnType("INTEGER"); // SQLite não tem bool, INTEGER 0/1
            });

            base.OnModelCreating(modelBuilder);
        }
    }
}

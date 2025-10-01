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
        public DbSet<Idempotencia> Idempotencias { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // ContaCorrente
            modelBuilder.Entity<ContaCorrente>(entity =>
            {
                entity.ToTable("CONTACORRENTE");

                entity.HasKey(e => e.IdContaCorrente);

                entity.Property(e => e.IdContaCorrente)
                  .HasConversion(
                      v => v.ToString("D"),       
                      v => Guid.Parse(v))
                  .HasColumnType("NVARCHAR2(37)")
                  .IsRequired();

                entity.Property(e => e.Numero)
                      .IsRequired();

                entity.HasIndex(e => e.Numero)
                      .IsUnique();

                entity.Property(e => e.Nome)
                      .HasMaxLength(100)
                      .IsRequired();

                entity.Property(e => e.Ativo)
                      .HasColumnType("NUMBER(1)")
                      .IsRequired();

                entity.Property(e => e.Senha)
                      .HasMaxLength(100)
                      .IsRequired();

                entity.Property(e => e.Salt)
                      .HasMaxLength(100)
                      .IsRequired();
            });

            // Movimento
            modelBuilder.Entity<Movimento>(entity =>
            {
                entity.ToTable("MOVIMENTO");

                entity.HasKey(e => e.IdMovimento);

                entity.Property(e => e.IdMovimento)
                 .HasConversion(
                     v => v.ToString("D"),
                     v => Guid.Parse(v))
                 .HasColumnType("NVARCHAR2(37)")
                 .IsRequired();

                entity.Property(e => e.IdContaCorrente)
                  .HasConversion(
                      v => v.ToString("D"),
                      v => Guid.Parse(v))
                  .HasColumnType("NVARCHAR2(37)")
                  .IsRequired();

                entity.Property(e => e.DataMovimento)
                      .HasMaxLength(25)
                      .IsRequired();

                entity.Property(e => e.TipoMovimento)
                      .HasMaxLength(1)
                      .IsRequired();

                entity.Property(e => e.Valor)
                      .HasColumnType("NUMBER(12,2)")
                      .IsRequired();

                entity.HasOne(e => e.ContaCorrente)
                      .WithMany(c => c.Movimentos)
                      .HasForeignKey(e => e.IdContaCorrente);
            });

            // Idempotencia
            modelBuilder.Entity<Idempotencia>(entity =>
            {
                entity.ToTable("IDEMPOTENCIA");

                entity.HasKey(e => e.ChaveIdempotencia);

                entity.Property(e => e.ChaveIdempotencia)
                .HasColumnName("Chave_Idempotencia")
                  .HasConversion(
                      v => v.ToString("D"),
                      v => Guid.Parse(v))
                  .HasColumnType("NVARCHAR2(37)")
                  .IsRequired();

                entity.Property(e => e.Requisicao)
                      .HasMaxLength(1000);

                entity.Property(e => e.Resultado)
                      .HasMaxLength(1000);
            });

            base.OnModelCreating(modelBuilder);
        }
    }
}
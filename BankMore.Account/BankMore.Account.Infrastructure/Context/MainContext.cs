using BankMore.Account.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

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
            var guidConverter = new ValueConverter<Guid, byte[]>(
        v => v.ToByteArray(),
        v => new Guid(v));

            // -----------------------------
            // Conta Corrente
            // -----------------------------
            modelBuilder.Entity<ContaCorrente>(entity =>
            {
                entity.ToTable("CONTACORRENTE");

                entity.HasKey(e => e.IdContaCorrente);

                entity.Property(e => e.IdContaCorrente)
                      .HasColumnType("RAW(16)")
                      .HasConversion(guidConverter)
                      .IsRequired();

                entity.Property(e => e.Numero)
                      .HasMaxLength(20)
                      .IsRequired();

                entity.Property(e => e.Nome)
                      .HasMaxLength(100)
                      .IsRequired();

                entity.Property(e => e.Cpf)
                      .HasMaxLength(20)
                      .IsRequired();

                entity.Property(e => e.Senha)
                      .HasMaxLength(200)
                      .IsRequired();

                // Boolean mapeado como NUMBER(1)
                entity.Property(e => e.Ativo)
                      .HasColumnType("NUMBER(1)")
                      .IsRequired();
            });

            // -----------------------------
            // Movimento
            // -----------------------------
            modelBuilder.Entity<Movimento>(entity =>
            {
                entity.ToTable("MOVIMENTO");

                entity.HasKey(e => e.IdMovimento);

                entity.Property(e => e.IdMovimento)
                      .HasColumnType("RAW(16)")
                      .HasConversion(guidConverter)
                      .IsRequired();

                entity.Property(e => e.Valor)
                      .HasPrecision(18, 2)
                      .IsRequired();

                entity.HasOne(e => e.ContaCorrente)
                      .WithMany(c => c.Movimentos)
                      .HasForeignKey(e => e.IdContaCorrente);
            });

            // -----------------------------
            // Transferência
            // -----------------------------
            modelBuilder.Entity<Transferencia>(entity =>
            {
                entity.ToTable("TRANSFERENCIA");

                entity.HasKey(e => e.IdTransferencia);

                entity.Property(e => e.IdTransferencia)
                      .HasColumnType("RAW(16)")
                      .HasConversion(guidConverter)
                      .IsRequired();

                entity.Property(e => e.Valor)
                      .HasPrecision(18, 2)
                      .IsRequired();
            });

            // -----------------------------
            // Tarifa
            // -----------------------------
            modelBuilder.Entity<Tarifa>(entity =>
            {
                entity.ToTable("TARIFA");

                entity.HasKey(e => e.IdTarifa);

                entity.Property(e => e.IdTarifa)
                      .HasColumnType("RAW(16)")
                      .HasConversion(guidConverter)
                      .IsRequired();

                entity.Property(e => e.Valor)
                      .HasPrecision(18, 2)
                      .IsRequired();

                entity.HasOne(e => e.ContaCorrente)
                      .WithMany(c => c.Tarifas)
                      .HasForeignKey(e => e.IdContaCorrente);
            });

            // -----------------------------
            // Idempotência
            // -----------------------------
            modelBuilder.Entity<Idempotencia>(entity =>
            {
                entity.ToTable("IDEMPOTENCIA");

                entity.HasKey(e => e.Chave_Idempotencia);

                entity.Property(e => e.Chave_Idempotencia)
                      .HasMaxLength(100)
                      .IsRequired();
            });

            base.OnModelCreating(modelBuilder);
        }
    }
}
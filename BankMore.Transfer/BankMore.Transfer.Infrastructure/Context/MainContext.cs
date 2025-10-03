
using BankMore.Transfer.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace BankMore.Transfer.Infrastructure.Context
{
    public class MainContext : DbContext
    {
        public MainContext(DbContextOptions<MainContext> options)
         : base(options)
        {
        }

        public DbSet<Transferencia> Transferencias { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // ContaCorrente
            modelBuilder.Entity<Transferencia>(entity =>
            {
                entity.ToTable("TRANSFERENCIA");

                entity.HasKey(e => e.IdTransferencia);

                entity.Property(e => e.IdTransferencia)
                  .HasConversion(
                      v => v.ToString("D"),
                      v => Guid.Parse(v))
                  .HasColumnType("NVARCHAR2(37)")
                  .IsRequired();

                entity.Property(e => e.IdContaCorrenteOrigem)
                  .HasConversion(
                      v => v.ToString("D"),       
                      v => Guid.Parse(v))
                  .HasColumnType("NVARCHAR2(37)")
                  .IsRequired();

                entity.Property(e => e.IdContaCorrenteDestino)
                  .HasConversion(
                      v => v.ToString("D"),
                      v => Guid.Parse(v))
                  .HasColumnType("NVARCHAR2(37)")
                  .IsRequired();

                entity.Property(e => e.DataTransferencia)
                      .IsRequired();

                entity.Property(e => e.Valor)
                          .HasColumnType("NUMBER(12,2)")
                          .IsRequired();
            });

            base.OnModelCreating(modelBuilder);
        }
    }
}
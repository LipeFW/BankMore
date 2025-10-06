using BankMore.Tariff.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace BankMore.Tariff.Infrastructure.Context
{
    public class MainContext : DbContext
    {
        public MainContext(DbContextOptions<MainContext> options)
         : base(options)
        {
        }

        public DbSet<Tarifa> Tarifas { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Tarifa
            modelBuilder.Entity<Tarifa>(entity =>
            {
                entity.ToTable("TARIFA");
                entity.HasKey(e => e.IdTarifa);
                entity.Property(e => e.IdTarifa)
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
                entity.Property(e => e.Valor)
                          .HasColumnType("NUMBER(12,2)")
                          .IsRequired();
                entity.Property(e => e.DataTarifacao)
                      .IsRequired();
            });
            base.OnModelCreating(modelBuilder);
        }
    }
}

using Domain.Entities;
using Domain.Enums;
using Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
namespace Infra.Data.Configurations;
public class VendaConfiguration : IEntityTypeConfiguration<Venda>
{
    public void Configure(EntityTypeBuilder<Venda> builder)
    {
        builder.ToTable("Vendas");
        builder.HasKey(v => v.Id);
        builder.Property(v => v.NumeroVenda)
            .IsRequired()
            .HasMaxLength(50);
        builder.Property(v => v.Data)
            .IsRequired();
        builder.Property(v => v.ClienteId)
            .HasConversion(
                v => v.Value,
                v => new ClienteId(v))
            .IsRequired();
        builder.Property(v => v.ClienteNome)
            .IsRequired()
            .HasMaxLength(200);
        builder.Property(v => v.FilialId)
            .HasConversion(
                v => v.Value,
                v => new FilialId(v))
            .IsRequired();
        builder.Property(v => v.FilialNome)
            .IsRequired()
            .HasMaxLength(200);
        builder.Property(v => v.Status)
            .HasConversion<int>()
            .IsRequired();
        builder.Property(v => v.ValorTotal)
            .HasPrecision(18, 2)
            .IsRequired();
        builder.Property(v => v.CreatedAt)
            .IsRequired();
        builder.Property(v => v.UpdatedAt);
        builder.HasMany(v => v.Itens)
            .WithOne()
            .HasForeignKey(i => i.VendaId)
            .OnDelete(DeleteBehavior.Cascade);
        builder.Ignore(v => v.DomainEvents);
    }
}

using Domain.Entities;
using Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
namespace Infra.Data.Configurations;
public class VendaItemConfiguration : IEntityTypeConfiguration<VendaItem>
{
    public void Configure(EntityTypeBuilder<VendaItem> builder)
    {
        builder.ToTable("VendaItens");
        builder.HasKey(i => i.Id);
        builder.Property(i => i.VendaId)
            .IsRequired();
        builder.Property(i => i.ProdutoId)
            .HasConversion(
                v => v.Value,
                v => new ProdutoId(v))
            .IsRequired();
        builder.Property(i => i.ProdutoNome)
            .IsRequired()
            .HasMaxLength(200);
        builder.Property(i => i.Quantidade)
            .IsRequired();
        builder.Property(i => i.ValorUnitario)
            .HasPrecision(18, 2)
            .IsRequired();
        builder.Property(i => i.Desconto)
            .HasPrecision(18, 2)
            .IsRequired();
        builder.Property(i => i.ValorTotalItem)
            .HasPrecision(18, 2)
            .IsRequired();
    }
}

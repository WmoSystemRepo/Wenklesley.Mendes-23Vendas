using System;
using Infra.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
#nullable disable
namespace Infra.Migrations
{
    [DbContext(typeof(VendaContext))]
    partial class VendaContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.4")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);
            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);
            modelBuilder.Entity("Domain.Entities.Venda", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");
                    b.Property<Guid>("ClienteId")
                        .HasColumnType("uniqueidentifier");
                    b.Property<string>("ClienteNome")
                        .IsRequired()
                        .HasMaxLength(200)
                        .HasColumnType("nvarchar(200)");
                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("datetime2");
                    b.Property<DateTime>("Data")
                        .HasColumnType("datetime2");
                    b.Property<Guid>("FilialId")
                        .HasColumnType("uniqueidentifier");
                    b.Property<string>("FilialNome")
                        .IsRequired()
                        .HasMaxLength(200)
                        .HasColumnType("nvarchar(200)");
                    b.Property<string>("NumeroVenda")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");
                    b.Property<int>("Status")
                        .HasColumnType("int");
                    b.Property<DateTime?>("UpdatedAt")
                        .HasColumnType("datetime2");
                    b.Property<decimal>("ValorTotal")
                        .HasPrecision(18, 2)
                        .HasColumnType("decimal(18,2)");
                    b.HasKey("Id");
                    b.ToTable("Vendas", (string)null);
                });
            modelBuilder.Entity("Domain.Entities.VendaItem", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");
                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("datetime2");
                    b.Property<decimal>("Desconto")
                        .HasPrecision(18, 2)
                        .HasColumnType("decimal(18,2)");
                    b.Property<Guid>("ProdutoId")
                        .HasColumnType("uniqueidentifier");
                    b.Property<string>("ProdutoNome")
                        .IsRequired()
                        .HasMaxLength(200)
                        .HasColumnType("nvarchar(200)");
                    b.Property<int>("Quantidade")
                        .HasColumnType("int");
                    b.Property<DateTime?>("UpdatedAt")
                        .HasColumnType("datetime2");
                    b.Property<decimal>("ValorTotalItem")
                        .HasPrecision(18, 2)
                        .HasColumnType("decimal(18,2)");
                    b.Property<decimal>("ValorUnitario")
                        .HasPrecision(18, 2)
                        .HasColumnType("decimal(18,2)");
                    b.Property<Guid>("VendaId")
                        .HasColumnType("uniqueidentifier");
                    b.HasKey("Id");
                    b.HasIndex("VendaId");
                    b.ToTable("VendaItens", (string)null);
                });
            modelBuilder.Entity("Domain.Entities.VendaItem", b =>
                {
                    b.HasOne("Domain.Entities.Venda", null)
                        .WithMany("Itens")
                        .HasForeignKey("VendaId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });
            modelBuilder.Entity("Domain.Entities.Venda", b =>
                {
                    b.Navigation("Itens");
                });
#pragma warning restore 612, 618
        }
    }
}

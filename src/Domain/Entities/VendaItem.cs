using Domain.Common;
using Domain.Services;
using Domain.ValueObjects;
namespace Domain.Entities;
public class VendaItem : Entity
{
    public Guid VendaId { get; private set; }
    public ProdutoId ProdutoId { get; private set; } = null!;
    public string ProdutoNome { get; private set; } = null!;
    public int Quantidade { get; private set; }
    public decimal ValorUnitario { get; private set; }
    public decimal Desconto { get; private set; }
    public decimal ValorTotalItem { get; private set; }
    private VendaItem() { }
    public VendaItem(
        Guid vendaId,
        ProdutoId produtoId,
        string produtoNome,
        int quantidade,
        decimal valorUnitario)
    {
        VendaId = vendaId;
        ProdutoId = produtoId;
        ProdutoNome = produtoNome ?? throw new ArgumentNullException(nameof(produtoNome));
        if (quantidade <= 0)
            throw new ArgumentException("Quantidade deve ser maior que zero", nameof(quantidade));
        if (quantidade > 20)
            throw new InvalidOperationException("Quantidade maior que 20 é proibida");
        if (valorUnitario <= 0)
            throw new ArgumentException("Valor unitário deve ser maior que zero", nameof(valorUnitario));
        Quantidade = quantidade;
        ValorUnitario = valorUnitario;
        CalcularValores();
    }
    public void AtualizarQuantidade(int quantidade)
    {
        if (quantidade <= 0)
            throw new ArgumentException("Quantidade deve ser maior que zero", nameof(quantidade));
        if (quantidade > 20)
            throw new InvalidOperationException("Quantidade maior que 20 é proibida");
        Quantidade = quantidade;
        CalcularValores();
    }
    public void AtualizarValorUnitario(decimal valorUnitario)
    {
        if (valorUnitario <= 0)
            throw new ArgumentException("Valor unitário deve ser maior que zero", nameof(valorUnitario));
        ValorUnitario = valorUnitario;
        CalcularValores();
    }
    private void CalcularValores()
    {
        Desconto = DescontoService.CalcularDesconto(Quantidade, ValorUnitario);
        ValorTotalItem = DescontoService.CalcularValorTotalItem(Quantidade, ValorUnitario);
    }
}

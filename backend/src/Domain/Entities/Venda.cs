using Domain.Common;
using Domain.Enums;
using Domain.Events;
using Domain.ValueObjects;
namespace Domain.Entities;
public class Venda : Entity
{
    private readonly List<VendaItem> _itens = new();
    public string NumeroVenda { get; private set; } = null!;
    public DateTime Data { get; private set; }
    public ClienteId ClienteId { get; private set; } = null!;
    public string ClienteNome { get; private set; } = null!;
    public FilialId FilialId { get; private set; } = null!;
    public string FilialNome { get; private set; } = null!;
    public VendaStatus Status { get; private set; }
    public decimal ValorTotal { get; private set; }
    public IReadOnlyCollection<VendaItem> Itens => _itens.AsReadOnly();
    private Venda() { }
    public Venda(
        string numeroVenda,
        ClienteId clienteId,
        string clienteNome,
        FilialId filialId,
        string filialNome)
    {
        NumeroVenda = numeroVenda ?? throw new ArgumentNullException(nameof(numeroVenda));
        Data = DateTime.UtcNow;
        ClienteId = clienteId;
        ClienteNome = clienteNome ?? throw new ArgumentNullException(nameof(clienteNome));
        FilialId = filialId;
        FilialNome = filialNome ?? throw new ArgumentNullException(nameof(filialNome));
        Status = VendaStatus.NaoCancelado;
        ValorTotal = 0;
    }
    public void AdicionarItem(
        ProdutoId produtoId,
        string produtoNome,
        int quantidade,
        decimal valorUnitario)
    {
        if (Status == VendaStatus.Cancelado)
            throw new InvalidOperationException("Não é possível adicionar itens a uma venda cancelada");
        var item = new VendaItem(Id, produtoId, produtoNome, quantidade, valorUnitario);
        _itens.Add(item);
        RecalcularValorTotal();
        if (_itens.Count == 1)
            AddDomainEvent(new CompraEfetuada(Id, NumeroVenda, ValorTotal));
    }
    public void RemoverItem(Guid itemId)
    {
        if (Status == VendaStatus.Cancelado)
            throw new InvalidOperationException("Não é possível remover itens de uma venda cancelada");
        var item = _itens.FirstOrDefault(i => i.Id == itemId);
        if (item == null)
            throw new InvalidOperationException("Item não encontrado");
        _itens.Remove(item);
        RecalcularValorTotal();
        AddDomainEvent(new ItemCancelado(Id, itemId, item.ProdutoNome));
    }
    public void AtualizarItem(
        Guid itemId,
        int? quantidade = null,
        decimal? valorUnitario = null)
    {
        if (Status == VendaStatus.Cancelado)
            throw new InvalidOperationException("Não é possível atualizar itens de uma venda cancelada");
        var item = _itens.FirstOrDefault(i => i.Id == itemId);
        if (item == null)
            throw new InvalidOperationException("Item não encontrado");
        if (quantidade.HasValue)
            item.AtualizarQuantidade(quantidade.Value);
        if (valorUnitario.HasValue)
            item.AtualizarValorUnitario(valorUnitario.Value);
        RecalcularValorTotal();
        AddDomainEvent(new CompraAlterada(Id, NumeroVenda, ValorTotal));
    }
    public void Cancelar()
    {
        if (Status == VendaStatus.Cancelado)
            throw new InvalidOperationException("Venda já está cancelada");
        Status = VendaStatus.Cancelado;
        UpdatedAt = DateTime.UtcNow;
        AddDomainEvent(new CompraCancelada(Id, NumeroVenda));
    }
    private void RecalcularValorTotal()
    {
        ValorTotal = _itens.Sum(i => i.ValorTotalItem);
        UpdatedAt = DateTime.UtcNow;
    }
    private readonly List<DomainEvent> _domainEvents = new();
    public IReadOnlyCollection<DomainEvent> DomainEvents => _domainEvents.AsReadOnly();
    protected void AddDomainEvent(DomainEvent domainEvent)
    {
        _domainEvents.Add(domainEvent);
    }
    public void ClearDomainEvents()
    {
        _domainEvents.Clear();
    }
}

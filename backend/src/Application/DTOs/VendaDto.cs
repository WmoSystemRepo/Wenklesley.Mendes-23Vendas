namespace Application.DTOs;
public class VendaDto
{
    public Guid Id { get; set; }
    public string NumeroVenda { get; set; } = string.Empty;
    public DateTime Data { get; set; }
    public Guid ClienteId { get; set; }
    public string ClienteNome { get; set; } = string.Empty;
    public Guid FilialId { get; set; }
    public string FilialNome { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public decimal ValorTotal { get; set; }
    public List<VendaItemDto> Itens { get; set; } = new();
}

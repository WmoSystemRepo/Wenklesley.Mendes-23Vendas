using Domain.Common;
namespace Domain.ValueObjects;
public class ProdutoId : ValueObject
{
    public Guid Value { get; }
    public ProdutoId(Guid value)
    {
        if (value == Guid.Empty)
            throw new ArgumentException("ProdutoId não pode ser vazio", nameof(value));
        Value = value;
    }
    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value;
    }
    public static implicit operator Guid(ProdutoId produtoId) => produtoId.Value;
    public static implicit operator ProdutoId(Guid value) => new(value);
}

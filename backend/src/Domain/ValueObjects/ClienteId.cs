using Domain.Common;
namespace Domain.ValueObjects;
public class ClienteId : ValueObject
{
    public Guid Value { get; }
    public ClienteId(Guid value)
    {
        if (value == Guid.Empty)
            throw new ArgumentException("ClienteId não pode ser vazio", nameof(value));
        Value = value;
    }
    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value;
    }
    public static implicit operator Guid(ClienteId clienteId) => clienteId.Value;
    public static implicit operator ClienteId(Guid value) => new(value);
}

using Domain.Common;
namespace Domain.ValueObjects;
public class FilialId : ValueObject
{
    public Guid Value { get; }
    public FilialId(Guid value)
    {
        if (value == Guid.Empty)
            throw new ArgumentException("FilialId não pode ser vazio", nameof(value));
        Value = value;
    }
    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value;
    }
    public static implicit operator Guid(FilialId filialId) => filialId.Value;
    public static implicit operator FilialId(Guid value) => new(value);
}

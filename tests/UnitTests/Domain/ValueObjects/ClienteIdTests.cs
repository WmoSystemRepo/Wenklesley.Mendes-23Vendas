using Domain.ValueObjects;
using Shouldly;
using Xunit;
namespace UnitTests.Domain.ValueObjects;
public class ClienteIdTests
{
    [Fact]
    public void CriarClienteId_ComGuidValido_DeveCriarComSucesso()
    {
        var guid = Guid.NewGuid();
        var clienteId = new ClienteId(guid);
        clienteId.Value.ShouldBe(guid);
    }
    [Fact]
    public void CriarClienteId_ComGuidVazio_DeveLancarExcecao()
    {
        Should.Throw<ArgumentException>(() => new ClienteId(Guid.Empty));
    }
    [Fact]
    public void ClienteId_ConversaoImplicita_DeveFuncionar()
    {
        var guid = Guid.NewGuid();
        var clienteId = new ClienteId(guid);
        Guid resultado = clienteId;
        resultado.ShouldBe(guid);
    }
}

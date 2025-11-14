using Application.Commands;
using Bogus;
namespace IntegrationTests.Helpers;
public static class VendaTestHelper
{
    private static readonly Faker _faker = new("pt_BR");
    public static CreateVendaCommand CriarVendaCommandValida(
        int quantidadeItens = 1,
        int quantidadePorItem = 5,
        decimal valorUnitario = 100m)
    {
        var command = new CreateVendaCommand
        {
            NumeroVenda = $"V{_faker.Random.AlphaNumeric(6).ToUpper()}",
            ClienteId = Guid.NewGuid(),
            ClienteNome = _faker.Person.FullName,
            FilialId = Guid.NewGuid(),
            FilialNome = $"Filial {_faker.Address.City()}",
            Itens = new List<CreateVendaItemCommand>()
        };
        for (int i = 0; i < quantidadeItens; i++)
        {
            command.Itens.Add(new CreateVendaItemCommand
            {
                ProdutoId = Guid.NewGuid(),
                ProdutoNome = $"Produto {_faker.Commerce.ProductName()}",
                Quantidade = quantidadePorItem,
                ValorUnitario = valorUnitario
            });
        }
        return command;
    }
    public static CreateVendaCommand CriarVendaCommandComQuantidade(int quantidade)
    {
        return CriarVendaCommandValida(quantidadeItens: 1, quantidadePorItem: quantidade);
    }
    public static CreateVendaCommand CriarVendaCommandInvalida()
    {
        return new CreateVendaCommand
        {
            NumeroVenda = string.Empty,
            ClienteId = Guid.Empty,
            ClienteNome = string.Empty,
            FilialId = Guid.Empty,
            FilialNome = string.Empty,
            Itens = new List<CreateVendaItemCommand>()
        };
    }
    public static CreateVendaCommand CriarVendaCommandComQuantidadeInvalida(int quantidade)
    {
        var command = CriarVendaCommandValida();
        if (command.Itens.Count > 0)
        {
            command.Itens[0].Quantidade = quantidade;
        }
        else
        {
            command.Itens.Add(new CreateVendaItemCommand
            {
                ProdutoId = Guid.NewGuid(),
                ProdutoNome = "Produto Teste",
                Quantidade = quantidade,
                ValorUnitario = 100m
            });
        }
        return command;
    }
}

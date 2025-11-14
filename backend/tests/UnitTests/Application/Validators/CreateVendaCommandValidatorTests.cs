using Application.Commands;
using Application.Validators;
using FluentValidation.TestHelper;
using Xunit;
namespace UnitTests.Application.Validators;
public class CreateVendaCommandValidatorTests
{
    private readonly CreateVendaCommandValidator _validator;
    public CreateVendaCommandValidatorTests()
    {
        _validator = new CreateVendaCommandValidator();
    }
    [Fact]
    public void Validar_ComandoValido_DevePassar()
    {
        var command = new CreateVendaCommand
        {
            NumeroVenda = "V001",
            ClienteId = Guid.NewGuid(),
            ClienteNome = "Cliente Teste",
            FilialId = Guid.NewGuid(),
            FilialNome = "Filial Teste",
            Itens = new List<CreateVendaItemCommand>
            {
                new()
                {
                    ProdutoId = Guid.NewGuid(),
                    ProdutoNome = "Produto Teste",
                    Quantidade = 5,
                    ValorUnitario = 100m
                }
            }
        };
        var result = _validator.TestValidate(command);
        result.ShouldNotHaveAnyValidationErrors();
    }
    [Fact]
    public void Validar_NumeroVendaVazio_DeveFalhar()
    {
        var command = new CreateVendaCommand
        {
            NumeroVenda = string.Empty,
            ClienteId = Guid.NewGuid(),
            ClienteNome = "Cliente Teste",
            FilialId = Guid.NewGuid(),
            FilialNome = "Filial Teste",
            Itens = new List<CreateVendaItemCommand>()
        };
        var result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.NumeroVenda);
    }
    [Fact]
    public void Validar_ItensVazio_DeveFalhar()
    {
        var command = new CreateVendaCommand
        {
            NumeroVenda = "V001",
            ClienteId = Guid.NewGuid(),
            ClienteNome = "Cliente Teste",
            FilialId = Guid.NewGuid(),
            FilialNome = "Filial Teste",
            Itens = new List<CreateVendaItemCommand>()
        };
        var result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.Itens);
    }
    [Fact]
    public void Validar_QuantidadeMaiorQue20_DeveFalhar()
    {
        var command = new CreateVendaCommand
        {
            NumeroVenda = "V001",
            ClienteId = Guid.NewGuid(),
            ClienteNome = "Cliente Teste",
            FilialId = Guid.NewGuid(),
            FilialNome = "Filial Teste",
            Itens = new List<CreateVendaItemCommand>
            {
                new()
                {
                    ProdutoId = Guid.NewGuid(),
                    ProdutoNome = "Produto Teste",
                    Quantidade = 21,
                    ValorUnitario = 100m
                }
            }
        };
        var result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor("Itens[0].Quantidade");
    }
}

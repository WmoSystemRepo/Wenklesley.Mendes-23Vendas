using Domain.Services;
using Shouldly;
using Xunit;
namespace UnitTests.Domain.Services;
public class DescontoServiceTests
{
    [Fact]
    public void CalcularDesconto_QuantidadeAte3_DeveRetornarZero()
    {
        var desconto = DescontoService.CalcularDesconto(3, 100m);
        desconto.ShouldBe(0);
    }
    [Fact]
    public void CalcularDesconto_Quantidade4_DeveRetornarZero()
    {
        var desconto = DescontoService.CalcularDesconto(4, 100m);
        desconto.ShouldBe(0);
    }
    [Fact]
    public void CalcularDesconto_Quantidade5_DeveRetornar10Porcento()
    {
        var desconto = DescontoService.CalcularDesconto(5, 100m);
        desconto.ShouldBe(50m);
    }
    [Fact]
    public void CalcularDesconto_Quantidade9_DeveRetornar10Porcento()
    {
        var desconto = DescontoService.CalcularDesconto(9, 100m);
        desconto.ShouldBe(90m);
    }
    [Fact]
    public void CalcularDesconto_Quantidade10_DeveRetornar20Porcento()
    {
        var desconto = DescontoService.CalcularDesconto(10, 100m);
        desconto.ShouldBe(200m);
    }
    [Fact]
    public void CalcularDesconto_Quantidade20_DeveRetornar20Porcento()
    {
        var desconto = DescontoService.CalcularDesconto(20, 100m);
        desconto.ShouldBe(400m);
    }
    [Fact]
    public void CalcularDesconto_QuantidadeMaiorQue20_DeveLancarExcecao()
    {
        Should.Throw<InvalidOperationException>(() =>
            DescontoService.CalcularDesconto(21, 100m));
    }
    [Fact]
    public void CalcularValorTotalItem_Quantidade5_DeveAplicar10PorcentoDesconto()
    {
        var valorTotal = DescontoService.CalcularValorTotalItem(5, 100m);
        valorTotal.ShouldBe(450m);
    }
    [Fact]
    public void CalcularValorTotalItem_Quantidade15_DeveAplicar20PorcentoDesconto()
    {
        var valorTotal = DescontoService.CalcularValorTotalItem(15, 100m);
        valorTotal.ShouldBe(1200m);
    }
}

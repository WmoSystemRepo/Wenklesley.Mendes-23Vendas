namespace Domain.Services;
public static class DescontoService
{
    public static decimal CalcularDesconto(int quantidade, decimal valorUnitario)
    {
        if (quantidade > 20)
            throw new InvalidOperationException("Quantidade maior que 20 é proibida");
        if (quantidade >= 10 && quantidade <= 20)
            return valorUnitario * quantidade * 0.20m;
        if (quantidade > 4 && quantidade <= 9)
            return valorUnitario * quantidade * 0.10m;
        return 0;
    }
    public static decimal CalcularValorTotalItem(int quantidade, decimal valorUnitario)
    {
        var valorBruto = valorUnitario * quantidade;
        var desconto = CalcularDesconto(quantidade, valorUnitario);
        return valorBruto - desconto;
    }
}

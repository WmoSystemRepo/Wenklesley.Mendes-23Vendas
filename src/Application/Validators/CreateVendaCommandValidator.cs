using Application.Commands;
using FluentValidation;
namespace Application.Validators;
public class CreateVendaCommandValidator : AbstractValidator<CreateVendaCommand>
{
    public CreateVendaCommandValidator()
    {
        RuleFor(x => x.NumeroVenda)
            .NotEmpty().WithMessage("Número da venda é obrigatório")
            .MaximumLength(50).WithMessage("Número da venda deve ter no máximo 50 caracteres");
        RuleFor(x => x.ClienteId)
            .NotEmpty().WithMessage("ClienteId é obrigatório");
        RuleFor(x => x.ClienteNome)
            .NotEmpty().WithMessage("Nome do cliente é obrigatório")
            .MaximumLength(200).WithMessage("Nome do cliente deve ter no máximo 200 caracteres");
        RuleFor(x => x.FilialId)
            .NotEmpty().WithMessage("FilialId é obrigatório");
        RuleFor(x => x.FilialNome)
            .NotEmpty().WithMessage("Nome da filial é obrigatório")
            .MaximumLength(200).WithMessage("Nome da filial deve ter no máximo 200 caracteres");
        RuleFor(x => x.Itens)
            .NotEmpty().WithMessage("A venda deve ter pelo menos um item")
            .Must(itens => itens != null && itens.Count > 0).WithMessage("A venda deve ter pelo menos um item");
        RuleForEach(x => x.Itens)
            .SetValidator(new CreateVendaItemCommandValidator());
    }
}
public class CreateVendaItemCommandValidator : AbstractValidator<CreateVendaItemCommand>
{
    public CreateVendaItemCommandValidator()
    {
        RuleFor(x => x.ProdutoId)
            .NotEmpty().WithMessage("ProdutoId é obrigatório");
        RuleFor(x => x.ProdutoNome)
            .NotEmpty().WithMessage("Nome do produto é obrigatório")
            .MaximumLength(200).WithMessage("Nome do produto deve ter no máximo 200 caracteres");
        RuleFor(x => x.Quantidade)
            .GreaterThan(0).WithMessage("Quantidade deve ser maior que zero")
            .LessThanOrEqualTo(20).WithMessage("Quantidade não pode ser maior que 20");
        RuleFor(x => x.ValorUnitario)
            .GreaterThan(0).WithMessage("Valor unitário deve ser maior que zero");
    }
}

using Application.Commands;
using FluentValidation;
namespace Application.Validators;
public class UpdateVendaCommandValidator : AbstractValidator<UpdateVendaCommand>
{
    public UpdateVendaCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("Id é obrigatório");
        When(x => x.ItensParaAdicionar != null && x.ItensParaAdicionar.Any(), () =>
        {
            RuleForEach(x => x.ItensParaAdicionar)
                .SetValidator(new UpdateVendaItemCommandValidator());
        });
        When(x => x.ItensParaAtualizar != null && x.ItensParaAtualizar.Any(), () =>
        {
            RuleForEach(x => x.ItensParaAtualizar)
                .SetValidator(new UpdateVendaItemExistenteCommandValidator());
        });
    }
}
public class UpdateVendaItemCommandValidator : AbstractValidator<UpdateVendaItemCommand>
{
    public UpdateVendaItemCommandValidator()
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
public class UpdateVendaItemExistenteCommandValidator : AbstractValidator<UpdateVendaItemExistenteCommand>
{
    public UpdateVendaItemExistenteCommandValidator()
    {
        RuleFor(x => x.ItemId)
            .NotEmpty().WithMessage("ItemId é obrigatório");
        When(x => x.Quantidade.HasValue, () =>
        {
            RuleFor(x => x.Quantidade!.Value)
                .GreaterThan(0).WithMessage("Quantidade deve ser maior que zero")
                .LessThanOrEqualTo(20).WithMessage("Quantidade não pode ser maior que 20");
        });
        When(x => x.ValorUnitario.HasValue, () =>
        {
            RuleFor(x => x.ValorUnitario!.Value)
                .GreaterThan(0).WithMessage("Valor unitário deve ser maior que zero");
        });
    }
}

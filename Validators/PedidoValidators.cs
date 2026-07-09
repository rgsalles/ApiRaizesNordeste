using ApiRaizesNordeste.DTOs;
using FluentValidation;

namespace ApiRaizesNordeste.Validators;

/// <summary>
/// Validador para criação de pedido
/// </summary>
public class CreatePedidoDtoValidator : AbstractValidator<CreatePedidoDto>
{
    public CreatePedidoDtoValidator()
    {
        RuleFor(x => x.UnidadeId)
            .NotEmpty().WithMessage("Unidade é obrigatória");

        RuleFor(x => x.Canal)
            .InclusiveBetween(1, 4).WithMessage("Canal deve ser: 1=App, 2=Totem, 3=Balcão, 4=PickUp");

        RuleFor(x => x.Itens)
            .NotEmpty().WithMessage("Pedido deve conter pelo menos um item")
            .Must(items => items.Count > 0).WithMessage("Pedido deve conter itens");

        RuleForEach(x => x.Itens).SetValidator(new ItemPedidoDtoValidator());

        RuleFor(x => x.PontosUtilizados)
            .GreaterThan(0).WithMessage("Pontos devem ser maior que 0, se informados")
            .When(x => x.PontosUtilizados.HasValue);

        RuleFor(x => x.Observacoes)
            .MaximumLength(500).WithMessage("Observações não podem exceder 500 caracteres");
    }
}

/// <summary>
/// Validador para itens do pedido
/// </summary>
public class ItemPedidoDtoValidator : AbstractValidator<ItemPedidoDto>
{
    public ItemPedidoDtoValidator()
    {
        RuleFor(x => x.ProdutoId)
            .NotEmpty().WithMessage("Produto é obrigatório");

        RuleFor(x => x.Quantidade)
            .GreaterThan(0).WithMessage("Quantidade deve ser maior que 0")
            .LessThanOrEqualTo(999).WithMessage("Quantidade não pode exceder 999");

        RuleFor(x => x.Observacoes)
            .MaximumLength(300).WithMessage("Observações do item não podem exceder 300 caracteres");
    }
}

/// <summary>
/// Validador para atualização de status do pedido
/// </summary>
public class AtualizarStatusPedidoDtoValidator : AbstractValidator<AtualizarStatusPedidoDto>
{
    public AtualizarStatusPedidoDtoValidator()
    {
        RuleFor(x => x.NovoStatus)
            .InclusiveBetween(1, 7).WithMessage("Status deve ser um valor válido (1-7)");

        RuleFor(x => x.Motivo)
            .MaximumLength(500).WithMessage("Motivo não pode exceder 500 caracteres");
    }
}

/// <summary>
/// Validador para cancelamento de pedido
/// </summary>
public class CancelarPedidoDtoValidator : AbstractValidator<CancelarPedidoDto>
{
    public CancelarPedidoDtoValidator()
    {
        RuleFor(x => x.Motivo)
            .NotEmpty().WithMessage("Motivo do cancelamento é obrigatório")
            .MinimumLength(3).WithMessage("Motivo deve ter pelo menos 3 caracteres")
            .MaximumLength(500).WithMessage("Motivo não pode exceder 500 caracteres");
    }
}

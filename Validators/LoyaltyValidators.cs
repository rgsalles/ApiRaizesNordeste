using ApiRaizesNordeste.DTOs;
using FluentValidation;

namespace ApiRaizesNordeste.Validators;

/// <summary>
/// Validador para resgate de pontos
/// </summary>
public class ResgatePontsDtoValidator : AbstractValidator<ResgatePointsDto>
{
    public ResgatePontsDtoValidator()
    {
        RuleFor(x => x.Pontos)
            .NotEmpty().WithMessage("Quantidade de pontos é obrigatória")
            .GreaterThan(0).WithMessage("Pontos devem ser maior que 0")
            .LessThanOrEqualTo(999999).WithMessage("Pontos não podem exceder 999999");

        RuleFor(x => x.Descricao)
            .MaximumLength(500).WithMessage("Descrição não pode exceder 500 caracteres");
    }
}

using ApiRaizesNordeste.DTOs;
using FluentValidation;

namespace ApiRaizesNordeste.Validators;

/// <summary>
/// Validador para criação de unidade
/// </summary>
public class CreateUnidadeDtoValidator : AbstractValidator<CreateUnidadeDto>
{
    public CreateUnidadeDtoValidator()
    {
        RuleFor(x => x.Nome)
            .NotEmpty().WithMessage("Nome da unidade é obrigatório")
            .MinimumLength(3).WithMessage("Nome deve ter pelo menos 3 caracteres")
            .MaximumLength(200).WithMessage("Nome não pode exceder 200 caracteres");

        RuleFor(x => x.Endereco)
            .NotEmpty().WithMessage("Endereço é obrigatório")
            .MaximumLength(500).WithMessage("Endereço não pode exceder 500 caracteres");

        RuleFor(x => x.Cidade)
            .NotEmpty().WithMessage("Cidade é obrigatória")
            .MaximumLength(100).WithMessage("Cidade não pode exceder 100 caracteres");

        RuleFor(x => x.Estado)
            .NotEmpty().WithMessage("Estado é obrigatório")
            .Length(2).WithMessage("Estado deve ter exatamente 2 caracteres");

        RuleFor(x => x.CEP)
            .NotEmpty().WithMessage("CEP é obrigatório")
            .Matches(@"^\d{5}-\d{3}$").WithMessage("CEP deve estar no formato xxxxx-xxx");

        RuleFor(x => x.Telefone)
            .NotEmpty().WithMessage("Telefone é obrigatório")
            .Matches(@"^\d{10,11}$").WithMessage("Telefone deve ter 10 ou 11 dígitos");

        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email é obrigatório")
            .EmailAddress().WithMessage("Email deve ser válido");

        RuleFor(x => x.HorarioAbertura)
            .NotEmpty().WithMessage("Horário de abertura é obrigatório");

        RuleFor(x => x.HorarioFechamento)
            .NotEmpty().WithMessage("Horário de fechamento é obrigatório");

        RuleFor(x => x.Latitude)
            .NotEmpty().WithMessage("Latitude é obrigatória")
            .InclusiveBetween(-90, 90).WithMessage("Latitude deve estar entre -90 e 90");

        RuleFor(x => x.Longitude)
            .NotEmpty().WithMessage("Longitude é obrigatória")
            .InclusiveBetween(-180, 180).WithMessage("Longitude deve estar entre -180 e 180");
    }
}

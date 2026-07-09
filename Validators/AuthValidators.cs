using ApiRaizesNordeste.DTOs;
using FluentValidation;

namespace ApiRaizesNordeste.Validators;

/// <summary>
/// Validador para registro de cliente
/// </summary>
public class RegisterClienteDtoValidator : AbstractValidator<RegisterClienteDto>
{
    public RegisterClienteDtoValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email é obrigatório")
            .EmailAddress().WithMessage("Email deve ser válido")
            .MaximumLength(255).WithMessage("Email não pode exceder 255 caracteres");

        RuleFor(x => x.Senha)
            .NotEmpty().WithMessage("Senha é obrigatória")
            .MinimumLength(6).WithMessage("Senha deve ter pelo menos 6 caracteres")
            .MaximumLength(100).WithMessage("Senha não pode exceder 100 caracteres")
            .Matches(@"[A-Z]").WithMessage("Senha deve conter pelo menos uma letra maiúscula")
            .Matches(@"[a-z]").WithMessage("Senha deve conter pelo menos uma letra minúscula")
            .Matches(@"[0-9]").WithMessage("Senha deve conter pelo menos um número");

        RuleFor(x => x.NomeCompleto)
            .NotEmpty().WithMessage("Nome completo é obrigatório")
            .MinimumLength(3).WithMessage("Nome deve ter pelo menos 3 caracteres")
            .MaximumLength(200).WithMessage("Nome não pode exceder 200 caracteres");

        RuleFor(x => x.Telefone)
            .NotEmpty().WithMessage("Telefone é obrigatório")
            .Matches(@"^\d{10,11}$").WithMessage("Telefone deve ter 10 ou 11 dígitos");
    }
}

/// <summary>
/// Validador para login
/// </summary>
public class LoginDtoValidator : AbstractValidator<LoginDto>
{
    public LoginDtoValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email é obrigatório")
            .EmailAddress().WithMessage("Email deve ser válido");

        RuleFor(x => x.Senha)
            .NotEmpty().WithMessage("Senha é obrigatória");
    }
}

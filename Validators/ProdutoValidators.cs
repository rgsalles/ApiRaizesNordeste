using ApiRaizesNordeste.DTOs;
using FluentValidation;

namespace ApiRaizesNordeste.Validators;

/// <summary>
/// Validador para criação de produto
/// </summary>
public class CreateProdutoDtoValidator : AbstractValidator<CreateProdutoDto>
{
    public CreateProdutoDtoValidator()
    {
        RuleFor(x => x.Nome)
            .NotEmpty().WithMessage("Nome do produto é obrigatório")
            .MinimumLength(3).WithMessage("Nome deve ter pelo menos 3 caracteres")
            .MaximumLength(200).WithMessage("Nome não pode exceder 200 caracteres");

        RuleFor(x => x.Descricao)
            .MaximumLength(1000).WithMessage("Descrição não pode exceder 1000 caracteres");

        RuleFor(x => x.Preco)
            .NotEmpty().WithMessage("Preço é obrigatório")
            .GreaterThan(0).WithMessage("Preço deve ser maior que 0")
            .LessThanOrEqualTo(9999.99m).WithMessage("Preço não pode ser maior que R$ 9999,99");

        RuleFor(x => x.CategoriaId)
            .NotEmpty().WithMessage("Categoria é obrigatória");

        RuleFor(x => x.TempoPreparo)
            .GreaterThan(0).WithMessage("Tempo de preparo deve ser maior que 0")
            .LessThanOrEqualTo(180).WithMessage("Tempo de preparo não pode exceder 180 minutos");

        RuleFor(x => x.DataInicioSazonal)
            .Must((dto, value) => 
            {
                if (!dto.Sazonal) return true; // Não é obrigatório se não for sazonal
                return value != null && value > DateTime.Today;
            })
            .WithMessage("Data de início deve ser no futuro para produtos sazonais");

        RuleFor(x => x.DataFimSazonal)
            .Must((dto, value) =>
            {
                if (!dto.Sazonal) return true; // Não é obrigatório se não for sazonal
                return value != null && value > dto.DataInicioSazonal;
            })
            .WithMessage("Data de fim deve ser após data de início para produtos sazonais");
    }
}

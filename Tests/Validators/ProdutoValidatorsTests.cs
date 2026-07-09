using ApiRaizesNordeste.DTOs;
using ApiRaizesNordeste.Validators;
using FluentValidation.TestHelper;
using Xunit;

namespace ApiRaizesNordeste.Tests.Validators;

/// <summary>
/// Testes para validadores de produtos
/// </summary>
public class ProdutoValidatorsTests
{
    [Fact]
    public void CreateProdutoValidator_ComDadosValidos_DevePassar()
    {
        // Arrange
        var validator = new CreateProdutoDtoValidator();
        var dto = new CreateProdutoDto
        {
            Nome = "Tapioca Recheada",
            Descricao = "Tapioca com queijo e manteiga",
            Preco = 15.50m,
            CategoriaId = Guid.NewGuid(),
            TempoPreparo = 10,
            Sazonal = false
        };

        // Act
        var result = validator.TestValidate(dto);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void CreateProdutoValidator_ComPrecoZero_DeveComFalha()
    {
        // Arrange
        var validator = new CreateProdutoDtoValidator();
        var dto = new CreateProdutoDto
        {
            Nome = "Tapioca",
            Preco = 0,
            CategoriaId = Guid.NewGuid(),
            TempoPreparo = 10,
            Sazonal = false
        };

        // Act & Assert
        validator.TestValidate(dto)
            .ShouldHaveValidationErrorFor(x => x.Preco)
            .WithErrorMessage("Preço deve ser maior que 0");
    }

    [Fact]
    public void CreateProdutoValidator_ComTempoPreparoInvalido_DeveComFalha()
    {
        // Arrange
        var validator = new CreateProdutoDtoValidator();
        var dto = new CreateProdutoDto
        {
            Nome = "Tapioca",
            Preco = 15.50m,
            CategoriaId = Guid.NewGuid(),
            TempoPreparo = 200, // Maior que 180
            Sazonal = false
        };

        // Act & Assert
        validator.TestValidate(dto)
            .ShouldHaveValidationErrorFor(x => x.TempoPreparo);
    }

    [Fact]
    public void CreateProdutoValidator_ComNomeVazio_DeveComFalha()
    {
        // Arrange
        var validator = new CreateProdutoDtoValidator();
        var dto = new CreateProdutoDto
        {
            Nome = "",
            Preco = 15.50m,
            CategoriaId = Guid.NewGuid(),
            TempoPreparo = 10,
            Sazonal = false
        };

        // Act & Assert
        validator.TestValidate(dto)
            .ShouldHaveValidationErrorFor(x => x.Nome);
    }

    [Fact]
    public void CreateProdutoValidator_ComSazanalSemDatas_DeveComFalha()
    {
        // Arrange
        var validator = new CreateProdutoDtoValidator();
        var dto = new CreateProdutoDto
        {
            Nome = "Cuscuz Sazonal",
            Preco = 15.50m,
            CategoriaId = Guid.NewGuid(),
            TempoPreparo = 10,
            Sazonal = true,
            DataInicioSazonal = null, // Faltando data
            DataFimSazonal = null
        };

        // Act & Assert
        var result = validator.TestValidate(dto);
        result.ShouldHaveValidationErrorFor(x => x.DataInicioSazonal);
    }
}

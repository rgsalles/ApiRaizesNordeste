using ApiRaizesNordeste.DTOs;
using ApiRaizesNordeste.Validators;
using FluentValidation.TestHelper;
using Xunit;

namespace ApiRaizesNordeste.Tests.Validators;

/// <summary>
/// Testes para validadores de pedidos
/// </summary>
public class PedidoValidatorsTests
{
    [Fact]
    public void CreatePedidoValidator_ComDadosValidos_DevePassar()
    {
        // Arrange
        var validator = new CreatePedidoDtoValidator();
        var dto = new CreatePedidoDto
        {
            UnidadeId = Guid.NewGuid(),
            Canal = 1,
            Itens = new List<ItemPedidoDto>
            {
                new ItemPedidoDto
                {
                    ProdutoId = Guid.NewGuid(),
                    Quantidade = 2
                }
            }
        };

        // Act
        var result = validator.TestValidate(dto);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void CreatePedidoValidator_SemItens_DeveComFalha()
    {
        // Arrange
        var validator = new CreatePedidoDtoValidator();
        var dto = new CreatePedidoDto
        {
            UnidadeId = Guid.NewGuid(),
            Canal = 1,
            Itens = new List<ItemPedidoDto>()
        };

        // Act & Assert
        validator.TestValidate(dto)
            .ShouldHaveValidationErrorFor(x => x.Itens);
    }

    [Fact]
    public void CreatePedidoValidator_ComCanalInvalido_DeveComFalha()
    {
        // Arrange
        var validator = new CreatePedidoDtoValidator();
        var dto = new CreatePedidoDto
        {
            UnidadeId = Guid.NewGuid(),
            Canal = 99, // Canal inválido
            Itens = new List<ItemPedidoDto>
            {
                new ItemPedidoDto
                {
                    ProdutoId = Guid.NewGuid(),
                    Quantidade = 1
                }
            }
        };

        // Act & Assert
        validator.TestValidate(dto)
            .ShouldHaveValidationErrorFor(x => x.Canal);
    }

    [Fact]
    public void ItemPedidoValidator_ComQuantidadeZero_DeveComFalha()
    {
        // Arrange
        var validator = new ItemPedidoDtoValidator();
        var item = new ItemPedidoDto
        {
            ProdutoId = Guid.NewGuid(),
            Quantidade = 0
        };

        // Act & Assert
        validator.TestValidate(item)
            .ShouldHaveValidationErrorFor(x => x.Quantidade)
            .WithErrorMessage("Quantidade deve ser maior que 0");
    }

    [Fact]
    public void ItemPedidoValidator_ComQuantidadeExcessiva_DeveComFalha()
    {
        // Arrange
        var validator = new ItemPedidoDtoValidator();
        var item = new ItemPedidoDto
        {
            ProdutoId = Guid.NewGuid(),
            Quantidade = 1000 // Mais que 999
        };

        // Act & Assert
        validator.TestValidate(item)
            .ShouldHaveValidationErrorFor(x => x.Quantidade);
    }

    [Fact]
    public void CancelarPedidoValidator_ComMotivoValido_DevePassar()
    {
        // Arrange
        var validator = new CancelarPedidoDtoValidator();
        var dto = new CancelarPedidoDto { Motivo = "Cliente solicitou cancelamento" };

        // Act
        var result = validator.TestValidate(dto);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void CancelarPedidoValidator_SemMotivo_DeveComFalha()
    {
        // Arrange
        var validator = new CancelarPedidoDtoValidator();
        var dto = new CancelarPedidoDto { Motivo = "" };

        // Act & Assert
        validator.TestValidate(dto)
            .ShouldHaveValidationErrorFor(x => x.Motivo);
    }
}

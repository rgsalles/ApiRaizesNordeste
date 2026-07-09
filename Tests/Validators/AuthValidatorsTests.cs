using ApiRaizesNordeste.DTOs;
using ApiRaizesNordeste.Models;
using ApiRaizesNordeste.Validators;
using FluentValidation.TestHelper;
using Xunit;

namespace ApiRaizesNordeste.Tests.Validators;

/// <summary>
/// Testes para validadores de autenticação
/// </summary>
public class AuthValidatorsTests
{
    [Fact]
    public void RegisterValidator_ComEmailValido_DevePassar()
    {
        // Arrange
        var validator = new RegisterClienteDtoValidator();
        var dto = new RegisterClienteDto
        {
            Email = "teste@example.com",
            Senha = "Senha123",
            NomeCompleto = "João Silva",
            Telefone = "11999999999"
        };

        // Act
        var result = validator.TestValidate(dto);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void RegisterValidator_ComEmailInvalido_DeveComFalha()
    {
        // Arrange
        var validator = new RegisterClienteDtoValidator();
        var dto = new RegisterClienteDto
        {
            Email = "email-inválido",
            Senha = "Senha123",
            NomeCompleto = "João Silva",
            Telefone = "11999999999"
        };

        // Act & Assert
        validator.TestValidate(dto)
            .ShouldHaveValidationErrorFor(x => x.Email)
            .WithErrorMessage("Email deve ser válido");
    }

    [Fact]
    public void RegisterValidator_ComSenhaFraca_DeveComFalha()
    {
        // Arrange
        var validator = new RegisterClienteDtoValidator();
        var dto = new RegisterClienteDto
        {
            Email = "teste@example.com",
            Senha = "abc", // Muito curta e sem números/maiúsculas
            NomeCompleto = "João Silva",
            Telefone = "11999999999"
        };

        // Act & Assert
        var result = validator.TestValidate(dto);
        result.ShouldHaveValidationErrorFor(x => x.Senha);
    }

    [Fact]
    public void RegisterValidator_ComTelefoneInvalido_DeveComFalha()
    {
        // Arrange
        var validator = new RegisterClienteDtoValidator();
        var dto = new RegisterClienteDto
        {
            Email = "teste@example.com",
            Senha = "Senha123",
            NomeCompleto = "João Silva",
            Telefone = "123" // Telefone inválido
        };

        // Act & Assert
        validator.TestValidate(dto)
            .ShouldHaveValidationErrorFor(x => x.Telefone)
            .WithErrorMessage("Telefone deve ter 10 ou 11 dígitos");
    }

    [Fact]
    public void LoginValidator_ComDadosValidos_DevePassar()
    {
        // Arrange
        var validator = new LoginDtoValidator();
        var dto = new LoginDto
        {
            Email = "teste@example.com",
            Senha = "Senha123"
        };

        // Act
        var result = validator.TestValidate(dto);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void LoginValidator_SemEmail_DeveComFalha()
    {
        // Arrange
        var validator = new LoginDtoValidator();
        var dto = new LoginDto
        {
            Email = "",
            Senha = "Senha123"
        };

        // Act & Assert
        validator.TestValidate(dto)
            .ShouldHaveValidationErrorFor(x => x.Email);
    }
}

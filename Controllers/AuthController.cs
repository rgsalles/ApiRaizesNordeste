using ApiRaizesNordeste.DTOs;
using ApiRaizesNordeste.Services;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace ApiRaizesNordeste.Controllers;

/// <summary>
/// Controller para autenticacao e gerenciamento de clientes.
/// </summary>
[ApiController]
[AllowAnonymous]
[Route("api/v1/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IClienteService _clienteService;
    private readonly IValidator<RegisterClienteDto> _registerValidator;
    private readonly IValidator<LoginDto> _loginValidator;
    private readonly ILogger<AuthController> _logger;

    public AuthController(
        IClienteService clienteService,
        IValidator<RegisterClienteDto> registerValidator,
        IValidator<LoginDto> loginValidator,
        ILogger<AuthController> logger)
    {
        _clienteService = clienteService;
        _registerValidator = registerValidator;
        _loginValidator = loginValidator;
        _logger = logger;
    }

    /// <summary>
    /// Registrar novo cliente.
    /// </summary>
    [HttpPost("register")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<AuthResponseDto>> Register([FromBody] RegisterClienteDto dto)
    {
        try
        {
            var validationResult = await _registerValidator.ValidateAsync(dto);
            if (!validationResult.IsValid)
                return BadRequest(CriarRespostaValidacao(validationResult));

            var resultado = await _clienteService.RegistrarAsync(dto);
            return Ok(resultado);
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning("Erro ao registrar cliente: {Mensagem}", ex.Message);
            return BadRequest(new ErrorResponseDto(StatusCodes.Status400BadRequest, ex.Message));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro interno ao registrar cliente");
            return StatusCode(
                StatusCodes.Status500InternalServerError,
                new ErrorResponseDto(StatusCodes.Status500InternalServerError, "Erro interno do servidor"));
        }
    }

    /// <summary>
    /// Fazer login com email e senha.
    /// </summary>
    [HttpPost("login")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<AuthResponseDto>> Login([FromBody] LoginDto dto)
    {
        try
        {
            var validationResult = await _loginValidator.ValidateAsync(dto);
            if (!validationResult.IsValid)
                return BadRequest(CriarRespostaValidacao(validationResult));

            var resultado = await _clienteService.LoginAsync(dto);
            if (resultado == null)
            {
                return Unauthorized(new ErrorResponseDto(
                    StatusCodes.Status401Unauthorized,
                    "Email ou senha invalidos"));
            }

            return Ok(resultado);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao fazer login");
            return StatusCode(
                StatusCodes.Status500InternalServerError,
                new ErrorResponseDto(StatusCodes.Status500InternalServerError, "Erro interno do servidor"));
        }
    }

    /// <summary>
    /// Verificar se email ja existe.
    /// </summary>
    [HttpGet("email-existe")]
    public async Task<ActionResult<bool>> EmailExiste([FromQuery] string email)
    {
        var existe = await _clienteService.VerificarEmailExisteAsync(email);
        return Ok(existe);
    }

    private static ErrorResponseDto CriarRespostaValidacao(FluentValidation.Results.ValidationResult validationResult)
    {
        var errors = validationResult.Errors
            .GroupBy(e => e.PropertyName)
            .ToDictionary(g => g.Key, g => g.Select(e => e.ErrorMessage).ToArray());

        return new ErrorResponseDto(
            StatusCodes.Status400BadRequest,
            "Um ou mais erros de validacao ocorreram",
            errors);
    }
}

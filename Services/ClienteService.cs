using System.Security.Cryptography;
using System.Text;
using ApiRaizesNordeste.Data;
using ApiRaizesNordeste.DTOs;
using ApiRaizesNordeste.Models;
using ApiRaizesNordeste.Repositories;

namespace ApiRaizesNordeste.Services;

/// <summary>
/// Interface para serviço de autenticação e clientes
/// </summary>
public interface IClienteService
{
    Task<AuthResponseDto> RegistrarAsync(RegisterClienteDto dto);
    Task<AuthResponseDto?> LoginAsync(LoginDto dto);
    Task<ClienteDto?> GetPorIdAsync(Guid id);
    Task<bool> VerificarEmailExisteAsync(string email);
    Task<bool> VerificarSenhaAsync(string email, string senha);
}

/// <summary>
/// Implementação do serviço de clientes
/// </summary>
public class ClienteService : IClienteService
{
    private readonly ApplicationDbContext _context;
    private readonly IClienteRepository _clientes;
    private readonly IRepository<ClienteLoyalty> _clientesLoyalty;
    private readonly ILogger<ClienteService> _logger;
    private readonly IJwtTokenService _jwtTokenService;

    public ClienteService(
        ApplicationDbContext context,
        IClienteRepository clientes,
        IRepository<ClienteLoyalty> clientesLoyalty,
        ILogger<ClienteService> logger,
        IJwtTokenService jwtTokenService)
    {
        _context = context;
        _clientes = clientes;
        _clientesLoyalty = clientesLoyalty;
        _logger = logger;
        _jwtTokenService = jwtTokenService;
    }

    public async Task<AuthResponseDto> RegistrarAsync(RegisterClienteDto dto)
    {
        try
        {
            // Validar email único
            var emailExiste = await _clientes.EmailExisteAsync(dto.Email);
            if (emailExiste)
                throw new InvalidOperationException("Email já cadastrado");

            // Criar cliente
            var cliente = new Cliente
            {
                Email = dto.Email,
                SenhaHash = GerarHashSenha(dto.Senha),
                NomeCompleto = dto.NomeCompleto,
                Telefone = dto.Telefone,
                Ativo = true
            };

            await _clientes.AddAsync(cliente);

            // Criar dados de loyalty
            var loyalty = new ClienteLoyalty
            {
                ClienteId = cliente.Id,
                Nivel = NivelLoyalty.Bronze,
                DataIngresso = DateTime.UtcNow
            };

            await _clientesLoyalty.AddAsync(loyalty);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Cliente {Email} registrado com sucesso", cliente.Email);

            return CriarRespostaAutenticacao(cliente);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao registrar cliente {Email}", dto.Email);
            throw;
        }
    }

    public async Task<ClienteDto?> GetPorIdAsync(Guid id)
    {
        var cliente = await _clientes.GetByIdAsync(id);
        return cliente != null ? MapearParaDto(cliente) : null;
    }

    public async Task<AuthResponseDto?> LoginAsync(LoginDto dto)
    {
        var cliente = await _clientes.GetPorEmailAsync(dto.Email);
        if (cliente == null || !cliente.Ativo || !VerificarHashSenha(dto.Senha, cliente.SenhaHash))
            return null;

        cliente.DataUltimoLogin = DateTime.UtcNow;
        await _context.SaveChangesAsync();

        _logger.LogInformation("Cliente {Email} autenticado com sucesso", cliente.Email);
        return CriarRespostaAutenticacao(cliente);
    }

    public async Task<bool> VerificarEmailExisteAsync(string email)
    {
        return await _clientes.EmailExisteAsync(email);
    }

    public async Task<bool> VerificarSenhaAsync(string email, string senha)
    {
        var cliente = await _clientes.GetPorEmailAsync(email);
        if (cliente == null)
            return false;

        return VerificarHashSenha(senha, cliente.SenhaHash);
    }

    private string GerarHashSenha(string senha)
    {
        using (var sha256 = SHA256.Create())
        {
            var hash = sha256.ComputeHash(Encoding.UTF8.GetBytes(senha));
            return Convert.ToBase64String(hash);
        }
    }

    private bool VerificarHashSenha(string senha, string hash)
    {
        var hashDaSenha = GerarHashSenha(senha);
        return hashDaSenha == hash;
    }

    private AuthResponseDto CriarRespostaAutenticacao(Cliente cliente)
    {
        var jwt = _jwtTokenService.GerarToken(cliente);
        return new AuthResponseDto
        {
            ClienteId = cliente.Id,
            Email = cliente.Email,
            NomeCompleto = cliente.NomeCompleto,
            Token = jwt.Token,
            TokenExpira = jwt.ExpiresAt
        };
    }

    private ClienteDto MapearParaDto(Cliente cliente)
    {
        return new ClienteDto
        {
            Id = cliente.Id,
            Email = cliente.Email,
            NomeCompleto = cliente.NomeCompleto,
            Telefone = cliente.Telefone,
            EmailVerificado = cliente.EmailVerificado,
            Ativo = cliente.Ativo,
            DataUltimoLogin = cliente.DataUltimoLogin,
            DataCriacao = cliente.DataCriacao
        };
    }
}

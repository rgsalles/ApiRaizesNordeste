using ApiRaizesNordeste.Data;
using ApiRaizesNordeste.DTOs;
using ApiRaizesNordeste.Models;
using ApiRaizesNordeste.Repositories;

namespace ApiRaizesNordeste.Services;

/// <summary>
/// Interface para serviço de unidades
/// </summary>
public interface IUnidadeService
{
    Task<UnidadeDto?> GetPorIdAsync(Guid id);
    Task<List<UnidadeDto>> GetTodasAsync();
    Task<List<UnidadeDto>> GetPorCidadeAsync(string cidade);
    Task<List<UnidadeDto>> GetAtivasAsync();
    Task<UnidadeDto> CriarAsync(CreateUnidadeDto dto);
    Task<bool> AtualizarAsync(Guid id, CreateUnidadeDto dto);
    Task<bool> DeletarAsync(Guid id);
}

/// <summary>
/// Implementação do serviço de unidades
/// </summary>
public class UnidadeService : IUnidadeService
{
    private readonly ApplicationDbContext _context;
    private readonly IUnidadeRepository _unidades;
    private readonly ILogger<UnidadeService> _logger;

    public UnidadeService(
        ApplicationDbContext context,
        IUnidadeRepository unidades,
        ILogger<UnidadeService> logger)
    {
        _context = context;
        _unidades = unidades;
        _logger = logger;
    }

    public async Task<UnidadeDto?> GetPorIdAsync(Guid id)
    {
        var unidade = await _unidades.GetByIdAsync(id);
        return unidade != null ? MapearParaDto(unidade) : null;
    }

    public async Task<List<UnidadeDto>> GetTodasAsync()
    {
        var unidades = await _unidades.GetAllAsync();
        return unidades.Select(MapearParaDto).ToList();
    }

    public async Task<List<UnidadeDto>> GetPorCidadeAsync(string cidade)
    {
        var unidades = await _unidades.GetPorCidadeAsync(cidade);
        return unidades.Select(MapearParaDto).ToList();
    }

    public async Task<List<UnidadeDto>> GetAtivasAsync()
    {
        var unidades = await _unidades.GetAtivasAsync();
        return unidades.Select(MapearParaDto).ToList();
    }

    public async Task<UnidadeDto> CriarAsync(CreateUnidadeDto dto)
    {
        try
        {
            var unidade = new Unidade
            {
                Nome = dto.Nome,
                Localizacao = dto.Localizacao,
                Endereco = dto.Endereco,
                Cidade = dto.Cidade,
                Estado = dto.Estado,
                CEP = dto.CEP,
                Telefone = dto.Telefone,
                Email = dto.Email,
                HorarioAbertura = dto.HorarioAbertura,
                HorarioFechamento = dto.HorarioFechamento,
                Latitude = dto.Latitude,
                Longitude = dto.Longitude,
                Ativa = true
            };

            await _unidades.AddAsync(unidade);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Unidade {UnidadeNome} criada com sucesso", unidade.Nome);

            return MapearParaDto(unidade);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao criar unidade {UnidadeNome}", dto.Nome);
            throw;
        }
    }

    public async Task<bool> AtualizarAsync(Guid id, CreateUnidadeDto dto)
    {
        try
        {
            var unidade = await _unidades.GetByIdAsync(id);
            if (unidade == null)
                return false;

            unidade.Nome = dto.Nome;
            unidade.Localizacao = dto.Localizacao;
            unidade.Endereco = dto.Endereco;
            unidade.Cidade = dto.Cidade;
            unidade.Estado = dto.Estado;
            unidade.CEP = dto.CEP;
            unidade.Telefone = dto.Telefone;
            unidade.Email = dto.Email;
            unidade.HorarioAbertura = dto.HorarioAbertura;
            unidade.HorarioFechamento = dto.HorarioFechamento;
            unidade.Latitude = dto.Latitude;
            unidade.Longitude = dto.Longitude;
            unidade.DataAtualizacao = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            _logger.LogInformation("Unidade {UnidadeId} atualizada com sucesso", id);

            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao atualizar unidade {UnidadeId}", id);
            return false;
        }
    }

    public async Task<bool> DeletarAsync(Guid id)
    {
        try
        {
            var unidade = await _unidades.GetByIdAsync(id);
            if (unidade == null)
                return false;

            unidade.Deletado = true;
            unidade.Ativa = false;
            unidade.DataAtualizacao = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            _logger.LogInformation("Unidade {UnidadeId} deletada (soft delete)", id);

            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao deletar unidade {UnidadeId}", id);
            return false;
        }
    }

    private UnidadeDto MapearParaDto(Unidade unidade)
    {
        return new UnidadeDto
        {
            Id = unidade.Id,
            Nome = unidade.Nome,
            Localizacao = unidade.Localizacao,
            Endereco = unidade.Endereco,
            Cidade = unidade.Cidade,
            Estado = unidade.Estado,
            CEP = unidade.CEP,
            Telefone = unidade.Telefone,
            Email = unidade.Email,
            HorarioAbertura = unidade.HorarioAbertura,
            HorarioFechamento = unidade.HorarioFechamento,
            Ativa = unidade.Ativa,
            Latitude = unidade.Latitude,
            Longitude = unidade.Longitude,
            DataCriacao = unidade.DataCriacao
        };
    }
}

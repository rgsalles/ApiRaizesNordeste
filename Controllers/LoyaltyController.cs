using ApiRaizesNordeste.DTOs;
using ApiRaizesNordeste.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace ApiRaizesNordeste.Controllers;

/// <summary>
/// Controller para programa de fidelização
/// </summary>
[ApiController]
[Authorize]
[Route("api/v1/[controller]")]
public class LoyaltyController : ControllerBase
{
    private readonly ILoyaltyService _loyaltyService;
    private readonly ILogger<LoyaltyController> _logger;
    public LoyaltyController(ILoyaltyService loyaltyService, ILogger<LoyaltyController> logger)
    {
        _loyaltyService = loyaltyService;
        _logger = logger;
    }

    /// <summary>
    /// Obter saldo de pontos do cliente
    /// </summary>
    /// <returns>Dados de fidelização</returns>
    [HttpGet("saldo")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ClienteLoyaltyDto>> GetSaldo()
    {
        if (!TryGetClienteId(out var clienteId))
            return BadRequest(new { erro = "Cliente ID invalido" });

        var loyalty = await _loyaltyService.GetSaldoAsync(clienteId);
        if (loyalty == null)
            return NotFound(new { erro = "Dados de fidelização não encontrados" });

        return Ok(loyalty);
    }

    /// <summary>
    /// Obter histórico de transações de pontos
    /// </summary>
    /// <returns>Lista de transações</returns>
    [HttpGet("historico")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<List<TransacaoLoyaltyDto>>> GetHistorico()
    {
        if (!TryGetClienteId(out var clienteId))
            return BadRequest(new { erro = "Cliente ID invalido" });

        var transacoes = await _loyaltyService.GetHistoricoAsync(clienteId);
        return Ok(transacoes);
    }

    /// <summary>
    /// Resgatar pontos de fidelização
    /// </summary>
    /// <param name="dto">Quantidade de pontos a resgatar</param>
    /// <returns>Resultado da operação</returns>
    [HttpPost("resgate")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Resgatar([FromBody] ResgatePointsDto dto)
    {
        if (!TryGetClienteId(out var clienteId))
            return BadRequest(new { erro = "Cliente ID invalido" });

        try
        {
            var resultado = await _loyaltyService.ResgataePontosAsync(
                clienteId,
                dto.Pontos,
                dto.Descricao ?? "Resgate de pontos");

            if (!resultado)
                return BadRequest(new { erro = "Não foi possível resgatar os pontos" });

            return Ok(new { mensagem = "Pontos resgatados com sucesso" });
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning("Erro ao resgatar pontos: {Mensagem}", ex.Message);
            return BadRequest(new { erro = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao resgatar pontos");
            return StatusCode(500, new { erro = "Erro interno do servidor" });
        }
    }

    /// <summary>
    /// Adicionar pontos manualmente (admin/gerente)
    /// </summary>
    /// <param name="clienteId">ID do cliente</param>
    /// <param name="dto">Quantidade de pontos e motivo</param>
    /// <returns>Novo saldo</returns>
    [HttpPost("{clienteId:guid}/adicionar-pontos")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<object>> AdicionarPontos(
        [FromRoute] Guid clienteId,
        [FromBody] ResgatePointsDto dto)
    {
        try
        {
            var novoSaldo = await _loyaltyService.AdicionarPontosAsync(
                clienteId,
                dto.Pontos,
                dto.Descricao ?? "Adição de pontos");

            return Ok(new { mensagem = "Pontos adicionados com sucesso", novoSaldo });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { erro = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao adicionar pontos");
            return StatusCode(500, new { erro = "Erro interno do servidor" });
        }
    }

    /// <summary>
    /// Atualizar nível de fidelização do cliente
    /// </summary>
    /// <returns>Resultado da operação</returns>
    [HttpPost("atualizar-nivel")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> AtualizarNivel()
    {
        if (!TryGetClienteId(out var clienteId))
            return BadRequest(new { erro = "Cliente ID invalido" });

        try
        {
            await _loyaltyService.AtualizarNivelAsync(clienteId);
            return Ok(new { mensagem = "Nível atualizado com sucesso" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao atualizar nível");
            return StatusCode(500, new { erro = "Erro interno do servidor" });
        }
    }

    private bool TryGetClienteId(out Guid clienteId)
    {
        return Guid.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out clienteId);
    }
}

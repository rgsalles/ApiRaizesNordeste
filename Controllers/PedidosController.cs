using ApiRaizesNordeste.DTOs;
using ApiRaizesNordeste.Models;
using ApiRaizesNordeste.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace ApiRaizesNordeste.Controllers;

/// <summary>
/// Controller para gerenciamento de pedidos
/// </summary>
[ApiController]
[Authorize]
[Route("api/v1/[controller]")]
public class PedidosController : ControllerBase
{
    private readonly IPedidoService _pedidoService;
    private readonly ILogger<PedidosController> _logger;
    public PedidosController(IPedidoService pedidoService, ILogger<PedidosController> logger)
    {
        _pedidoService = pedidoService;
        _logger = logger;
    }

    /// <summary>
    /// Criar novo pedido
    /// </summary>
    /// <param name="dto">Dados do pedido</param>
    /// <returns>Pedido criado</returns>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<PedidoDto>> Criar([FromBody] CreatePedidoDto dto)
    {
        try
        {
            if (!TryGetClienteId(out var clienteId))
                return BadRequest(new { erro = "Cliente ID invalido" });

            var pedido = await _pedidoService.CriarPedidoAsync(dto, clienteId);
            return CreatedAtAction(nameof(GetPorId), new { id = pedido.Id }, pedido);
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning("Erro ao criar pedido: {Mensagem}", ex.Message);
            return BadRequest(new { erro = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao criar pedido");
            return StatusCode(500, new { erro = "Erro interno do servidor" });
        }
    }

    /// <summary>
    /// Obter pedido por ID
    /// </summary>
    /// <param name="id">ID do pedido</param>
    /// <returns>Dados do pedido</returns>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<PedidoDto>> GetPorId([FromRoute] Guid id)
    {
        var pedido = await _pedidoService.GetPorIdAsync(id);
        if (pedido == null)
            return NotFound(new { erro = "Pedido não encontrado" });

        return Ok(pedido);
    }

    /// <summary>
    /// Obter todos os pedidos do cliente
    /// </summary>
    /// <returns>Lista de pedidos</returns>
    [HttpGet("meus-pedidos")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<List<PedidoDto>>> GetMeusPedidos()
    {
        if (!TryGetClienteId(out var clienteId))
            return BadRequest(new { erro = "Cliente ID invalido" });

        var pedidos = await _pedidoService.GetPorClienteAsync(clienteId);
        return Ok(pedidos);
    }

    /// <summary>
    /// Obter pedidos pendentes de uma unidade
    /// </summary>
    /// <param name="unidadeId">ID da unidade</param>
    /// <returns>Lista de pedidos pendentes</returns>
    [HttpGet("pendentes/{unidadeId:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<List<PedidoDto>>> GetPendentes([FromRoute] Guid unidadeId)
    {
        var pedidos = await _pedidoService.GetPendentesAsync(unidadeId);
        return Ok(pedidos);
    }

    /// <summary>
    /// Atualizar status do pedido
    /// </summary>
    /// <param name="id">ID do pedido</param>
    /// <param name="dto">Novo status e motivo</param>
    /// <returns>Resultado da operação</returns>
    [HttpPatch("{id:guid}/status")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> AtualizarStatus([FromRoute] Guid id, [FromBody] AtualizarStatusPedidoDto dto)
    {
        var resultado = await _pedidoService.AtualizarStatusAsync(
            id,
            (StatusPedido)dto.NovoStatus,
            dto.Motivo);

        if (!resultado)
            return NotFound(new { erro = "Pedido não encontrado" });

        return Ok(new { mensagem = "Status do pedido atualizado com sucesso" });
    }

    /// <summary>
    /// Cancelar pedido
    /// </summary>
    /// <param name="id">ID do pedido</param>
    /// <param name="dto">Motivo do cancelamento</param>
    /// <returns>Resultado da operação</returns>
    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Cancelar([FromRoute] Guid id, [FromBody] CancelarPedidoDto dto)
    {
        var resultado = await _pedidoService.CancelarAsync(id, dto.Motivo);
        if (!resultado)
            return NotFound(new { erro = "Pedido não encontrado" });

        return Ok(new { mensagem = "Pedido cancelado com sucesso" });
    }

    /// <summary>
    /// Validar disponibilidade de produto
    /// </summary>
    /// <param name="produtoId">ID do produto</param>
    /// <param name="unidadeId">ID da unidade</param>
    /// <returns>True se disponível, false caso contrário</returns>
    [HttpGet("validar-disponibilidade")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<bool>> ValidarDisponibilidade(
        [FromQuery] Guid produtoId,
        [FromQuery] Guid unidadeId)
    {
        var disponivel = await _pedidoService.ValidarDisponibilidadeAsync(produtoId, unidadeId);
        return Ok(disponivel);
    }

    private bool TryGetClienteId(out Guid clienteId)
    {
        return Guid.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out clienteId);
    }
}

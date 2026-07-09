using ApiRaizesNordeste.DTOs;
using ApiRaizesNordeste.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace ApiRaizesNordeste.Controllers;

/// <summary>
/// Controller para gerenciamento de unidades (lojas/franquias)
/// </summary>
[ApiController]
[Route("api/v1/[controller]")]
public class UnidadesController : ControllerBase
{
    private readonly IUnidadeService _unidadeService;
    private readonly ILogger<UnidadesController> _logger;

    public UnidadesController(IUnidadeService unidadeService, ILogger<UnidadesController> logger)
    {
        _unidadeService = unidadeService;
        _logger = logger;
    }

    /// <summary>
    /// Obter todas as unidades ativas
    /// </summary>
    /// <returns>Lista de unidades</returns>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<List<UnidadeDto>>> GetTodas()
    {
        var unidades = await _unidadeService.GetAtivasAsync();
        return Ok(unidades);
    }

    /// <summary>
    /// Obter unidade por ID
    /// </summary>
    /// <param name="id">ID da unidade</param>
    /// <returns>Dados da unidade</returns>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<UnidadeDto>> GetPorId([FromRoute] Guid id)
    {
        var unidade = await _unidadeService.GetPorIdAsync(id);
        if (unidade == null)
            return NotFound(new { erro = "Unidade não encontrada" });

        return Ok(unidade);
    }

    /// <summary>
    /// Buscar unidades por cidade
    /// </summary>
    /// <param name="cidade">Nome da cidade</param>
    /// <returns>Lista de unidades</returns>
    [HttpGet("por-cidade/{cidade}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<List<UnidadeDto>>> GetPorCidade([FromRoute] string cidade)
    {
        var unidades = await _unidadeService.GetPorCidadeAsync(cidade);
        return Ok(unidades);
    }

    /// <summary>
    /// Criar nova unidade
    /// </summary>
    /// <param name="dto">Dados da unidade</param>
    /// <returns>Unidade criada</returns>
    [HttpPost]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<UnidadeDto>> Criar([FromBody] CreateUnidadeDto dto)
    {
        try
        {
            var unidade = await _unidadeService.CriarAsync(dto);
            return CreatedAtAction(nameof(GetPorId), new { id = unidade.Id }, unidade);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao criar unidade");
            return BadRequest(new { erro = ex.Message });
        }
    }

    /// <summary>
    /// Atualizar unidade
    /// </summary>
    /// <param name="id">ID da unidade</param>
    /// <param name="dto">Dados atualizados</param>
    /// <returns>Resultado da operação</returns>
    [HttpPut("{id:guid}")]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Atualizar([FromRoute] Guid id, [FromBody] CreateUnidadeDto dto)
    {
        var resultado = await _unidadeService.AtualizarAsync(id, dto);
        if (!resultado)
            return NotFound(new { erro = "Unidade não encontrada" });

        return Ok(new { mensagem = "Unidade atualizada com sucesso" });
    }

    /// <summary>
    /// Deletar (desativar) unidade
    /// </summary>
    /// <param name="id">ID da unidade</param>
    /// <returns>Resultado da operação</returns>
    [HttpDelete("{id:guid}")]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Deletar([FromRoute] Guid id)
    {
        var resultado = await _unidadeService.DeletarAsync(id);
        if (!resultado)
            return NotFound(new { erro = "Unidade não encontrada" });

        return Ok(new { mensagem = "Unidade deletada com sucesso" });
    }
}

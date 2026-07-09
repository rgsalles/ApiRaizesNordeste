using ApiRaizesNordeste.DTOs;
using ApiRaizesNordeste.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace ApiRaizesNordeste.Controllers;

/// <summary>
/// Controller para gerenciamento de produtos e cardápio
/// </summary>
[ApiController]
[Route("api/v1/[controller]")]
public class ProdutosController : ControllerBase
{
    private readonly IProdutoService _produtoService;
    private readonly ILogger<ProdutosController> _logger;

    public ProdutosController(IProdutoService produtoService, ILogger<ProdutosController> logger)
    {
        _produtoService = produtoService;
        _logger = logger;
    }

    /// <summary>
    /// Obter todos os produtos ativos
    /// </summary>
    /// <returns>Lista de produtos</returns>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<List<ProdutoDto>>> GetTodos()
    {
        var produtos = await _produtoService.GetTodosAtivosAsync();
        return Ok(produtos);
    }

    /// <summary>
    /// Obter produto por ID
    /// </summary>
    /// <param name="id">ID do produto</param>
    /// <returns>Dados do produto</returns>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ProdutoDto>> GetPorId([FromRoute] Guid id)
    {
        var produto = await _produtoService.GetPorIdAsync(id);
        if (produto == null)
            return NotFound(new { erro = "Produto não encontrado" });

        return Ok(produto);
    }

    /// <summary>
    /// Obter produtos por categoria
    /// </summary>
    /// <param name="categoriaId">ID da categoria</param>
    /// <returns>Lista de produtos</returns>
    [HttpGet("por-categoria/{categoriaId:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<List<ProdutoDto>>> GetPorCategoria([FromRoute] Guid categoriaId)
    {
        var produtos = await _produtoService.GetPorCategoriaAsync(categoriaId);
        return Ok(produtos);
    }

    /// <summary>
    /// Obter cardápio de uma unidade (produtos com estoque disponível)
    /// </summary>
    /// <param name="unidadeId">ID da unidade</param>
    /// <returns>Cardápio da unidade</returns>
    [HttpGet("cardapio/{unidadeId:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<List<ProdutoDto>>> GetCardapioPorUnidade([FromRoute] Guid unidadeId)
    {
        var produtos = await _produtoService.GetPorUnidadeAsync(unidadeId);
        return Ok(produtos);
    }

    /// <summary>
    /// Criar novo produto
    /// </summary>
    /// <param name="dto">Dados do produto</param>
    /// <returns>Produto criado</returns>
    [HttpPost]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ProdutoDto>> Criar([FromBody] CreateProdutoDto dto)
    {
        try
        {
            var produto = await _produtoService.CriarAsync(dto);
            return CreatedAtAction(nameof(GetPorId), new { id = produto.Id }, produto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao criar produto");
            return BadRequest(new { erro = ex.Message });
        }
    }

    /// <summary>
    /// Atualizar produto
    /// </summary>
    /// <param name="id">ID do produto</param>
    /// <param name="dto">Dados atualizados</param>
    /// <returns>Resultado da operação</returns>
    [HttpPut("{id:guid}")]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Atualizar([FromRoute] Guid id, [FromBody] CreateProdutoDto dto)
    {
        var resultado = await _produtoService.AtualizarAsync(id, dto);
        if (!resultado)
            return NotFound(new { erro = "Produto não encontrado" });

        return Ok(new { mensagem = "Produto atualizado com sucesso" });
    }

    /// <summary>
    /// Deletar (desativar) produto
    /// </summary>
    /// <param name="id">ID do produto</param>
    /// <returns>Resultado da operação</returns>
    [HttpDelete("{id:guid}")]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Deletar([FromRoute] Guid id)
    {
        var resultado = await _produtoService.DeletarAsync(id);
        if (!resultado)
            return NotFound(new { erro = "Produto não encontrado" });

        return Ok(new { mensagem = "Produto deletado com sucesso" });
    }
}

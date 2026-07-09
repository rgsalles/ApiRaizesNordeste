using ApiRaizesNordeste.DTOs;
using ApiRaizesNordeste.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace ApiRaizesNordeste.Controllers;

/// <summary>
/// Relatórios gerenciais da operação.
/// </summary>
[ApiController]
[Authorize]
[Route("api/v1/relatorios")]
public class RelatoriosController : ControllerBase
{
    private readonly IRelatorioService _relatorioService;

    public RelatoriosController(IRelatorioService relatorioService)
    {
        _relatorioService = relatorioService;
    }

    [HttpGet("vendas/resumo")]
    public async Task<ActionResult<ResumoVendasDto>> ObterResumoVendas(
        [FromQuery] DateTime? dataInicio,
        [FromQuery] DateTime? dataFim,
        [FromQuery] Guid? unidadeId)
    {
        var periodo = ObterPeriodo(dataInicio, dataFim);
        if (periodo.Erro != null)
            return BadRequest(new { erro = periodo.Erro });

        return Ok(await _relatorioService.ObterResumoVendasAsync(
            periodo.Inicio, periodo.Fim, unidadeId));
    }

    [HttpGet("produtos/mais-vendidos")]
    public async Task<ActionResult<List<ProdutoMaisVendidoDto>>> ObterProdutosMaisVendidos(
        [FromQuery] DateTime? dataInicio,
        [FromQuery] DateTime? dataFim,
        [FromQuery] Guid? unidadeId,
        [FromQuery] int limite = 10)
    {
        var periodo = ObterPeriodo(dataInicio, dataFim);
        if (periodo.Erro != null)
            return BadRequest(new { erro = periodo.Erro });
        if (limite is < 1 or > 100)
            return BadRequest(new { erro = "O limite deve estar entre 1 e 100." });

        return Ok(await _relatorioService.ObterProdutosMaisVendidosAsync(
            periodo.Inicio, periodo.Fim, unidadeId, limite));
    }

    [HttpGet("clientes/maiores-compradores")]
    public async Task<ActionResult<List<ClienteComprasDto>>> ObterMelhoresClientes(
        [FromQuery] DateTime? dataInicio,
        [FromQuery] DateTime? dataFim,
        [FromQuery] Guid? unidadeId,
        [FromQuery] int limite = 10)
    {
        var periodo = ObterPeriodo(dataInicio, dataFim);
        if (periodo.Erro != null)
            return BadRequest(new { erro = periodo.Erro });
        if (limite is < 1 or > 100)
            return BadRequest(new { erro = "O limite deve estar entre 1 e 100." });

        return Ok(await _relatorioService.ObterMelhoresClientesAsync(
            periodo.Inicio, periodo.Fim, unidadeId, limite));
    }

    [HttpGet("estoque/baixo")]
    public async Task<ActionResult<List<EstoqueBaixoDto>>> ObterEstoqueBaixo(
        [FromQuery] Guid? unidadeId)
    {
        return Ok(await _relatorioService.ObterEstoqueBaixoAsync(unidadeId));
    }

    private static (DateTime Inicio, DateTime Fim, string? Erro) ObterPeriodo(
        DateTime? dataInicio, DateTime? dataFim)
    {
        var fim = (dataFim ?? DateTime.UtcNow).Date;
        var inicio = (dataInicio ?? fim.AddDays(-29)).Date;

        if (inicio > fim)
            return (inicio, fim, "A data inicial não pode ser posterior à data final.");

        return (inicio, fim, null);
    }
}

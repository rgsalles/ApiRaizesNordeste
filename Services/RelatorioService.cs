using ApiRaizesNordeste.Data;
using ApiRaizesNordeste.DTOs;
using ApiRaizesNordeste.Models;
using Microsoft.EntityFrameworkCore;

namespace ApiRaizesNordeste.Services;

public interface IRelatorioService
{
    Task<ResumoVendasDto> ObterResumoVendasAsync(
        DateTime dataInicio, DateTime dataFim, Guid? unidadeId = null);

    Task<List<ProdutoMaisVendidoDto>> ObterProdutosMaisVendidosAsync(
        DateTime dataInicio, DateTime dataFim, Guid? unidadeId = null, int limite = 10);

    Task<List<ClienteComprasDto>> ObterMelhoresClientesAsync(
        DateTime dataInicio, DateTime dataFim, Guid? unidadeId = null, int limite = 10);

    Task<List<EstoqueBaixoDto>> ObterEstoqueBaixoAsync(Guid? unidadeId = null);
}

public class RelatorioService : IRelatorioService
{
    private readonly ApplicationDbContext _context;

    public RelatorioService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<ResumoVendasDto> ObterResumoVendasAsync(
        DateTime dataInicio, DateTime dataFim, Guid? unidadeId = null)
    {
        var inicio = dataInicio.Date;
        var fimExclusivo = dataFim.Date.AddDays(1);
        var pedidos = FiltrarPedidos(inicio, fimExclusivo, unidadeId);

        var totalPedidos = await pedidos.CountAsync();
        var pedidosEntregues = pedidos.Where(p => p.Status == StatusPedido.Entregue);
        var totalEntregues = await pedidosEntregues.CountAsync();
        var faturamento = await pedidosEntregues.SumAsync(p => (decimal?)p.ValorFinal) ?? 0;
        var descontos = await pedidosEntregues.SumAsync(p => (decimal?)p.Desconto) ?? 0;

        var porStatus = await pedidos
            .GroupBy(p => p.Status)
            .Select(g => new { Descricao = g.Key, Quantidade = g.Count() })
            .ToListAsync();

        var porCanal = await pedidos
            .GroupBy(p => p.Canal)
            .Select(g => new { Descricao = g.Key, Quantidade = g.Count() })
            .ToListAsync();

        return new ResumoVendasDto
        {
            DataInicio = inicio,
            DataFim = dataFim.Date,
            UnidadeId = unidadeId,
            TotalPedidos = totalPedidos,
            PedidosEntregues = totalEntregues,
            PedidosCancelados = await pedidos.CountAsync(p => p.Status == StatusPedido.Cancelado),
            Faturamento = faturamento,
            TotalDescontos = descontos,
            TicketMedio = totalEntregues == 0 ? 0 : decimal.Round(faturamento / totalEntregues, 2),
            PedidosPorStatus = porStatus.Select(x => new QuantidadePorDescricaoDto
            {
                Descricao = x.Descricao.ToString(),
                Quantidade = x.Quantidade
            }).ToList(),
            PedidosPorCanal = porCanal.Select(x => new QuantidadePorDescricaoDto
            {
                Descricao = x.Descricao.ToString(),
                Quantidade = x.Quantidade
            }).ToList()
        };
    }

    public Task<List<ProdutoMaisVendidoDto>> ObterProdutosMaisVendidosAsync(
        DateTime dataInicio, DateTime dataFim, Guid? unidadeId = null, int limite = 10)
    {
        var pedidos = FiltrarPedidos(dataInicio.Date, dataFim.Date.AddDays(1), unidadeId)
            .Where(p => p.Status == StatusPedido.Entregue);

        return _context.ItensPedidos
            .AsNoTracking()
            .Where(i => pedidos.Any(p => p.Id == i.PedidoId))
            .GroupBy(i => new { i.ProdutoId, Produto = i.Produto!.Nome })
            .Select(g => new ProdutoMaisVendidoDto
            {
                ProdutoId = g.Key.ProdutoId,
                Produto = g.Key.Produto,
                QuantidadeVendida = g.Sum(i => i.Quantidade),
                Faturamento = g.Sum(i => i.Subtotal)
            })
            .OrderByDescending(x => x.QuantidadeVendida)
            .ThenBy(x => x.Produto)
            .Take(limite)
            .ToListAsync();
    }

    public Task<List<ClienteComprasDto>> ObterMelhoresClientesAsync(
        DateTime dataInicio, DateTime dataFim, Guid? unidadeId = null, int limite = 10)
    {
        return FiltrarPedidos(dataInicio.Date, dataFim.Date.AddDays(1), unidadeId)
            .Where(p => p.Status == StatusPedido.Entregue)
            .GroupBy(p => new { p.ClienteId, Cliente = p.Cliente!.NomeCompleto })
            .Select(g => new ClienteComprasDto
            {
                ClienteId = g.Key.ClienteId,
                Cliente = g.Key.Cliente,
                QuantidadePedidos = g.Count(),
                TotalCompras = g.Sum(p => p.ValorFinal)
            })
            .OrderByDescending(x => x.TotalCompras)
            .ThenBy(x => x.Cliente)
            .Take(limite)
            .ToListAsync();
    }

    public Task<List<EstoqueBaixoDto>> ObterEstoqueBaixoAsync(Guid? unidadeId = null)
    {
        var estoques = _context.EstoquesProduto
            .AsNoTracking()
            .Where(e => e.Quantidade <= e.QuantidadeMinima);

        if (unidadeId.HasValue)
            estoques = estoques.Where(e => e.UnidadeId == unidadeId.Value);

        return estoques
            .Select(e => new EstoqueBaixoDto
            {
                UnidadeId = e.UnidadeId,
                Unidade = e.Unidade!.Nome,
                ProdutoId = e.ProdutoId,
                Produto = e.Produto!.Nome,
                Quantidade = e.Quantidade,
                QuantidadeMinima = e.QuantidadeMinima
            })
            .OrderBy(x => x.Unidade)
            .ThenBy(x => x.Quantidade)
            .ToListAsync();
    }

    private IQueryable<Pedido> FiltrarPedidos(
        DateTime inicio, DateTime fimExclusivo, Guid? unidadeId)
    {
        var pedidos = _context.Pedidos
            .AsNoTracking()
            .Where(p => p.DataCriacao >= inicio && p.DataCriacao < fimExclusivo);

        if (unidadeId.HasValue)
            pedidos = pedidos.Where(p => p.UnidadeId == unidadeId.Value);

        return pedidos;
    }
}

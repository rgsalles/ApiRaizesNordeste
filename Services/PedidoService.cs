using ApiRaizesNordeste.Data;
using ApiRaizesNordeste.DTOs;
using ApiRaizesNordeste.Models;
using ApiRaizesNordeste.Repositories;
using Microsoft.EntityFrameworkCore;

namespace ApiRaizesNordeste.Services;

/// <summary>
/// Implementação do serviço de pedidos
/// </summary>
public class PedidoService : IPedidoService
{
    private readonly ApplicationDbContext _context;
    private readonly IPedidoRepository _pedidos;
    private readonly IClienteRepository _clientes;
    private readonly IUnidadeRepository _unidades;
    private readonly IRepository<Produto> _produtos;
    private readonly IRepository<EstoqueProduto> _estoques;
    private readonly IRepository<ClienteLoyalty> _clientesLoyalty;
    private readonly IRepository<AuditoriaOperacao> _auditorias;
    private readonly ILogger<PedidoService> _logger;

    public PedidoService(
        ApplicationDbContext context,
        IPedidoRepository pedidos,
        IClienteRepository clientes,
        IUnidadeRepository unidades,
        IRepository<Produto> produtos,
        IRepository<EstoqueProduto> estoques,
        IRepository<ClienteLoyalty> clientesLoyalty,
        IRepository<AuditoriaOperacao> auditorias,
        ILogger<PedidoService> logger)
    {
        _context = context;
        _pedidos = pedidos;
        _clientes = clientes;
        _unidades = unidades;
        _produtos = produtos;
        _estoques = estoques;
        _clientesLoyalty = clientesLoyalty;
        _auditorias = auditorias;
        _logger = logger;
    }

    public async Task<PedidoDto> CriarPedidoAsync(CreatePedidoDto dto, Guid clienteId)
    {
        try
        {
            // Validar cliente
            var cliente = await _clientes.GetByIdAsync(clienteId);
            if (cliente == null)
                throw new InvalidOperationException("Cliente não encontrado");

            // Validar unidade
            var unidade = await _unidades.GetByIdAsync(dto.UnidadeId);
            if (unidade == null)
                throw new InvalidOperationException("Unidade não encontrada");

            if (!unidade.Ativa)
                throw new InvalidOperationException("Unidade inativa");

            // Validar itens
            if (dto.Itens == null || !dto.Itens.Any())
                throw new InvalidOperationException("Pedido deve conter itens");

            decimal valorTotal = 0;
            var pedido = new Pedido
            {
                ClienteId = clienteId,
                UnidadeId = dto.UnidadeId,
                Canal = (CanalPedido)dto.Canal,
                NumeroPedido = GerarNumeroPedido(),
                Status = StatusPedido.Pendente,
                Observacoes = dto.Observacoes
            };

            // Processar itens
            foreach (var itemDto in dto.Itens)
            {
                var produto = await _produtos.GetByIdAsync(itemDto.ProdutoId);
                if (produto == null)
                    throw new InvalidOperationException($"Produto {itemDto.ProdutoId} não encontrado");

                if (!produto.Ativo)
                    throw new InvalidOperationException($"Produto {produto.Nome} inativo");

                // Validar estoque
                var estoque = await _estoques.FirstOrDefaultAsync(
                    e => e.ProdutoId == itemDto.ProdutoId && e.UnidadeId == dto.UnidadeId);

                if (estoque == null || estoque.Quantidade < itemDto.Quantidade)
                    throw new InvalidOperationException($"Estoque insuficiente para {produto.Nome}");

                var subtotal = produto.Preco * itemDto.Quantidade;
                valorTotal += subtotal;

                pedido.Itens.Add(new ItemPedido
                {
                    ProdutoId = itemDto.ProdutoId,
                    Quantidade = itemDto.Quantidade,
                    PrecoUnitario = produto.Preco,
                    Subtotal = subtotal,
                    Observacoes = itemDto.Observacoes
                });
            }

            pedido.ValorTotal = valorTotal;
            pedido.Desconto = 0;

            // Aplicar pontos de fidelização se houver
            if (dto.PontosUtilizados.HasValue && dto.PontosUtilizados > 0)
            {
                var loyalty = await _clientesLoyalty.FirstOrDefaultAsync(l => l.ClienteId == clienteId);
                if (loyalty != null && loyalty.SaldoPontos >= dto.PontosUtilizados)
                {
                    decimal desconto = dto.PontosUtilizados.Value * 0.1m; // 1 ponto = R$ 0.10
                    pedido.Desconto = desconto;
                    pedido.PontosUtilizados = dto.PontosUtilizados;
                }
            }

            pedido.ValorFinal = pedido.ValorTotal - pedido.Desconto;

            // Salvar pedido
            await _pedidos.AddAsync(pedido);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Pedido {NumeroPedido} criado com sucesso para cliente {ClienteId}", 
                pedido.NumeroPedido, clienteId);

            return MapearParaDto(pedido);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao criar pedido para cliente {ClienteId}", clienteId);
            throw;
        }
    }

    public async Task<PedidoDto?> GetPorIdAsync(Guid pedidoId)
    {
        var pedido = await _pedidos.GetComItensAsync(pedidoId);
        return pedido != null ? MapearParaDto(pedido) : null;
    }

    public async Task<List<PedidoDto>> GetPorClienteAsync(Guid clienteId)
    {
        var pedidos = await _pedidos.GetPorClienteAsync(clienteId);
        return pedidos.Select(MapearParaDto).ToList();
    }

    public async Task<List<PedidoDto>> GetPendentesAsync(Guid unidadeId)
    {
        var pedidos = await _pedidos.GetPendentesAsync(unidadeId);
        return pedidos
            .Select(MapearParaDto)
            .ToList();
    }

    public async Task<bool> AtualizarStatusAsync(Guid pedidoId, StatusPedido novoStatus, string? motivo = null)
    {
        try
        {
            var pedido = await _pedidos.GetByIdAsync(pedidoId);
            if (pedido == null)
                throw new InvalidOperationException("Pedido não encontrado");

            var statusAnterior = pedido.Status;
            pedido.Status = novoStatus;
            pedido.DataAtualizacao = DateTime.UtcNow;

            // Registrar auditoria
            var auditoria = new AuditoriaOperacao
            {
                Tipo = TipoOperacaoAuditoria.CancelamentoPedido,
                EntidadeAfetada = "Pedido",
                IdEntidade = pedidoId,
                UsuarioExecucao = "Sistema",
                Descricao = $"Status alterado de {statusAnterior} para {novoStatus}",
                DadosAnteriores = $"Status: {statusAnterior}",
                DadosPosteriores = $"Status: {novoStatus}",
                Motivo = motivo
            };

            await _auditorias.AddAsync(auditoria);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Pedido {PedidoId} status atualizado para {NovoStatus}", pedidoId, novoStatus);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao atualizar status do pedido {PedidoId}", pedidoId);
            return false;
        }
    }

    public async Task<bool> CancelarAsync(Guid pedidoId, string motivo)
    {
        try
        {
            var pedido = await _pedidos.GetByIdAsync(pedidoId);
            if (pedido == null)
                throw new InvalidOperationException("Pedido não encontrado");

            if (pedido.Status == StatusPedido.Entregue || pedido.Status == StatusPedido.Cancelado)
                throw new InvalidOperationException("Pedido não pode ser cancelado neste status");

            pedido.Status = StatusPedido.Cancelado;
            pedido.MotivoCancelamento = motivo;
            pedido.DataAtualizacao = DateTime.UtcNow;

            // Registrar auditoria
            var auditoria = new AuditoriaOperacao
            {
                Tipo = TipoOperacaoAuditoria.CancelamentoPedido,
                EntidadeAfetada = "Pedido",
                IdEntidade = pedidoId,
                UsuarioExecucao = "Sistema",
                Descricao = $"Pedido cancelado",
                DadosPosteriores = $"Motivo: {motivo}",
                Motivo = motivo,
                RequererAprovacao = true
            };

            await _auditorias.AddAsync(auditoria);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Pedido {PedidoId} cancelado. Motivo: {Motivo}", pedidoId, motivo);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao cancelar pedido {PedidoId}", pedidoId);
            return false;
        }
    }

    public async Task<bool> ValidarDisponibilidadeAsync(Guid produtoId, Guid unidadeId)
    {
        var estoque = await _estoques.FirstOrDefaultAsync(
            e => e.ProdutoId == produtoId && e.UnidadeId == unidadeId);

        return estoque != null && estoque.Quantidade > 0;
    }

    private string GerarNumeroPedido()
    {
        return $"PED{DateTime.Now:yyyyMMddHHmmss}{Guid.NewGuid().ToString().Substring(0, 8).ToUpper()}";
    }

    private PedidoDto MapearParaDto(Pedido pedido)
    {
        return new PedidoDto
        {
            Id = pedido.Id,
            NumeroPedido = pedido.NumeroPedido,
            Canal = (int)pedido.Canal,
            Status = (int)pedido.Status,
            ValorTotal = pedido.ValorTotal,
            Desconto = pedido.Desconto,
            ValorFinal = pedido.ValorFinal,
            Observacoes = pedido.Observacoes,
            DataEstimadaEntrega = pedido.DataEstimadaEntrega,
            DataConfirmacaoPagamento = pedido.DataConfirmacaoPagamento,
            Itens = pedido.Itens.Select(i => new ItemPedidoResponseDto
            {
                Id = i.Id,
                ProdutoNome = i.Produto?.Nome ?? "Desconhecido",
                Quantidade = i.Quantidade,
                PrecoUnitario = i.PrecoUnitario,
                Subtotal = i.Subtotal,
                Observacoes = i.Observacoes
            }).ToList(),
            DataCriacao = pedido.DataCriacao
        };
    }
}

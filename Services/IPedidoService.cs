using ApiRaizesNordeste.DTOs;
using ApiRaizesNordeste.Models;

namespace ApiRaizesNordeste.Services;

/// <summary>
/// Interface para serviço de pedidos
/// </summary>
public interface IPedidoService
{
    /// <summary>
    /// Criar um novo pedido
    /// </summary>
    Task<PedidoDto> CriarPedidoAsync(CreatePedidoDto dto, Guid clienteId);

    /// <summary>
    /// Obter pedido por ID
    /// </summary>
    Task<PedidoDto?> GetPorIdAsync(Guid pedidoId);

    /// <summary>
    /// Obter todos os pedidos do cliente
    /// </summary>
    Task<List<PedidoDto>> GetPorClienteAsync(Guid clienteId);

    /// <summary>
    /// Obter pedidos pendentes de uma unidade
    /// </summary>
    Task<List<PedidoDto>> GetPendentesAsync(Guid unidadeId);

    /// <summary>
    /// Atualizar status do pedido
    /// </summary>
    Task<bool> AtualizarStatusAsync(Guid pedidoId, StatusPedido novoStatus, string? motivo = null);

    /// <summary>
    /// Cancelar pedido
    /// </summary>
    Task<bool> CancelarAsync(Guid pedidoId, string motivo);

    /// <summary>
    /// Validar se produto está disponível
    /// </summary>
    Task<bool> ValidarDisponibilidadeAsync(Guid produtoId, Guid unidadeId);
}

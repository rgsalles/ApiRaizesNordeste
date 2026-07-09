namespace ApiRaizesNordeste.Models;

/// <summary>
/// Representa um pedido no sistema
/// </summary>
public class Pedido : BaseEntity
{
    /// <summary>
    /// Cliente que fez o pedido
    /// </summary>
    public Guid ClienteId { get; set; }

    /// <summary>
    /// Unidade responsável pelo pedido
    /// </summary>
    public Guid UnidadeId { get; set; }

    /// <summary>
    /// Número do pedido (sequencial)
    /// </summary>
    public string NumeroPedido { get; set; } = string.Empty;

    /// <summary>
    /// Canal de origem (App, Totem, Balcao, PickUp)
    /// </summary>
    public CanalPedido Canal { get; set; }

    /// <summary>
    /// Status do pedido
    /// </summary>
    public StatusPedido Status { get; set; } = StatusPedido.Pendente;

    /// <summary>
    /// Valor total
    /// </summary>
    public decimal ValorTotal { get; set; }

    /// <summary>
    /// Desconto aplicado
    /// </summary>
    public decimal Desconto { get; set; } = 0;

    /// <summary>
    /// Valor final
    /// </summary>
    public decimal ValorFinal { get; set; }

    /// <summary>
    /// Pontos de loyalty utilizados
    /// </summary>
    public decimal? PontosUtilizados { get; set; }

    /// <summary>
    /// Observações do pedido
    /// </summary>
    public string? Observacoes { get; set; }

    /// <summary>
    /// Data estimada de entrega/retirada
    /// </summary>
    public DateTime? DataEstimadaEntrega { get; set; }

    /// <summary>
    /// Data de confirmação do pagamento
    /// </summary>
    public DateTime? DataConfirmacaoPagamento { get; set; }

    /// <summary>
    /// Motivo do cancelamento (se aplicável)
    /// </summary>
    public string? MotivoCancelamento { get; set; }

    // Relacionamentos
    public Cliente? Cliente { get; set; }
    public Unidade? Unidade { get; set; }
    public Pagamento? Pagamento { get; set; }
    public ICollection<ItemPedido> Itens { get; set; } = new List<ItemPedido>();
}

/// <summary>
/// Canais de origem do pedido
/// </summary>
public enum CanalPedido
{
    App = 1,
    Totem = 2,
    Balcao = 3,
    PickUp = 4
}

/// <summary>
/// Status do pedido
/// </summary>
public enum StatusPedido
{
    Pendente = 1,
    Confirmado = 2,
    Preparando = 3,
    Pronto = 4,
    Entregue = 5,
    Cancelado = 6,
    PendentePagamento = 7
}

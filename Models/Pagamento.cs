namespace ApiRaizesNordeste.Models;

/// <summary>
/// Registra informações de pagamento do pedido
/// </summary>
public class Pagamento : BaseEntity
{
    /// <summary>
    /// Pedido associado
    /// </summary>
    public Guid PedidoId { get; set; }

    /// <summary>
    /// Valor do pagamento
    /// </summary>
    public decimal Valor { get; set; }

    /// <summary>
    /// Método de pagamento (Cartao, Dinheiro, PIX, etc)
    /// </summary>
    public MetodoPagamento Metodo { get; set; }

    /// <summary>
    /// Status do pagamento
    /// </summary>
    public StatusPagamento Status { get; set; } = StatusPagamento.Pendente;

    /// <summary>
    /// ID da transação no provedor externo
    /// </summary>
    public string? IdTransacaoExternal { get; set; }

    /// <summary>
    /// Data de conclusão
    /// </summary>
    public DateTime? DataConclusao { get; set; }

    /// <summary>
    /// Motivo de falha (se aplicável)
    /// </summary>
    public string? MotivoFalha { get; set; }

    /// <summary>
    /// Tentativas de processamento
    /// </summary>
    public int TentativasProcessamento { get; set; } = 0;

    // Relacionamentos
    public Pedido? Pedido { get; set; }
}

/// <summary>
/// Métodos de pagamento
/// </summary>
public enum MetodoPagamento
{
    Cartao = 1,
    Dinheiro = 2,
    PIX = 3,
    Boleto = 4,
    TransferenciaApps = 5
}

/// <summary>
/// Status do pagamento
/// </summary>
public enum StatusPagamento
{
    Pendente = 1,
    Processando = 2,
    Aprovado = 3,
    Recusado = 4,
    Cancelado = 5,
    Reembolsado = 6
}

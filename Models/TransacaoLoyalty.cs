namespace ApiRaizesNordeste.Models;

/// <summary>
/// Transação de pontos no programa de fidelização
/// </summary>
public class TransacaoLoyalty : BaseEntity
{
    /// <summary>
    /// Cliente Loyalty associado
    /// </summary>
    public Guid ClienteLoyaltyId { get; set; }

    /// <summary>
    /// Pedido associado (quando aplicável)
    /// </summary>
    public Guid? PedidoId { get; set; }

    /// <summary>
    /// Tipo de transação (Acúmulo, Resgate, Ajuste, Cancelamento)
    /// </summary>
    public TipoTransacaoLoyalty Tipo { get; set; }

    /// <summary>
    /// Quantidade de pontos (positivo ou negativo)
    /// </summary>
    public decimal Pontos { get; set; }

    /// <summary>
    /// Descrição da transação
    /// </summary>
    public string Descricao { get; set; } = string.Empty;

    /// <summary>
    /// Saldo anterior
    /// </summary>
    public decimal SaldoAnterior { get; set; }

    /// <summary>
    /// Saldo posterior
    /// </summary>
    public decimal SaldoNovo { get; set; }

    // Relacionamentos
    public ClienteLoyalty? ClienteLoyalty { get; set; }
    public Pedido? Pedido { get; set; }
}

/// <summary>
/// Tipos de transação de loyalidade
/// </summary>
public enum TipoTransacaoLoyalty
{
    Acumulo = 1,
    Resgate = 2,
    Ajuste = 3,
    Cancelamento = 4,
    Expirado = 5
}

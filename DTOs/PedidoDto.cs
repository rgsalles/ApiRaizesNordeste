namespace ApiRaizesNordeste.DTOs;

/// <summary>
/// DTO para criar pedido
/// </summary>
public class CreatePedidoDto
{
    public Guid UnidadeId { get; set; }
    public int Canal { get; set; } // CanalPedido enum value
    public List<ItemPedidoDto> Itens { get; set; } = new();
    public string? Observacoes { get; set; }
    public decimal? PontosUtilizados { get; set; }
}

/// <summary>
/// DTO para item do pedido
/// </summary>
public class ItemPedidoDto
{
    public Guid ProdutoId { get; set; }
    public int Quantidade { get; set; }
    public string? Observacoes { get; set; }
}

/// <summary>
/// DTO para resposta de pedido
/// </summary>
public class PedidoDto
{
    public Guid Id { get; set; }
    public string NumeroPedido { get; set; } = string.Empty;
    public int Canal { get; set; }
    public int Status { get; set; }
    public decimal ValorTotal { get; set; }
    public decimal Desconto { get; set; }
    public decimal ValorFinal { get; set; }
    public string? Observacoes { get; set; }
    public DateTime? DataEstimadaEntrega { get; set; }
    public DateTime? DataConfirmacaoPagamento { get; set; }
    public List<ItemPedidoResponseDto> Itens { get; set; } = new();
    public DateTime DataCriacao { get; set; }
}

/// <summary>
/// DTO para resposta de item do pedido
/// </summary>
public class ItemPedidoResponseDto
{
    public Guid Id { get; set; }
    public string ProdutoNome { get; set; } = string.Empty;
    public int Quantidade { get; set; }
    public decimal PrecoUnitario { get; set; }
    public decimal Subtotal { get; set; }
    public string? Observacoes { get; set; }
}

/// <summary>
/// DTO para atualizar status do pedido
/// </summary>
public class AtualizarStatusPedidoDto
{
    public int NovoStatus { get; set; }
    public string? Motivo { get; set; }
}

/// <summary>
/// DTO para cancelar pedido
/// </summary>
public class CancelarPedidoDto
{
    public string Motivo { get; set; } = string.Empty;
}

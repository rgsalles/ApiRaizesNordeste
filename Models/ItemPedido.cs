namespace ApiRaizesNordeste.Models;

/// <summary>
/// Representa um item dentro de um pedido
/// </summary>
public class ItemPedido : BaseEntity
{
    /// <summary>
    /// Pedido associado
    /// </summary>
    public Guid PedidoId { get; set; }

    /// <summary>
    /// Produto
    /// </summary>
    public Guid ProdutoId { get; set; }

    /// <summary>
    /// Quantidade
    /// </summary>
    public int Quantidade { get; set; }

    /// <summary>
    /// Preço unitário no momento da compra
    /// </summary>
    public decimal PrecoUnitario { get; set; }

    /// <summary>
    /// Subtotal (quantidade × preço)
    /// </summary>
    public decimal Subtotal { get; set; }

    /// <summary>
    /// Observações do item (extras, restrições, etc)
    /// </summary>
    public string? Observacoes { get; set; }

    // Relacionamentos
    public Pedido? Pedido { get; set; }
    public Produto? Produto { get; set; }
}

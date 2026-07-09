namespace ApiRaizesNordeste.Models;

/// <summary>
/// Representa o estoque de um produto em uma unidade
/// </summary>
public class EstoqueProduto : BaseEntity
{
    /// <summary>
    /// Produto
    /// </summary>
    public Guid ProdutoId { get; set; }

    /// <summary>
    /// Unidade
    /// </summary>
    public Guid UnidadeId { get; set; }

    /// <summary>
    /// Quantidade em estoque
    /// </summary>
    public int Quantidade { get; set; }

    /// <summary>
    /// Quantidade mínima para alerta
    /// </summary>
    public int QuantidadeMinima { get; set; }

    /// <summary>
    /// Data do último movimento
    /// </summary>
    public DateTime? DataUltimoMovimento { get; set; }

    // Relacionamentos
    public Produto? Produto { get; set; }
    public Unidade? Unidade { get; set; }
}

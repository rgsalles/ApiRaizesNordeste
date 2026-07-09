namespace ApiRaizesNordeste.Models;

/// <summary>
/// Representa um produto do cardápio
/// </summary>
public class Produto : BaseEntity
{
    /// <summary>
    /// Nome do produto
    /// </summary>
    public string Nome { get; set; } = string.Empty;

    /// <summary>
    /// Descrição detalhada
    /// </summary>
    public string? Descricao { get; set; }

    /// <summary>
    /// Preço base
    /// </summary>
    public decimal Preco { get; set; }

    /// <summary>
    /// Categoria do produto
    /// </summary>
    public Guid CategoriaId { get; set; }

    /// <summary>
    /// Imagem do produto
    /// </summary>
    public string? Imagem { get; set; }

    /// <summary>
    /// Informações nutricionais
    /// </summary>
    public string? InformacaoNutricional { get; set; }

    /// <summary>
    /// Ingredientes (separados por vírgula)
    /// </summary>
    public string? Ingredientes { get; set; }

    /// <summary>
    /// Tempo de preparo em minutos
    /// </summary>
    public int? TempoPreparo { get; set; }

    /// <summary>
    /// Se é produto regional (varia por unidade)
    /// </summary>
    public bool Sazonal { get; set; } = false;

    /// <summary>
    /// Data de início da disponibilidade sazonal
    /// </summary>
    public DateTime? DataInicioSazonal { get; set; }

    /// <summary>
    /// Data de fim da disponibilidade sazonal
    /// </summary>
    public DateTime? DataFimSazonal { get; set; }

    /// <summary>
    /// Se o produto está ativo
    /// </summary>
    public bool Ativo { get; set; } = true;

    // Relacionamentos
    public Categoria? Categoria { get; set; }
    public ICollection<EstoqueProduto> Estoques { get; set; } = new List<EstoqueProduto>();
    public ICollection<ItemPedido> ItensPedidos { get; set; } = new List<ItemPedido>();
}

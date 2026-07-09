namespace ApiRaizesNordeste.Models;

/// <summary>
/// Representa uma categoria de produtos
/// </summary>
public class Categoria : BaseEntity
{
    /// <summary>
    /// Nome da categoria
    /// </summary>
    public string Nome { get; set; } = string.Empty;

    /// <summary>
    /// Descrição
    /// </summary>
    public string? Descricao { get; set; }

    /// <summary>
    /// Ícone ou imagem da categoria
    /// </summary>
    public string? Icone { get; set; }

    /// <summary>
    /// Ordem de exibição
    /// </summary>
    public int Ordem { get; set; }

    /// <summary>
    /// Se a categoria está ativa
    /// </summary>
    public bool Ativa { get; set; } = true;

    // Relacionamentos
    public ICollection<Produto> Produtos { get; set; } = new List<Produto>();
}

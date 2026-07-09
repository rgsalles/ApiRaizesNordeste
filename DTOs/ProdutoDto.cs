namespace ApiRaizesNordeste.DTOs;

/// <summary>
/// DTO para criar/atualizar produto
/// </summary>
public class CreateProdutoDto
{
    public string Nome { get; set; } = string.Empty;
    public string? Descricao { get; set; }
    public decimal Preco { get; set; }
    public Guid CategoriaId { get; set; }
    public string? Imagem { get; set; }
    public string? InformacaoNutricional { get; set; }
    public string? Ingredientes { get; set; }
    public int? TempoPreparo { get; set; }
    public bool Sazonal { get; set; }
    public DateTime? DataInicioSazonal { get; set; }
    public DateTime? DataFimSazonal { get; set; }
}

/// <summary>
/// DTO para resposta de produto
/// </summary>
public class ProdutoDto
{
    public Guid Id { get; set; }
    public string Nome { get; set; } = string.Empty;
    public string? Descricao { get; set; }
    public decimal Preco { get; set; }
    public Guid CategoriaId { get; set; }
    public string? Categoria { get; set; }
    public string? Imagem { get; set; }
    public string? InformacaoNutricional { get; set; }
    public string? Ingredientes { get; set; }
    public int? TempoPreparo { get; set; }
    public bool Sazonal { get; set; }
    public bool Ativo { get; set; }
    public DateTime DataCriacao { get; set; }
}

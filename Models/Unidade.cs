namespace ApiRaizesNordeste.Models;

/// <summary>
/// Representa uma unidade (loja/franquia) da Raízes do Nordeste
/// </summary>
public class Unidade : BaseEntity
{
    /// <summary>
    /// Nome da unidade
    /// </summary>
    public string Nome { get; set; } = string.Empty;

    /// <summary>
    /// Número de localização
    /// </summary>
    public string Localizacao { get; set; } = string.Empty;

    /// <summary>
    /// Endereço completo
    /// </summary>
    public string Endereco { get; set; } = string.Empty;

    /// <summary>
    /// Cidade
    /// </summary>
    public string Cidade { get; set; } = string.Empty;

    /// <summary>
    /// Estado (UF)
    /// </summary>
    public string Estado { get; set; } = string.Empty;

    /// <summary>
    /// CEP
    /// </summary>
    public string CEP { get; set; } = string.Empty;

    /// <summary>
    /// Telefone de contato
    /// </summary>
    public string? Telefone { get; set; }

    /// <summary>
    /// Email da unidade
    /// </summary>
    public string? Email { get; set; }

    /// <summary>
    /// Tipo de operação (Completa, Reduzida)
    /// </summary>
    public TipoOperacao TipoOperacao { get; set; } = TipoOperacao.Completa;

    /// <summary>
    /// Horário de abertura
    /// </summary>
    public TimeSpan HorarioAbertura { get; set; }

    /// <summary>
    /// Horário de fechamento
    /// </summary>
    public TimeSpan HorarioFechamento { get; set; }

    /// <summary>
    /// Dias de funcionamento da semana (0-6, domingo a sábado)
    /// </summary>
    public string? DiasAbertura { get; set; } // "0,1,2,3,4,5,6"

    /// <summary>
    /// Se a unidade está ativa
    /// </summary>
    public bool Ativa { get; set; } = true;

    /// <summary>
    /// Latitude para localização
    /// </summary>
    public double? Latitude { get; set; }

    /// <summary>
    /// Longitude para localização
    /// </summary>
    public double? Longitude { get; set; }

    // Relacionamentos
    public ICollection<Produto> Produtos { get; set; } = new List<Produto>();
    public ICollection<Pedido> Pedidos { get; set; } = new List<Pedido>();
    public ICollection<EstoqueProduto> Estoques { get; set; } = new List<EstoqueProduto>();
}

/// <summary>
/// Tipo de operação da unidade
/// </summary>
public enum TipoOperacao
{
    Completa = 1,
    Reduzida = 2
}

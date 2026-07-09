namespace ApiRaizesNordeste.Models;

/// <summary>
/// Registra consentimento LGPD do cliente
/// </summary>
public class ConsentimentoLGPD : BaseEntity
{
    /// <summary>
    /// Cliente
    /// </summary>
    public Guid ClienteId { get; set; }

    /// <summary>
    /// Tipo de consentimento (Marketing, Dados, Cookies, etc)
    /// </summary>
    public TipoConsentimento Tipo { get; set; }

    /// <summary>
    /// Se foi concedido
    /// </summary>
    public bool Concedido { get; set; } = false;

    /// <summary>
    /// Data de vencimento do consentimento
    /// </summary>
    public DateTime? DataVencimento { get; set; }

    /// <summary>
    /// Descrição do que foi consentido
    /// </summary>
    public string Descricao { get; set; } = string.Empty;

    /// <summary>
    /// IP de origem
    /// </summary>
    public string? IPOrigem { get; set; }

    /// <summary>
    /// User Agent (navegador/dispositivo)
    /// </summary>
    public string? UserAgent { get; set; }

    // Relacionamentos
    public Cliente? Cliente { get; set; }
}

/// <summary>
/// Tipos de consentimento LGPD
/// </summary>
public enum TipoConsentimento
{
    UsoDados = 1,
    Marketing = 2,
    Cookies = 3,
    Profiling = 4,
    CompartilhamentoDados = 5
}

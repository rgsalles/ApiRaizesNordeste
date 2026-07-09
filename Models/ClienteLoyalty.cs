namespace ApiRaizesNordeste.Models;

/// <summary>
/// Dados do programa de fidelização do cliente
/// </summary>
public class ClienteLoyalty : BaseEntity
{
    /// <summary>
    /// Cliente associado
    /// </summary>
    public Guid ClienteId { get; set; }

    /// <summary>
    /// Saldo de pontos
    /// </summary>
    public decimal SaldoPontos { get; set; } = 0;

    /// <summary>
    /// Nível de fidelização (Bronze, Prata, Ouro, Platina)
    /// </summary>
    public NivelLoyalty Nivel { get; set; } = NivelLoyalty.Bronze;

    /// <summary>
    /// Total de pontos acumulados (histórico)
    /// </summary>
    public decimal PontosAcumulados { get; set; } = 0;

    /// <summary>
    /// Data de ingresso no programa
    /// </summary>
    public DateTime DataIngresso { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Data de renovação do nível
    /// </summary>
    public DateTime? DataRenovacaoNivel { get; set; }

    // Relacionamentos
    public Cliente? Cliente { get; set; }
    public ICollection<TransacaoLoyalty> Transacoes { get; set; } = new List<TransacaoLoyalty>();
}

/// <summary>
/// Níveis de fidelização
/// </summary>
public enum NivelLoyalty
{
    Bronze = 1,
    Prata = 2,
    Ouro = 3,
    Platina = 4
}

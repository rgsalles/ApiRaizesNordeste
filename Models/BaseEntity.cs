namespace ApiRaizesNordeste.Models;

/// <summary>
/// Entidade base para todas as entidades do domínio
/// </summary>
public abstract class BaseEntity
{
    /// <summary>
    /// Identificador único
    /// </summary>
    public Guid Id { get; set; } = Guid.NewGuid();

    /// <summary>
    /// Data de criação
    /// </summary>
    public DateTime DataCriacao { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Data da última atualização
    /// </summary>
    public DateTime? DataAtualizacao { get; set; }

    /// <summary>
    /// Usuário que criou o registro
    /// </summary>
    public string? CriadoPor { get; set; }

    /// <summary>
    /// Usuário que atualizou por último
    /// </summary>
    public string? AtualizadoPor { get; set; }

    /// <summary>
    /// Indica se o registro foi deletado (soft delete)
    /// </summary>
    public bool Deletado { get; set; } = false;
}

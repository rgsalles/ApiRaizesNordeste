namespace ApiRaizesNordeste.Models;

/// <summary>
/// Representa um cliente do sistema
/// </summary>
public class Cliente : BaseEntity
{
    /// <summary>
    /// Email do cliente (usado para login)
    /// </summary>
    public string Email { get; set; } = string.Empty;

    /// <summary>
    /// Hash da senha
    /// </summary>
    public string SenhaHash { get; set; } = string.Empty;

    /// <summary>
    /// Nome completo
    /// </summary>
    public string NomeCompleto { get; set; } = string.Empty;

    /// <summary>
    /// Telefone
    /// </summary>
    public string? Telefone { get; set; }

    /// <summary>
    /// Gênero (M, F, Outro, Prefiro não informar)
    /// </summary>
    public char? Genero { get; set; }

    /// <summary>
    /// Data de nascimento
    /// </summary>
    public DateTime? DataNascimento { get; set; }

    /// <summary>
    /// CPF (criptografado)
    /// </summary>
    public string? CPF { get; set; }

    /// <summary>
    /// Endereço padrão
    /// </summary>
    public string? Endereco { get; set; }

    /// <summary>
    /// Se o email foi verificado
    /// </summary>
    public bool EmailVerificado { get; set; } = false;

    /// <summary>
    /// Se o cliente está ativo
    /// </summary>
    public bool Ativo { get; set; } = true;

    /// <summary>
    /// Data do último login
    /// </summary>
    public DateTime? DataUltimoLogin { get; set; }

    // Relacionamentos
    public ClienteLoyalty? DadosLoyalty { get; set; }
    public ICollection<Pedido> Pedidos { get; set; } = new List<Pedido>();
    public ICollection<ConsentimentoLGPD> Consentimentos { get; set; } = new List<ConsentimentoLGPD>();
}

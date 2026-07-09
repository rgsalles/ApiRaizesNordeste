namespace ApiRaizesNordeste.Models;

/// <summary>
/// Registro de auditoria para operações sensíveis
/// </summary>
public class AuditoriaOperacao : BaseEntity
{
    /// <summary>
    /// Tipo de operação (Cancelamento, Desconto, Ajuste, etc)
    /// </summary>
    public TipoOperacaoAuditoria Tipo { get; set; }

    /// <summary>
    /// Entidade afetada (Pedido, Cliente, Produto, etc)
    /// </summary>
    public string EntidadeAfetada { get; set; } = string.Empty;

    /// <summary>
    /// ID da entidade
    /// </summary>
    public Guid IdEntidade { get; set; }

    /// <summary>
    /// Usuário que realizou
    /// </summary>
    public string UsuarioExecucao { get; set; } = string.Empty;

    /// <summary>
    /// Descrição da operação
    /// </summary>
    public string Descricao { get; set; } = string.Empty;

    /// <summary>
    /// Dados anteriores (JSON)
    /// </summary>
    public string? DadosAnteriores { get; set; }

    /// <summary>
    /// Dados posteriores (JSON)
    /// </summary>
    public string? DadosPosteriores { get; set; }

    /// <summary>
    /// IP de origem
    /// </summary>
    public string? IPOrigem { get; set; }

    /// <summary>
    /// Motivo da operação
    /// </summary>
    public string? Motivo { get; set; }

    /// <summary>
    /// Se requer aprovação de gerente
    /// </summary>
    public bool RequererAprovacao { get; set; } = false;

    /// <summary>
    /// Se foi aprovada
    /// </summary>
    public bool? Aprovada { get; set; }

    /// <summary>
    /// Usuário que aprovou
    /// </summary>
    public string? AprovadoPor { get; set; }

    /// <summary>
    /// Data de aprovação
    /// </summary>
    public DateTime? DataAprovacao { get; set; }
}

/// <summary>
/// Tipos de operação auditados
/// </summary>
public enum TipoOperacaoAuditoria
{
    CancelamentoPedido = 1,
    AplicacaoDesconto = 2,
    AjusteEstoque = 3,
    AlteracaoPreco = 4,
    DeletarCliente = 5,
    AcessoDADOSSensiveis = 6,
    AjustePoints = 7,
    ReembolsoPagamento = 8
}

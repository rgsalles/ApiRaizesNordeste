namespace ApiRaizesNordeste.DTOs;

/// <summary>
/// DTO para resposta de fidelização do cliente
/// </summary>
public class ClienteLoyaltyDto
{
    public Guid Id { get; set; }
    public decimal SaldoPontos { get; set; }
    public int Nivel { get; set; }
    public decimal PontosAcumulados { get; set; }
    public DateTime DataIngresso { get; set; }
    public DateTime? DataRenovacaoNivel { get; set; }
}

/// <summary>
/// DTO para resgate de pontos
/// </summary>
public class ResgatePointsDto
{
    public decimal Pontos { get; set; }
    public string? Descricao { get; set; }
}

/// <summary>
/// DTO para resposta de transação de loyalty
/// </summary>
public class TransacaoLoyaltyDto
{
    public Guid Id { get; set; }
    public int Tipo { get; set; }
    public decimal Pontos { get; set; }
    public string Descricao { get; set; } = string.Empty;
    public decimal SaldoAnterior { get; set; }
    public decimal SaldoNovo { get; set; }
    public DateTime DataCriacao { get; set; }
}

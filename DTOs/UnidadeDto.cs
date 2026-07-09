namespace ApiRaizesNordeste.DTOs;

/// <summary>
/// DTO para criar/atualizar uma unidade
/// </summary>
public class CreateUnidadeDto
{
    public string Nome { get; set; } = string.Empty;
    public string Localizacao { get; set; } = string.Empty;
    public string Endereco { get; set; } = string.Empty;
    public string Cidade { get; set; } = string.Empty;
    public string Estado { get; set; } = string.Empty;
    public string CEP { get; set; } = string.Empty;
    public string? Telefone { get; set; }
    public string? Email { get; set; }
    public TimeSpan HorarioAbertura { get; set; }
    public TimeSpan HorarioFechamento { get; set; }
    public double? Latitude { get; set; }
    public double? Longitude { get; set; }
}

/// <summary>
/// DTO para resposta de unidade
/// </summary>
public class UnidadeDto
{
    public Guid Id { get; set; }
    public string Nome { get; set; } = string.Empty;
    public string Localizacao { get; set; } = string.Empty;
    public string Endereco { get; set; } = string.Empty;
    public string Cidade { get; set; } = string.Empty;
    public string Estado { get; set; } = string.Empty;
    public string CEP { get; set; } = string.Empty;
    public string? Telefone { get; set; }
    public string? Email { get; set; }
    public TimeSpan HorarioAbertura { get; set; }
    public TimeSpan HorarioFechamento { get; set; }
    public bool Ativa { get; set; }
    public double? Latitude { get; set; }
    public double? Longitude { get; set; }
    public DateTime DataCriacao { get; set; }
}

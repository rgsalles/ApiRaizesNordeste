namespace ApiRaizesNordeste.DTOs;

/// <summary>
/// DTO para registrar novo cliente
/// </summary>
public class RegisterClienteDto
{
    public string Email { get; set; } = string.Empty;
    public string Senha { get; set; } = string.Empty;
    public string NomeCompleto { get; set; } = string.Empty;
    public string? Telefone { get; set; }
}

/// <summary>
/// DTO para login
/// </summary>
public class LoginDto
{
    public string Email { get; set; } = string.Empty;
    public string Senha { get; set; } = string.Empty;
}

/// <summary>
/// DTO para resposta de autenticação
/// </summary>
public class AuthResponseDto
{
    public Guid ClienteId { get; set; }
    public string NomeCompleto { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Token { get; set; } = string.Empty;
    public DateTime TokenExpira { get; set; }
}

/// <summary>
/// DTO para resposta de cliente
/// </summary>
public class ClienteDto
{
    public Guid Id { get; set; }
    public string Email { get; set; } = string.Empty;
    public string NomeCompleto { get; set; } = string.Empty;
    public string? Telefone { get; set; }
    public bool EmailVerificado { get; set; }
    public bool Ativo { get; set; }
    public DateTime? DataUltimoLogin { get; set; }
    public DateTime DataCriacao { get; set; }
}

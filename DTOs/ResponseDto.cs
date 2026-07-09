namespace ApiRaizesNordeste.DTOs;

/// <summary>
/// Resposta de erro padronizada
/// </summary>
public class ErrorResponseDto
{
    public int StatusCode { get; set; }
    public string Message { get; set; } = string.Empty;
    public Dictionary<string, string[]>? Errors { get; set; }
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;

    public ErrorResponseDto(int statusCode, string message)
    {
        StatusCode = statusCode;
        Message = message;
    }

    public ErrorResponseDto(int statusCode, string message, Dictionary<string, string[]> errors)
    {
        StatusCode = statusCode;
        Message = message;
        Errors = errors;
    }
}

/// <summary>
/// Resposta de sucesso padronizada
/// </summary>
public class SuccessResponseDto<T>
{
    public bool Success { get; set; } = true;
    public T? Data { get; set; }
    public string? Message { get; set; }
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
}

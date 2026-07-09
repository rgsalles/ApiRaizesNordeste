using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using ApiRaizesNordeste.Models;
using ApiRaizesNordeste.Services;
using Microsoft.Extensions.Options;
using Xunit;

namespace ApiRaizesNordeste.Tests.Services;

public class JwtTokenServiceTests
{
    [Fact]
    public void GerarToken_IncluiIdentidadeDoClienteEExpiracao()
    {
        var options = Options.Create(new JwtOptions
        {
            Issuer = "test-issuer",
            Audience = "test-audience",
            Key = "uma-chave-de-testes-com-pelo-menos-32-bytes",
            ExpirationMinutes = 60
        });
        var cliente = new Cliente
        {
            Id = Guid.NewGuid(),
            Email = "cliente@teste.com",
            NomeCompleto = "Cliente Teste"
        };
        var service = new JwtTokenService(options);

        var resultado = service.GerarToken(cliente);
        var token = new JwtSecurityTokenHandler().ReadJwtToken(resultado.Token);

        Assert.Equal("test-issuer", token.Issuer);
        Assert.Contains("test-audience", token.Audiences);
        Assert.Contains(token.Claims, c =>
            c.Type == ClaimTypes.NameIdentifier && c.Value == cliente.Id.ToString());
        Assert.True(resultado.ExpiresAt > DateTime.UtcNow);
    }
}

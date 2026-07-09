using ApiRaizesNordeste.Data;
using ApiRaizesNordeste.DTOs;
using ApiRaizesNordeste.Models;
using ApiRaizesNordeste.Repositories;
using ApiRaizesNordeste.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace ApiRaizesNordeste.Tests.Services;

public class ClienteServiceTests : IDisposable
{
    private readonly ApplicationDbContext _context;
    private readonly ClienteService _service;

    public ClienteServiceTests()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        _context = new ApplicationDbContext(options);

        var jwtTokenService = new Mock<IJwtTokenService>();
        jwtTokenService
            .Setup(x => x.GerarToken(It.IsAny<Cliente>()))
            .Returns(("jwt-token-valido", DateTime.UtcNow.AddHours(8)));

        _service = new ClienteService(
            _context,
            new ClienteRepository(_context),
            new Repository<ClienteLoyalty>(_context),
            Mock.Of<ILogger<ClienteService>>(),
            jwtTokenService.Object);
    }

    [Fact]
    public async Task RegistrarAsync_ComDadosValidos_DeveRegistrarCliente()
    {
        var dto = new RegisterClienteDto
        {
            Email = "novo@test.com",
            Senha = "Senha123",
            NomeCompleto = "João Silva",
            Telefone = "11999999999"
        };

        var resultado = await _service.RegistrarAsync(dto);

        Assert.Equal(dto.Email, resultado.Email);
        Assert.NotEqual(Guid.Empty, resultado.ClienteId);
        Assert.False(string.IsNullOrWhiteSpace(resultado.Token));
        Assert.Equal(1, await _context.Clientes.CountAsync());
        Assert.Equal(1, await _context.ClientesLoyalty.CountAsync());
    }

    [Fact]
    public async Task RegistrarAsync_ComEmailExistente_DeveThrowException()
    {
        _context.Clientes.Add(new Cliente
        {
            Email = "existente@test.com",
            SenhaHash = "hash",
            NomeCompleto = "Cliente existente"
        });
        await _context.SaveChangesAsync();

        var dto = new RegisterClienteDto
        {
            Email = "existente@test.com",
            Senha = "Senha123",
            NomeCompleto = "João"
        };

        await Assert.ThrowsAsync<InvalidOperationException>(
            () => _service.RegistrarAsync(dto));
    }

    [Fact]
    public async Task VerificarEmailExisteAsync_ComEmailExistente_DeveRetornarTrue()
    {
        _context.Clientes.Add(new Cliente
        {
            Email = "existe@test.com",
            SenhaHash = "hash",
            NomeCompleto = "Cliente"
        });
        await _context.SaveChangesAsync();

        var resultado = await _service.VerificarEmailExisteAsync("existe@test.com");

        Assert.True(resultado);
    }

    [Fact]
    public async Task GetPorIdAsync_ComClienteExistente_DeveRetornarCliente()
    {
        var cliente = new Cliente
        {
            Email = "test@test.com",
            SenhaHash = "hash",
            NomeCompleto = "João"
        };
        _context.Clientes.Add(cliente);
        await _context.SaveChangesAsync();

        var resultado = await _service.GetPorIdAsync(cliente.Id);

        Assert.NotNull(resultado);
        Assert.Equal(cliente.Email, resultado.Email);
    }

    public void Dispose()
    {
        _context.Dispose();
    }
}

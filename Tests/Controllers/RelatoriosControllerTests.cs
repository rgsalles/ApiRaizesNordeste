using ApiRaizesNordeste.Controllers;
using ApiRaizesNordeste.DTOs;
using ApiRaizesNordeste.Services;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace ApiRaizesNordeste.Tests.Controllers;

public class RelatoriosControllerTests
{
    private readonly Mock<IRelatorioService> _service = new();

    [Fact]
    public async Task ObterResumoVendas_ComPeriodoValido_RetornaRelatorio()
    {
        var inicio = new DateTime(2026, 1, 1);
        var fim = new DateTime(2026, 1, 31);
        var esperado = new ResumoVendasDto { TotalPedidos = 12, Faturamento = 500 };
        _service.Setup(s => s.ObterResumoVendasAsync(inicio, fim, null))
            .ReturnsAsync(esperado);
        var controller = new RelatoriosController(_service.Object);

        var resultado = await controller.ObterResumoVendas(inicio, fim, null);

        var ok = Assert.IsType<OkObjectResult>(resultado.Result);
        Assert.Same(esperado, ok.Value);
    }

    [Fact]
    public async Task ObterResumoVendas_ComDataInicialPosterior_RetornaBadRequest()
    {
        var controller = new RelatoriosController(_service.Object);

        var resultado = await controller.ObterResumoVendas(
            new DateTime(2026, 2, 1), new DateTime(2026, 1, 1), null);

        Assert.IsType<BadRequestObjectResult>(resultado.Result);
        _service.VerifyNoOtherCalls();
    }

    [Theory]
    [InlineData(0)]
    [InlineData(101)]
    public async Task ObterProdutosMaisVendidos_ComLimiteInvalido_RetornaBadRequest(int limite)
    {
        var controller = new RelatoriosController(_service.Object);

        var resultado = await controller.ObterProdutosMaisVendidos(
            new DateTime(2026, 1, 1), new DateTime(2026, 1, 31), null, limite);

        Assert.IsType<BadRequestObjectResult>(resultado.Result);
        _service.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task ObterEstoqueBaixo_RetornaItensDoServico()
    {
        var itens = new List<EstoqueBaixoDto>
        {
            new() { Produto = "Cuscuz", Quantidade = 2, QuantidadeMinima = 5 }
        };
        _service.Setup(s => s.ObterEstoqueBaixoAsync(null)).ReturnsAsync(itens);
        var controller = new RelatoriosController(_service.Object);

        var resultado = await controller.ObterEstoqueBaixo(null);

        var ok = Assert.IsType<OkObjectResult>(resultado.Result);
        Assert.Same(itens, ok.Value);
    }
}

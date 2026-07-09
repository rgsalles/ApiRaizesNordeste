namespace ApiRaizesNordeste.DTOs;

public class ResumoVendasDto
{
    public DateTime DataInicio { get; set; }
    public DateTime DataFim { get; set; }
    public Guid? UnidadeId { get; set; }
    public int TotalPedidos { get; set; }
    public int PedidosEntregues { get; set; }
    public int PedidosCancelados { get; set; }
    public decimal Faturamento { get; set; }
    public decimal TotalDescontos { get; set; }
    public decimal TicketMedio { get; set; }
    public List<QuantidadePorDescricaoDto> PedidosPorStatus { get; set; } = [];
    public List<QuantidadePorDescricaoDto> PedidosPorCanal { get; set; } = [];
}

public class QuantidadePorDescricaoDto
{
    public string Descricao { get; set; } = string.Empty;
    public int Quantidade { get; set; }
}

public class ProdutoMaisVendidoDto
{
    public Guid ProdutoId { get; set; }
    public string Produto { get; set; } = string.Empty;
    public int QuantidadeVendida { get; set; }
    public decimal Faturamento { get; set; }
}

public class ClienteComprasDto
{
    public Guid ClienteId { get; set; }
    public string Cliente { get; set; } = string.Empty;
    public int QuantidadePedidos { get; set; }
    public decimal TotalCompras { get; set; }
}

public class EstoqueBaixoDto
{
    public Guid UnidadeId { get; set; }
    public string Unidade { get; set; } = string.Empty;
    public Guid ProdutoId { get; set; }
    public string Produto { get; set; } = string.Empty;
    public int Quantidade { get; set; }
    public int QuantidadeMinima { get; set; }
}

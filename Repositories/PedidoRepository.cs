using ApiRaizesNordeste.Data;
using ApiRaizesNordeste.Models;
using Microsoft.EntityFrameworkCore;

namespace ApiRaizesNordeste.Repositories;

public interface IPedidoRepository : IRepository<Pedido>
{
    Task<Pedido?> GetComItensAsync(Guid pedidoId);
    Task<IEnumerable<Pedido>> GetPorClienteAsync(Guid clienteId);
    Task<IEnumerable<Pedido>> GetPendentesAsync(Guid unidadeId);
}

public class PedidoRepository : Repository<Pedido>, IPedidoRepository
{
    public PedidoRepository(ApplicationDbContext context) : base(context)
    {
    }

    public Task<Pedido?> GetComItensAsync(Guid pedidoId) =>
        Context.Pedidos
            .Include(p => p.Itens)
            .ThenInclude(i => i.Produto)
            .Include(p => p.Pagamento)
            .Include(p => p.Cliente)
            .FirstOrDefaultAsync(p => p.Id == pedidoId);

    public async Task<IEnumerable<Pedido>> GetPorClienteAsync(Guid clienteId) =>
        await Context.Pedidos
            .Where(p => p.ClienteId == clienteId)
            .Include(p => p.Itens)
            .ThenInclude(i => i.Produto)
            .OrderByDescending(p => p.DataCriacao)
            .ToListAsync();

    public async Task<IEnumerable<Pedido>> GetPendentesAsync(Guid unidadeId) =>
        await Context.Pedidos
            .Where(p => p.UnidadeId == unidadeId &&
                        (p.Status == StatusPedido.Pendente ||
                         p.Status == StatusPedido.Confirmado ||
                         p.Status == StatusPedido.Preparando))
            .Include(p => p.Itens)
            .ThenInclude(i => i.Produto)
            .OrderBy(p => p.DataCriacao)
            .ToListAsync();
}

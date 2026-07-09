using ApiRaizesNordeste.Data;
using ApiRaizesNordeste.Models;

namespace ApiRaizesNordeste.Repositories;

public interface IUnidadeRepository : IRepository<Unidade>
{
    Task<IEnumerable<Unidade>> GetPorCidadeAsync(string cidade);
    Task<IEnumerable<Unidade>> GetAtivasAsync();
}

public class UnidadeRepository : Repository<Unidade>, IUnidadeRepository
{
    public UnidadeRepository(ApplicationDbContext context) : base(context)
    {
    }

    public Task<IEnumerable<Unidade>> GetPorCidadeAsync(string cidade) =>
        FindAsync(u => u.Cidade == cidade && u.Ativa);

    public Task<IEnumerable<Unidade>> GetAtivasAsync() =>
        FindAsync(u => u.Ativa);
}

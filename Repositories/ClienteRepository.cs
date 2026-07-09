using ApiRaizesNordeste.Data;
using ApiRaizesNordeste.Models;
using Microsoft.EntityFrameworkCore;

namespace ApiRaizesNordeste.Repositories;

public interface IClienteRepository : IRepository<Cliente>
{
    Task<Cliente?> GetPorEmailAsync(string email);
    Task<bool> EmailExisteAsync(string email);
}

public class ClienteRepository : Repository<Cliente>, IClienteRepository
{
    public ClienteRepository(ApplicationDbContext context) : base(context)
    {
    }

    public Task<Cliente?> GetPorEmailAsync(string email) =>
        Context.Clientes.FirstOrDefaultAsync(c => c.Email == email);

    public Task<bool> EmailExisteAsync(string email) =>
        Context.Clientes.AnyAsync(c => c.Email == email);
}

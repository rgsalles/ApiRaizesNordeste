using ApiRaizesNordeste.Data;
using ApiRaizesNordeste.DTOs;
using ApiRaizesNordeste.Models;
using ApiRaizesNordeste.Repositories;

namespace ApiRaizesNordeste.Services;

/// <summary>
/// Interface para serviço de programa de fidelização
/// </summary>
public interface ILoyaltyService
{
    Task<ClienteLoyaltyDto?> GetSaldoAsync(Guid clienteId);
    Task<List<TransacaoLoyaltyDto>> GetHistoricoAsync(Guid clienteId);
    Task<decimal> AdicionarPontosAsync(Guid clienteId, decimal pontos, string motivo);
    Task<bool> ResgataePontosAsync(Guid clienteId, decimal pontos, string motivo);
    Task AtualizarNivelAsync(Guid clienteId);
}

/// <summary>
/// Implementação do serviço de loyalty
/// </summary>
public class LoyaltyService : ILoyaltyService
{
    private readonly ApplicationDbContext _context;
    private readonly IRepository<ClienteLoyalty> _clientesLoyalty;
    private readonly IRepository<TransacaoLoyalty> _transacoes;
    private readonly ILogger<LoyaltyService> _logger;

    public LoyaltyService(
        ApplicationDbContext context,
        IRepository<ClienteLoyalty> clientesLoyalty,
        IRepository<TransacaoLoyalty> transacoes,
        ILogger<LoyaltyService> logger)
    {
        _context = context;
        _clientesLoyalty = clientesLoyalty;
        _transacoes = transacoes;
        _logger = logger;
    }

    public async Task<ClienteLoyaltyDto?> GetSaldoAsync(Guid clienteId)
    {
        var loyalty = await _clientesLoyalty.FirstOrDefaultAsync(l => l.ClienteId == clienteId);
        return loyalty != null ? MapearParaDto(loyalty) : null;
    }

    public async Task<List<TransacaoLoyaltyDto>> GetHistoricoAsync(Guid clienteId)
    {
        var loyalty = await _clientesLoyalty.FirstOrDefaultAsync(l => l.ClienteId == clienteId);
        if (loyalty == null)
            return new List<TransacaoLoyaltyDto>();

        var transacoes = await _transacoes.FindAsync(t => t.ClienteLoyaltyId == loyalty.Id);
        return transacoes.Select(MapearTransacaoParaDto).ToList();
    }

    public async Task<decimal> AdicionarPontosAsync(Guid clienteId, decimal pontos, string motivo)
    {
        try
        {
            var loyalty = await _clientesLoyalty.FirstOrDefaultAsync(l => l.ClienteId == clienteId);
            if (loyalty == null)
                throw new InvalidOperationException("Cliente não tem dados de loyalty");

            var saldoAnterior = loyalty.SaldoPontos;
            loyalty.SaldoPontos += pontos;
            loyalty.PontosAcumulados += pontos;

            // Registrar transação
            var transacao = new TransacaoLoyalty
            {
                ClienteLoyaltyId = loyalty.Id,
                Tipo = TipoTransacaoLoyalty.Acumulo,
                Pontos = pontos,
                Descricao = motivo,
                SaldoAnterior = saldoAnterior,
                SaldoNovo = loyalty.SaldoPontos
            };

            await _transacoes.AddAsync(transacao);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Pontos adicionados ao cliente {ClienteId}: {Pontos}", clienteId, pontos);

            return loyalty.SaldoPontos;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao adicionar pontos ao cliente {ClienteId}", clienteId);
            throw;
        }
    }

    public async Task<bool> ResgataePontosAsync(Guid clienteId, decimal pontos, string motivo)
    {
        try
        {
            var loyalty = await _clientesLoyalty.FirstOrDefaultAsync(l => l.ClienteId == clienteId);
            if (loyalty == null)
                throw new InvalidOperationException("Cliente não tem dados de loyalty");

            if (loyalty.SaldoPontos < pontos)
                throw new InvalidOperationException("Saldo de pontos insuficiente");

            var saldoAnterior = loyalty.SaldoPontos;
            loyalty.SaldoPontos -= pontos;

            // Registrar transação
            var transacao = new TransacaoLoyalty
            {
                ClienteLoyaltyId = loyalty.Id,
                Tipo = TipoTransacaoLoyalty.Resgate,
                Pontos = -pontos,
                Descricao = motivo,
                SaldoAnterior = saldoAnterior,
                SaldoNovo = loyalty.SaldoPontos
            };

            await _transacoes.AddAsync(transacao);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Pontos resgatados do cliente {ClienteId}: {Pontos}", clienteId, pontos);

            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao resgatar pontos do cliente {ClienteId}", clienteId);
            return false;
        }
    }

    public async Task AtualizarNivelAsync(Guid clienteId)
    {
        try
        {
            var loyalty = await _clientesLoyalty.FirstOrDefaultAsync(l => l.ClienteId == clienteId);
            if (loyalty == null)
                return;

            var novoNivel = DeterminarNivel(loyalty.PontosAcumulados);
            if (novoNivel != loyalty.Nivel)
            {
                loyalty.Nivel = novoNivel;
                loyalty.DataRenovacaoNivel = DateTime.UtcNow;
                await _context.SaveChangesAsync();

                _logger.LogInformation("Nível de cliente {ClienteId} atualizado para {Nivel}", clienteId, novoNivel);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao atualizar nível de cliente {ClienteId}", clienteId);
        }
    }

    private NivelLoyalty DeterminarNivel(decimal pontosAcumulados)
    {
        return pontosAcumulados switch
        {
            >= 5000 => NivelLoyalty.Platina,
            >= 2500 => NivelLoyalty.Ouro,
            >= 1000 => NivelLoyalty.Prata,
            _ => NivelLoyalty.Bronze
        };
    }

    private ClienteLoyaltyDto MapearParaDto(ClienteLoyalty loyalty)
    {
        return new ClienteLoyaltyDto
        {
            Id = loyalty.Id,
            SaldoPontos = loyalty.SaldoPontos,
            Nivel = (int)loyalty.Nivel,
            PontosAcumulados = loyalty.PontosAcumulados,
            DataIngresso = loyalty.DataIngresso,
            DataRenovacaoNivel = loyalty.DataRenovacaoNivel
        };
    }

    private TransacaoLoyaltyDto MapearTransacaoParaDto(TransacaoLoyalty transacao)
    {
        return new TransacaoLoyaltyDto
        {
            Id = transacao.Id,
            Tipo = (int)transacao.Tipo,
            Pontos = transacao.Pontos,
            Descricao = transacao.Descricao,
            SaldoAnterior = transacao.SaldoAnterior,
            SaldoNovo = transacao.SaldoNovo,
            DataCriacao = transacao.DataCriacao
        };
    }
}

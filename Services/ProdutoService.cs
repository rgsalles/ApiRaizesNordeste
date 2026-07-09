using ApiRaizesNordeste.Data;
using ApiRaizesNordeste.DTOs;
using ApiRaizesNordeste.Models;
using ApiRaizesNordeste.Repositories;

namespace ApiRaizesNordeste.Services;

/// <summary>
/// Interface para serviço de produtos
/// </summary>
public interface IProdutoService
{
    Task<ProdutoDto?> GetPorIdAsync(Guid id);
    Task<List<ProdutoDto>> GetPorCategoriaAsync(Guid categoriaId);
    Task<List<ProdutoDto>> GetPorUnidadeAsync(Guid unidadeId);
    Task<List<ProdutoDto>> GetTodosAtivosAsync();
    Task<ProdutoDto> CriarAsync(CreateProdutoDto dto);
    Task<bool> AtualizarAsync(Guid id, CreateProdutoDto dto);
    Task<bool> DeletarAsync(Guid id);
}

/// <summary>
/// Implementação do serviço de produtos
/// </summary>
public class ProdutoService : IProdutoService
{
    private readonly ApplicationDbContext _context;
    private readonly IRepository<Produto> _produtos;
    private readonly IRepository<Categoria> _categorias;
    private readonly IRepository<EstoqueProduto> _estoques;
    private readonly ILogger<ProdutoService> _logger;

    public ProdutoService(
        ApplicationDbContext context,
        IRepository<Produto> produtos,
        IRepository<Categoria> categorias,
        IRepository<EstoqueProduto> estoques,
        ILogger<ProdutoService> logger)
    {
        _context = context;
        _produtos = produtos;
        _categorias = categorias;
        _estoques = estoques;
        _logger = logger;
    }

    public async Task<ProdutoDto?> GetPorIdAsync(Guid id)
    {
        var produto = await _produtos.GetByIdAsync(id);
        return produto != null ? MapearParaDto(produto) : null;
    }

    public async Task<List<ProdutoDto>> GetPorCategoriaAsync(Guid categoriaId)
    {
        var produtos = await _produtos.FindAsync(
            p => p.CategoriaId == categoriaId && p.Ativo);
        return produtos.Select(MapearParaDto).ToList();
    }

    public async Task<List<ProdutoDto>> GetPorUnidadeAsync(Guid unidadeId)
    {
        var estoques = await _estoques.FindAsync(
            e => e.UnidadeId == unidadeId && e.Quantidade > 0);
        var produtos = estoques
            .Where(e => e.Produto != null && e.Produto.Ativo)
            .Select(e => e.Produto!)
            .Distinct()
            .ToList();
        
        return produtos.Select(MapearParaDto).ToList();
    }

    public async Task<List<ProdutoDto>> GetTodosAtivosAsync()
    {
        var produtos = await _produtos.FindAsync(p => p.Ativo);
        return produtos.Select(MapearParaDto).ToList();
    }

    public async Task<ProdutoDto> CriarAsync(CreateProdutoDto dto)
    {
        try
        {
            var categoria = await _categorias.GetByIdAsync(dto.CategoriaId);
            if (categoria == null)
                throw new InvalidOperationException("Categoria não encontrada");

            var produto = new Produto
            {
                Nome = dto.Nome,
                Descricao = dto.Descricao,
                Preco = dto.Preco,
                CategoriaId = dto.CategoriaId,
                Imagem = dto.Imagem,
                InformacaoNutricional = dto.InformacaoNutricional,
                Ingredientes = dto.Ingredientes,
                TempoPreparo = dto.TempoPreparo,
                Sazonal = dto.Sazonal,
                DataInicioSazonal = dto.DataInicioSazonal,
                DataFimSazonal = dto.DataFimSazonal,
                Ativo = true
            };

            await _produtos.AddAsync(produto);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Produto {ProdutoNome} criado com sucesso", produto.Nome);

            return MapearParaDto(produto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao criar produto {ProdutoNome}", dto.Nome);
            throw;
        }
    }

    public async Task<bool> AtualizarAsync(Guid id, CreateProdutoDto dto)
    {
        try
        {
            var produto = await _produtos.GetByIdAsync(id);
            if (produto == null)
                return false;

            var categoria = await _categorias.GetByIdAsync(dto.CategoriaId);
            if (categoria == null)
                return false;

            produto.Nome = dto.Nome;
            produto.Descricao = dto.Descricao;
            produto.Preco = dto.Preco;
            produto.CategoriaId = dto.CategoriaId;
            produto.Imagem = dto.Imagem;
            produto.InformacaoNutricional = dto.InformacaoNutricional;
            produto.Ingredientes = dto.Ingredientes;
            produto.TempoPreparo = dto.TempoPreparo;
            produto.Sazonal = dto.Sazonal;
            produto.DataInicioSazonal = dto.DataInicioSazonal;
            produto.DataFimSazonal = dto.DataFimSazonal;
            produto.DataAtualizacao = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            _logger.LogInformation("Produto {ProdutoId} atualizado com sucesso", id);

            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao atualizar produto {ProdutoId}", id);
            return false;
        }
    }

    public async Task<bool> DeletarAsync(Guid id)
    {
        try
        {
            var produto = await _produtos.GetByIdAsync(id);
            if (produto == null)
                return false;

            produto.Deletado = true;
            produto.Ativo = false;
            produto.DataAtualizacao = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            _logger.LogInformation("Produto {ProdutoId} deletado (soft delete)", id);

            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao deletar produto {ProdutoId}", id);
            return false;
        }
    }

    private ProdutoDto MapearParaDto(Produto produto)
    {
        return new ProdutoDto
        {
            Id = produto.Id,
            Nome = produto.Nome,
            Descricao = produto.Descricao,
            Preco = produto.Preco,
            CategoriaId = produto.CategoriaId,
            Categoria = produto.Categoria?.Nome,
            Imagem = produto.Imagem,
            InformacaoNutricional = produto.InformacaoNutricional,
            Ingredientes = produto.Ingredientes,
            TempoPreparo = produto.TempoPreparo,
            Sazonal = produto.Sazonal,
            Ativo = produto.Ativo,
            DataCriacao = produto.DataCriacao
        };
    }
}

using ApiRaizesNordeste.Models;
using Microsoft.EntityFrameworkCore;

namespace ApiRaizesNordeste.Data;

/// <summary>
/// Contexto de banco de dados da aplicação API Raízes Nordeste
/// </summary>
public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    // DbSets
    public DbSet<Unidade> Unidades => Set<Unidade>();
    public DbSet<Categoria> Categorias => Set<Categoria>();
    public DbSet<Produto> Produtos => Set<Produto>();
    public DbSet<EstoqueProduto> EstoquesProduto => Set<EstoqueProduto>();
    public DbSet<Cliente> Clientes => Set<Cliente>();
    public DbSet<ClienteLoyalty> ClientesLoyalty => Set<ClienteLoyalty>();
    public DbSet<TransacaoLoyalty> TransacoesLoyalty => Set<TransacaoLoyalty>();
    public DbSet<Pedido> Pedidos => Set<Pedido>();
    public DbSet<ItemPedido> ItensPedidos => Set<ItemPedido>();
    public DbSet<Pagamento> Pagamentos => Set<Pagamento>();
    public DbSet<ConsentimentoLGPD> ConsentimentosLGPD => Set<ConsentimentoLGPD>();
    public DbSet<AuditoriaOperacao> AuditoriasOperacoes => Set<AuditoriaOperacao>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        
        // Configurações de Unidade
        modelBuilder.Entity<Unidade>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Nome).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Localizacao).IsRequired().HasMaxLength(50);
            entity.Property(e => e.Endereco).HasMaxLength(255);
            entity.Property(e => e.Cidade).HasMaxLength(100);
            entity.Property(e => e.Estado).HasMaxLength(2);
            entity.HasQueryFilter(e => !e.Deletado);
            entity.HasIndex(e => e.Cidade);
        });

        // Configurações de Categoria
        modelBuilder.Entity<Categoria>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Nome).IsRequired().HasMaxLength(50);
            entity.HasQueryFilter(e => !e.Deletado);
            entity.HasIndex(e => e.Ordem);
        });

        // Configurações de Produto
        modelBuilder.Entity<Produto>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Nome).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Preco).HasPrecision(10, 2);
            entity.HasOne(e => e.Categoria)
                .WithMany(c => c.Produtos)
                .HasForeignKey(e => e.CategoriaId)
                .OnDelete(DeleteBehavior.Restrict);
            entity.HasQueryFilter(e => !e.Deletado);
            entity.HasIndex(e => e.CategoriaId);
        });

        // Configurações de EstoqueProduto
        modelBuilder.Entity<EstoqueProduto>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasOne(e => e.Produto)
                .WithMany(p => p.Estoques)
                .HasForeignKey(e => e.ProdutoId)
                .OnDelete(DeleteBehavior.Cascade);
            entity.HasOne(e => e.Unidade)
                .WithMany(u => u.Estoques)
                .HasForeignKey(e => e.UnidadeId)
                .OnDelete(DeleteBehavior.Cascade);
            entity.HasQueryFilter(e => !e.Deletado);
            entity.HasIndex(e => new { e.ProdutoId, e.UnidadeId }).IsUnique();
        });

        // Configurações de Cliente
        modelBuilder.Entity<Cliente>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Email).IsRequired().HasMaxLength(100);
            entity.Property(e => e.NomeCompleto).IsRequired().HasMaxLength(100);
            entity.HasOne(e => e.DadosLoyalty)
                .WithOne(l => l.Cliente)
                .HasForeignKey<ClienteLoyalty>(l => l.ClienteId)
                .OnDelete(DeleteBehavior.Cascade);
            entity.HasQueryFilter(e => !e.Deletado);
            entity.HasIndex(e => e.Email).IsUnique();
        });

        // Configurações de ClienteLoyalty
        modelBuilder.Entity<ClienteLoyalty>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.SaldoPontos).HasPrecision(12, 2);
            entity.Property(e => e.PontosAcumulados).HasPrecision(12, 2);
            entity.HasQueryFilter(e => !e.Deletado);
        });

        // Configurações de TransacaoLoyalty
        modelBuilder.Entity<TransacaoLoyalty>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Pontos).HasPrecision(12, 2);
            entity.Property(e => e.SaldoAnterior).HasPrecision(12, 2);
            entity.Property(e => e.SaldoNovo).HasPrecision(12, 2);
            entity.HasOne(e => e.ClienteLoyalty)
                .WithMany(l => l.Transacoes)
                .HasForeignKey(e => e.ClienteLoyaltyId)
                .OnDelete(DeleteBehavior.Cascade);
            entity.HasQueryFilter(e => !e.Deletado);
            entity.HasIndex(e => e.ClienteLoyaltyId);
        });

        // Configurações de Pedido
        modelBuilder.Entity<Pedido>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.NumeroPedido).IsRequired().HasMaxLength(20);
            entity.Property(e => e.ValorTotal).HasPrecision(10, 2);
            entity.Property(e => e.Desconto).HasPrecision(10, 2);
            entity.Property(e => e.ValorFinal).HasPrecision(10, 2);
            entity.Property(e => e.PontosUtilizados).HasPrecision(12, 2);
            entity.HasOne(e => e.Cliente)
                .WithMany(c => c.Pedidos)
                .HasForeignKey(e => e.ClienteId)
                .OnDelete(DeleteBehavior.Restrict);
            entity.HasOne(e => e.Unidade)
                .WithMany(u => u.Pedidos)
                .HasForeignKey(e => e.UnidadeId)
                .OnDelete(DeleteBehavior.Restrict);
            entity.HasQueryFilter(e => !e.Deletado);
            entity.HasIndex(e => e.NumeroPedido).IsUnique();
            entity.HasIndex(e => e.ClienteId);
        });

        // Configurações de ItemPedido
        modelBuilder.Entity<ItemPedido>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.PrecoUnitario).HasPrecision(10, 2);
            entity.Property(e => e.Subtotal).HasPrecision(10, 2);
            entity.HasOne(e => e.Pedido)
                .WithMany(p => p.Itens)
                .HasForeignKey(e => e.PedidoId)
                .OnDelete(DeleteBehavior.Cascade);
            entity.HasOne(e => e.Produto)
                .WithMany(p => p.ItensPedidos)
                .HasForeignKey(e => e.ProdutoId)
                .OnDelete(DeleteBehavior.Restrict);
            entity.HasQueryFilter(e => !e.Deletado);
        });

        // Configurações de Pagamento
        modelBuilder.Entity<Pagamento>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Valor).HasPrecision(10, 2);
            entity.HasOne(e => e.Pedido)
                .WithOne(p => p.Pagamento)
                .HasForeignKey<Pagamento>(p => p.PedidoId)
                .OnDelete(DeleteBehavior.Cascade);
            entity.HasQueryFilter(e => !e.Deletado);
            entity.HasIndex(e => e.IdTransacaoExternal);
        });

        // Configurações de ConsentimentoLGPD
        modelBuilder.Entity<ConsentimentoLGPD>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasOne(e => e.Cliente)
                .WithMany(c => c.Consentimentos)
                .HasForeignKey(e => e.ClienteId)
                .OnDelete(DeleteBehavior.Cascade);
            entity.HasQueryFilter(e => !e.Deletado);
            entity.HasIndex(e => e.ClienteId);
        });

        // Configurações de AuditoriaOperacao
        modelBuilder.Entity<AuditoriaOperacao>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.EntidadeAfetada).HasMaxLength(50);
            entity.Property(e => e.UsuarioExecucao).HasMaxLength(100);
            entity.HasIndex(e => e.IdEntidade);
            entity.HasIndex(e => e.DataCriacao);
            entity.HasIndex(e => e.Tipo);
        });
    }
}

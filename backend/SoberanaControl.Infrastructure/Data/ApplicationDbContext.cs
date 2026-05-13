using Microsoft.EntityFrameworkCore;
using SoberanaControl.Application.Interfaces;
using SoberanaControl.Domain.Entities;

namespace SoberanaControl.Infrastructure.Data;

public class ApplicationDbContext : DbContext, IApplicationDbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

    public DbSet<Usuario> Usuarios { get; set; }
    public DbSet<Obra> Obras { get; set; }
    public DbSet<Fornecedor> Fornecedores { get; set; }
    public DbSet<Categoria> Categorias { get; set; }
    public DbSet<UnidadeMedida> UnidadesMedida { get; set; }
    public DbSet<Produto> Produtos { get; set; }
    public DbSet<Estoque> Estoques { get; set; }
    public DbSet<Movimentacao> Movimentacoes { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Fluent API Configurations
        modelBuilder.Entity<Usuario>().HasKey(e => e.Id);
        
        modelBuilder.Entity<Obra>().HasKey(e => e.Id);

        modelBuilder.Entity<Fornecedor>().HasKey(e => e.Id);

        modelBuilder.Entity<Categoria>().HasKey(e => e.Id);
        
        modelBuilder.Entity<UnidadeMedida>().HasKey(e => e.Id);

        modelBuilder.Entity<Produto>().HasKey(e => e.Id);
        modelBuilder.Entity<Produto>()
            .HasOne(p => p.Categoria)
            .WithMany()
            .HasForeignKey(p => p.CategoriaId);
        modelBuilder.Entity<Produto>()
            .HasOne(p => p.UnidadeMedida)
            .WithMany()
            .HasForeignKey(p => p.UnidadeMedidaId);

        modelBuilder.Entity<Estoque>().HasKey(e => e.Id);
        modelBuilder.Entity<Estoque>()
            .HasOne(e => e.Produto)
            .WithMany()
            .HasForeignKey(e => e.ProdutoId);
        modelBuilder.Entity<Estoque>()
            .HasOne(e => e.Obra)
            .WithMany()
            .HasForeignKey(e => e.ObraId);

        modelBuilder.Entity<Movimentacao>().HasKey(e => e.Id);
        modelBuilder.Entity<Movimentacao>()
            .HasOne(m => m.Produto)
            .WithMany()
            .HasForeignKey(m => m.ProdutoId);
        modelBuilder.Entity<Movimentacao>()
            .HasOne(m => m.ObraOrigem)
            .WithMany()
            .HasForeignKey(m => m.ObraOrigemId);
        modelBuilder.Entity<Movimentacao>()
            .HasOne(m => m.ObraDestino)
            .WithMany()
            .HasForeignKey(m => m.ObraDestinoId);
        modelBuilder.Entity<Movimentacao>()
            .HasOne(m => m.Usuario)
            .WithMany()
            .HasForeignKey(m => m.UsuarioId);
    }
}

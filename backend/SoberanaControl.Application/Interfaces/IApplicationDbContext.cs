using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using SoberanaControl.Domain.Entities;
using System.Threading;
using System.Threading.Tasks;

namespace SoberanaControl.Application.Interfaces
{
    public interface IApplicationDbContext
    {
        DbSet<Produto> Produtos { get; }
        DbSet<Fornecedor> Fornecedores { get; }
        DbSet<Usuario> Usuarios { get; }
        DbSet<Obra> Obras { get; }
        DbSet<Estoque> Estoques { get; }
        DbSet<Movimentacao> Movimentacoes { get; }
        DbSet<Categoria> Categorias { get; }
        DbSet<UnidadeMedida> UnidadesMedida { get; }
        DatabaseFacade Database { get; }
        
        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}

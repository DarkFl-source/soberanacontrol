namespace SoberanaControl.Domain.Entities;

public class Movimentacao : Entity
{
    public Guid ProdutoId { get; private set; }
    public Produto Produto { get; private set; }
    
    public Guid? ObraOrigemId { get; private set; }
    public Obra ObraOrigem { get; private set; }

    public Guid? ObraDestinoId { get; private set; }
    public Obra ObraDestino { get; private set; }

    public TipoMovimentacao Tipo { get; private set; }
    public decimal Quantidade { get; private set; }
    public decimal ValorUnitario { get; private set; }
    public DateTime DataHora { get; private set; }
    
    public Guid UsuarioId { get; private set; }
    public Usuario Usuario { get; private set; }

    protected Movimentacao() { }

    public Movimentacao(Guid produtoId, Guid? obraOrigemId, Guid? obraDestinoId, TipoMovimentacao tipo, decimal quantidade, decimal valorUnitario, Guid usuarioId)
    {
        ProdutoId = produtoId;
        ObraOrigemId = obraOrigemId;
        ObraDestinoId = obraDestinoId;
        Tipo = tipo;
        Quantidade = quantidade;
        ValorUnitario = valorUnitario;
        UsuarioId = usuarioId;
        DataHora = DateTime.UtcNow;
    }
}

namespace SoberanaControl.Domain.Entities;

public class Estoque : Entity
{
    public Guid ProdutoId { get; private set; }
    public Produto Produto { get; private set; }
    public Guid ObraId { get; private set; }
    public Obra Obra { get; private set; }
    public decimal Quantidade { get; private set; }

    protected Estoque() { }

    public Estoque(Guid produtoId, Guid obraId, decimal quantidadeInicial = 0)
    {
        ProdutoId = produtoId;
        ObraId = obraId;
        Quantidade = quantidadeInicial;
    }

    public void Adicionar(decimal quantidade)
    {
        Quantidade += quantidade;
    }

    public void Remover(decimal quantidade)
    {
        if (Quantidade < quantidade)
            throw new Exception("Estoque insuficiente.");
        Quantidade -= quantidade;
    }
}

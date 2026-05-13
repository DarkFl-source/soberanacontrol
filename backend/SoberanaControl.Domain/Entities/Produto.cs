namespace SoberanaControl.Domain.Entities;

public class Produto : Entity
{
    public string CodigoInterno { get; private set; }
    public string Nome { get; private set; }
    public string Descricao { get; private set; }
    public Guid CategoriaId { get; private set; }
    public Categoria Categoria { get; private set; }
    public Guid UnidadeMedidaId { get; private set; }
    public UnidadeMedida UnidadeMedida { get; private set; }
    public decimal EstoqueMinimo { get; private set; }
    public decimal PrecoMedio { get; private set; }

    protected Produto() { }

    public Produto(string codigoInterno, string nome, Guid categoriaId, Guid unidadeMedidaId, decimal estoqueMinimo = 0, string descricao = "")
    {
        CodigoInterno = codigoInterno;
        Nome = nome;
        Descricao = descricao;
        CategoriaId = categoriaId;
        UnidadeMedidaId = unidadeMedidaId;
        EstoqueMinimo = estoqueMinimo;
        PrecoMedio = 0;
    }

    public void AtualizarPrecoMedio(decimal novoPrecoMedio)
    {
        PrecoMedio = novoPrecoMedio;
    }
}

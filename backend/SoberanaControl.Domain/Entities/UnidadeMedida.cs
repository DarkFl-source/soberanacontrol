namespace SoberanaControl.Domain.Entities;

public class UnidadeMedida : Entity
{
    public string Sigla { get; private set; }
    public string Descricao { get; private set; }

    protected UnidadeMedida() { }

    public UnidadeMedida(string sigla, string descricao)
    {
        Sigla = sigla;
        Descricao = descricao;
    }
}

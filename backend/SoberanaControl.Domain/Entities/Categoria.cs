namespace SoberanaControl.Domain.Entities;

public class Categoria : Entity
{
    public string Nome { get; private set; }

    protected Categoria() { }

    public Categoria(string nome)
    {
        Nome = nome;
    }
}

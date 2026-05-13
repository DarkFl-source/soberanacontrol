namespace SoberanaControl.Domain.Entities;

public class Obra : Entity
{
    public string Nome { get; private set; }
    public string Endereco { get; private set; }
    public bool Ativo { get; private set; }

    protected Obra() { }

    public Obra(string nome, string endereco)
    {
        Nome = nome;
        Endereco = endereco;
        Ativo = true;
    }

    public void Atualizar(string nome, string endereco)
    {
        Nome = nome;
        Endereco = endereco;
    }
}

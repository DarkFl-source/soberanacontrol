namespace SoberanaControl.Domain.Entities;

public class Usuario : Entity
{
    public string Nome { get; private set; }
    public string Email { get; private set; }
    public string Perfil { get; private set; } // Admin, Gerente, Almoxarife

    protected Usuario() { }

    public Usuario(string nome, string email, string perfil)
    {
        Nome = nome;
        Email = email;
        Perfil = perfil;
    }
}

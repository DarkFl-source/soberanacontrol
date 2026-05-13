namespace SoberanaControl.Domain.Entities;

public class Fornecedor : Entity
{
    public string Cnpj { get; private set; }
    public string RazaoSocial { get; private set; }
    public string Contato { get; private set; }
    public string Endereco { get; private set; }

    protected Fornecedor() { }

    public Fornecedor(string cnpj, string razaoSocial, string contato = null, string endereco = null)
    {
        Cnpj = cnpj;
        RazaoSocial = razaoSocial;
        Contato = contato;
        Endereco = endereco;
    }
}

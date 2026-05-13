using SoberanaControl.Domain.Entities;

namespace SoberanaControl.Application.DTOs;

public class MovimentacaoCreateDto
{
    public Guid ProdutoId { get; set; }
    public Guid? ObraOrigemId { get; set; }
    public Guid? ObraDestinoId { get; set; }
    public TipoMovimentacao Tipo { get; set; }
    public decimal Quantidade { get; set; }
    public decimal ValorUnitario { get; set; }
}

public class MovimentacaoResponseDto
{
    public Guid Id { get; set; }
    public string ProdutoNome { get; set; }
    public string? ObraOrigemNome { get; set; }
    public string? ObraDestinoNome { get; set; }
    public string Tipo { get; set; }
    public decimal Quantidade { get; set; }
    public decimal ValorUnitario { get; set; }
    public DateTime DataHora { get; set; }
}

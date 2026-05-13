using System;
using System.Collections.Generic;

namespace SoberanaControl.Application.DTOs
{
    public class NFeParsedDto
    {
        public string ChaveAcesso { get; set; } = string.Empty;
        public EmitenteDto Emitente { get; set; } = new EmitenteDto();
        public List<ProdutoNFeDto> Produtos { get; set; } = new List<ProdutoNFeDto>();
    }

    public class EmitenteDto
    {
        public string Cnpj { get; set; } = string.Empty;
        public string RazaoSocial { get; set; } = string.Empty;
        public string NomeFantasia { get; set; } = string.Empty;
    }

    public class ProdutoNFeDto
    {
        public string CodigoInterno { get; set; } = string.Empty;
        public string Descricao { get; set; } = string.Empty;
        public string Unidade { get; set; } = string.Empty;
        public decimal Quantidade { get; set; }
        public decimal ValorUnitario { get; set; }
    }
}

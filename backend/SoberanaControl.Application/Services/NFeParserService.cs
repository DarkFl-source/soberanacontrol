using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using SoberanaControl.Application.DTOs;

namespace SoberanaControl.Application.Services
{
    public class NFeParserService
    {
        public NFeParsedDto ParseFromStream(Stream xmlStream)
        {
            var dto = new NFeParsedDto();
            XDocument doc;
            
            try
            {
                doc = XDocument.Load(xmlStream);
            }
            catch (Exception ex)
            {
                throw new Exception("Falha ao ler o arquivo XML. Certifique-se de que é um XML válido.", ex);
            }

            // O XML da NFe possui namespaces, precisamos considerar isso ou usar LocalName
            // Vamos usar uma abordagem tolerante procurando por LocalName
            
            var infNFe = doc.Descendants().FirstOrDefault(x => x.Name.LocalName == "infNFe");
            if (infNFe == null)
                throw new Exception("Arquivo XML não parece ser uma NFe válida (tag infNFe não encontrada).");

            var idAttribute = infNFe.Attribute("Id");
            if (idAttribute != null && idAttribute.Value.Length > 3)
            {
                dto.ChaveAcesso = idAttribute.Value.Substring(3); // Remove o 'NFe' do começo
            }

            var emitNode = infNFe.Elements().FirstOrDefault(x => x.Name.LocalName == "emit");
            if (emitNode != null)
            {
                dto.Emitente.Cnpj = emitNode.Elements().FirstOrDefault(x => x.Name.LocalName == "CNPJ")?.Value ?? "";
                dto.Emitente.RazaoSocial = emitNode.Elements().FirstOrDefault(x => x.Name.LocalName == "xNome")?.Value ?? "";
                dto.Emitente.NomeFantasia = emitNode.Elements().FirstOrDefault(x => x.Name.LocalName == "xFant")?.Value ?? dto.Emitente.RazaoSocial;
            }

            var detNodes = infNFe.Elements().Where(x => x.Name.LocalName == "det");
            foreach (var det in detNodes)
            {
                var prodNode = det.Elements().FirstOrDefault(x => x.Name.LocalName == "prod");
                if (prodNode != null)
                {
                    var cProd = prodNode.Elements().FirstOrDefault(x => x.Name.LocalName == "cProd")?.Value ?? "";
                    var xProd = prodNode.Elements().FirstOrDefault(x => x.Name.LocalName == "xProd")?.Value ?? "";
                    var uCom = prodNode.Elements().FirstOrDefault(x => x.Name.LocalName == "uCom")?.Value ?? "";
                    
                    decimal qCom = 0m;
                    decimal vUnCom = 0m;

                    var qComStr = prodNode.Elements().FirstOrDefault(x => x.Name.LocalName == "qCom")?.Value;
                    if (qComStr != null) decimal.TryParse(qComStr, System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out qCom);

                    var vUnComStr = prodNode.Elements().FirstOrDefault(x => x.Name.LocalName == "vUnCom")?.Value;
                    if (vUnComStr != null) decimal.TryParse(vUnComStr, System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out vUnCom);

                    dto.Produtos.Add(new ProdutoNFeDto
                    {
                        CodigoInterno = cProd,
                        Descricao = xProd,
                        Unidade = uCom,
                        Quantidade = qCom,
                        ValorUnitario = vUnCom
                    });
                }
            }

            return dto;
        }
    }
}

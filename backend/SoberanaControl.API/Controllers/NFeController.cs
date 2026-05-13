using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SoberanaControl.Application.Services;
using SoberanaControl.Application.UseCases;

namespace SoberanaControl.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class NFeController : ControllerBase
    {
        private readonly NFeParserService _parserService;
        private readonly ImportarNfeUseCase _importarUseCase;

        public NFeController(NFeParserService parserService, ImportarNfeUseCase importarUseCase)
        {
            _parserService = parserService;
            _importarUseCase = importarUseCase;
        }

        [HttpPost("upload")]
        public async Task<IActionResult> UploadNFe([FromForm] IFormFile file, [FromForm] Guid obraId)
        {
            if (file == null || file.Length == 0)
                return BadRequest("O arquivo XML é obrigatório.");

            if (obraId == Guid.Empty)
                return BadRequest("A Obra de destino é obrigatória.");

            if (!file.FileName.EndsWith(".xml", StringComparison.OrdinalIgnoreCase))
                return BadRequest("O arquivo deve ser um XML válido.");

            try
            {
                using var stream = file.OpenReadStream();
                
                // 1. Extrair os dados do XML (Em memória)
                var parsedNfe = _parserService.ParseFromStream(stream);

                // 2. Executar a regra de negócio e salvar no banco
                await _importarUseCase.ExecuteAsync(parsedNfe, obraId);

                return Ok(new { mensagem = "NFe processada e estoque atualizado com sucesso!", chave = parsedNfe.ChaveAcesso });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erro ao processar a NFe: {ex.Message}");
            }
        }
    }
}

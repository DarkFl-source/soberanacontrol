using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SoberanaControl.Application.DTOs;
using SoberanaControl.Domain.Entities;
using SoberanaControl.Application.Interfaces;

namespace SoberanaControl.Application.UseCases
{
    public class ImportarNfeUseCase
    {
        private readonly IApplicationDbContext _dbContext;

        public ImportarNfeUseCase(IApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task ExecuteAsync(NFeParsedDto nfe, Guid obraDestinoId)
        {
            using var transaction = await _dbContext.Database.BeginTransactionAsync();

            try
            {
                // Pegar o primeiro usuario como System User para a Movimentacao (MVP)
                var usuarioId = await _dbContext.Usuarios.Select(u => u.Id).FirstOrDefaultAsync();
                if (usuarioId == Guid.Empty) usuarioId = Guid.NewGuid();

                // 1. Processar Fornecedor
                var fornecedor = await _dbContext.Fornecedores
                    .FirstOrDefaultAsync(f => f.Cnpj == nfe.Emitente.Cnpj);

                if (fornecedor == null)
                {
                    fornecedor = new Fornecedor(nfe.Emitente.Cnpj, nfe.Emitente.RazaoSocial, string.Empty, string.Empty);
                    _dbContext.Fornecedores.Add(fornecedor);
                    await _dbContext.SaveChangesAsync(); // Precisa salvar para pegar o Id
                }

                // Garantir Categoria Padrão para os produtos novos
                var categoriaPadrao = await _dbContext.Categorias.FirstOrDefaultAsync(c => c.Nome == "Geral");
                if (categoriaPadrao == null)
                {
                    categoriaPadrao = new Categoria("Geral");
                    _dbContext.Categorias.Add(categoriaPadrao);
                    await _dbContext.SaveChangesAsync();
                }

                // 2. Processar Produtos
                foreach (var itemXml in nfe.Produtos)
                {
                    var produto = await _dbContext.Produtos
                        .FirstOrDefaultAsync(p => p.CodigoInterno == itemXml.CodigoInterno);

                    // Verifica se precisa criar Unidade de Medida
                    var unidade = await _dbContext.UnidadesMedida
                        .FirstOrDefaultAsync(u => u.Sigla == itemXml.Unidade);
                    
                    if (unidade == null)
                    {
                        unidade = new UnidadeMedida(itemXml.Unidade, itemXml.Unidade);
                        _dbContext.UnidadesMedida.Add(unidade);
                        await _dbContext.SaveChangesAsync();
                    }

                    if (produto == null)
                    {
                        // Produto Novo
                        produto = new Produto(itemXml.CodigoInterno, itemXml.Descricao, categoriaPadrao.Id, unidade.Id, 0);
                        produto.AtualizarPrecoMedio(itemXml.ValorUnitario);
                        
                        _dbContext.Produtos.Add(produto);
                        await _dbContext.SaveChangesAsync();
                    }
                    else
                    {
                        // Produto Existente: Recalcular Preço Médio Ponderado
                        var estoqueTotal = await _dbContext.Estoques
                            .Where(e => e.ProdutoId == produto.Id)
                            .SumAsync(e => e.Quantidade);

                        var valorTotalAtual = estoqueTotal * produto.PrecoMedio;
                        var valorTotalEntrada = itemXml.Quantidade * itemXml.ValorUnitario;
                        
                        var novoEstoqueTotal = estoqueTotal + itemXml.Quantidade;
                        
                        if (novoEstoqueTotal > 0)
                        {
                            produto.AtualizarPrecoMedio((valorTotalAtual + valorTotalEntrada) / novoEstoqueTotal);
                            _dbContext.Produtos.Update(produto);
                        }
                    }

                    // 3. Registrar Movimentacao
                    var movimentacao = new Movimentacao(
                        produtoId: produto.Id,
                        obraOrigemId: null, // É uma entrada de fornecedor
                        obraDestinoId: obraDestinoId,
                        tipo: TipoMovimentacao.Entrada,
                        quantidade: itemXml.Quantidade,
                        valorUnitario: itemXml.ValorUnitario,
                        usuarioId: usuarioId
                    );
                    _dbContext.Movimentacoes.Add(movimentacao);

                    // 4. Atualizar Estoque na Obra
                    var estoqueObra = await _dbContext.Estoques
                        .FirstOrDefaultAsync(e => e.ProdutoId == produto.Id && e.ObraId == obraDestinoId);

                    if (estoqueObra == null)
                    {
                        estoqueObra = new Estoque(produto.Id, obraDestinoId, itemXml.Quantidade);
                        _dbContext.Estoques.Add(estoqueObra);
                    }
                    else
                    {
                        estoqueObra.Adicionar(itemXml.Quantidade);
                        _dbContext.Estoques.Update(estoqueObra);
                    }
                }

                await _dbContext.SaveChangesAsync();
                await transaction.CommitAsync();
            }
            catch (Exception)
            {
                await transaction.RollbackAsync();
                throw;
            }
        }
    }
}

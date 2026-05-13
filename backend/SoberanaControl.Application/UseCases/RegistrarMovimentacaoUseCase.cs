using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SoberanaControl.Application.DTOs;
using SoberanaControl.Domain.Entities;
using SoberanaControl.Application.Interfaces;

namespace SoberanaControl.Application.UseCases;

public class RegistrarMovimentacaoUseCase
{
    private readonly IApplicationDbContext _dbContext;

    public RegistrarMovimentacaoUseCase(IApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task ExecuteAsync(MovimentacaoCreateDto dto)
    {
        using var transaction = await _dbContext.Database.BeginTransactionAsync();

        try
        {
            // 1. Pegar usuário (Garantir que existe um usuário para a FK)
            var usuario = await _dbContext.Usuarios.FirstOrDefaultAsync();
            if (usuario == null) 
                throw new Exception("Nenhum usuário encontrado no sistema. Por favor, execute o Seed do banco.");
            
            var usuarioId = usuario.Id;

            // 2. Validações de Negócio (Sem "gambiarra", validação rigorosa)
            if (dto.ProdutoId == Guid.Empty) throw new Exception("O Produto é obrigatório.");
            if (dto.Quantidade <= 0) throw new Exception("A quantidade deve ser maior que zero.");

            Guid? origemId = dto.ObraOrigemId == Guid.Empty ? null : dto.ObraOrigemId;
            Guid? destinoId = dto.ObraDestinoId == Guid.Empty ? null : dto.ObraDestinoId;

            // Validação baseada no Tipo
            switch (dto.Tipo)
            {
                case TipoMovimentacao.Entrada:
                    if (destinoId == null) throw new Exception("Para Entradas, a Obra de Destino é obrigatória.");
                    break;
                case TipoMovimentacao.Saida:
                    if (origemId == null) throw new Exception("Para Saídas, a Obra de Origem é obrigatória.");
                    break;
                case TipoMovimentacao.Transferencia:
                    if (origemId == null || destinoId == null) 
                        throw new Exception("Para Transferências, as Obras de Origem e Destino são obrigatórias.");
                    if (origemId == destinoId)
                        throw new Exception("A Obra de Origem não pode ser igual à de Destino.");
                    break;
                default:
                    throw new Exception("Tipo de movimentação inválido.");
            }

            Console.WriteLine($"[DEBUG] Registrando movimentação: Tipo={dto.Tipo}, Produto={dto.ProdutoId}, Qtd={dto.Quantidade}");

            // 3. Criar registro de movimentação
            var movimentacao = new Movimentacao(
                dto.ProdutoId,
                origemId,
                destinoId,
                dto.Tipo,
                dto.Quantidade,
                dto.ValorUnitario,
                usuarioId
            );

            _dbContext.Movimentacoes.Add(movimentacao);
            await _dbContext.SaveChangesAsync(); 

            // 4. Atualizar estoque(s)
            if (dto.Tipo == TipoMovimentacao.Entrada)
            {
                await AtualizarEstoque(dto.ProdutoId, destinoId!.Value, dto.Quantidade, true);
            }
            else if (dto.Tipo == TipoMovimentacao.Saida)
            {
                await AtualizarEstoque(dto.ProdutoId, origemId!.Value, dto.Quantidade, false);
            }
            else if (dto.Tipo == TipoMovimentacao.Transferencia)
            {
                await AtualizarEstoque(dto.ProdutoId, origemId!.Value, dto.Quantidade, false);
                await AtualizarEstoque(dto.ProdutoId, destinoId!.Value, dto.Quantidade, true);
            }

            // Salvar atualizações de estoque e confirmar transação
            await _dbContext.SaveChangesAsync();
            await transaction.CommitAsync();
            Console.WriteLine("[DEBUG] Movimentação e estoques persistidos com sucesso.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[ERROR] Falha na persistência: {ex.Message}");
            await transaction.RollbackAsync();
            throw;
        }
    }

    private async Task AtualizarEstoque(Guid produtoId, Guid obraId, decimal quantidade, bool adicionar)
    {
        var estoque = await _dbContext.Estoques
            .FirstOrDefaultAsync(e => e.ProdutoId == produtoId && e.ObraId == obraId);

        if (estoque == null)
        {
            if (!adicionar) throw new Exception("Não há estoque disponível para esta obra.");
            
            estoque = new Estoque(produtoId, obraId, quantidade);
            _dbContext.Estoques.Add(estoque);
        }
        else
        {
            if (adicionar)
            {
                estoque.Adicionar(quantidade);
            }
            else
            {
                estoque.Remover(quantidade);
            }
            _dbContext.Estoques.Update(estoque);
        }
    }
}

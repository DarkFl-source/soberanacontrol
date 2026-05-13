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
            // 1. Pegar usuário (Mocked as first user for now)
            var usuarioId = await _dbContext.Usuarios.Select(u => u.Id).FirstOrDefaultAsync();
            if (usuarioId == Guid.Empty) usuarioId = Guid.NewGuid();

            // 2. Criar registro de movimentação
            var movimentacao = new Movimentacao(
                dto.ProdutoId,
                dto.ObraOrigemId,
                dto.ObraDestinoId,
                dto.Tipo,
                dto.Quantidade,
                dto.ValorUnitario,
                usuarioId
            );

            _dbContext.Movimentacoes.Add(movimentacao);

            // 3. Atualizar estoque(s)
            if (dto.Tipo == TipoMovimentacao.Entrada)
            {
                if (dto.ObraDestinoId == null) throw new Exception("Obra de destino é obrigatória para Entrada.");
                await AtualizarEstoque(dto.ProdutoId, dto.ObraDestinoId.Value, dto.Quantidade, true);
            }
            else if (dto.Tipo == TipoMovimentacao.Saida)
            {
                if (dto.ObraOrigemId == null) throw new Exception("Obra de origem é obrigatória para Saída.");
                await AtualizarEstoque(dto.ProdutoId, dto.ObraOrigemId.Value, dto.Quantidade, false);
            }
            else if (dto.Tipo == TipoMovimentacao.Transferencia)
            {
                if (dto.ObraOrigemId == null || dto.ObraDestinoId == null) 
                    throw new Exception("Obras de origem e destino são obrigatórias para Transferência.");
                
                await AtualizarEstoque(dto.ProdutoId, dto.ObraOrigemId.Value, dto.Quantidade, false);
                await AtualizarEstoque(dto.ProdutoId, dto.ObraDestinoId.Value, dto.Quantidade, true);
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

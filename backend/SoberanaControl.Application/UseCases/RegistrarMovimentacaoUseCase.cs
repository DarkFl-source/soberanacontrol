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

            // 2. Normalizar IDs vazios para nulo (evita erro de FK com Guid.Empty)
            var origemId = dto.ObraOrigemId == Guid.Empty ? null : dto.ObraOrigemId;
            var destinoId = dto.ObraDestinoId == Guid.Empty ? null : dto.ObraDestinoId;

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

            // 4. Atualizar estoque(s)
            if (dto.Tipo == TipoMovimentacao.Entrada)
            {
                if (destinoId == null) throw new Exception("Obra de destino é obrigatória para Entrada.");
                await AtualizarEstoque(dto.ProdutoId, destinoId.Value, dto.Quantidade, true);
            }
            else if (dto.Tipo == TipoMovimentacao.Saida)
            {
                if (origemId == null) throw new Exception("Obra de origem é obrigatória para Saída.");
                await AtualizarEstoque(dto.ProdutoId, origemId.Value, dto.Quantidade, false);
            }
            else if (dto.Tipo == TipoMovimentacao.Transferencia)
            {
                if (origemId == null || destinoId == null) 
                    throw new Exception("Obras de origem e destino são obrigatórias para Transferência.");
                
                await AtualizarEstoque(dto.ProdutoId, origemId.Value, dto.Quantidade, false);
                await AtualizarEstoque(dto.ProdutoId, destinoId.Value, dto.Quantidade, true);
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

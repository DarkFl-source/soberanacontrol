using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SoberanaControl.Domain.Entities;

namespace SoberanaControl.Infrastructure.Data
{
    public static class DbSeeder
    {
        public static async Task SeedAsync(ApplicationDbContext context)
        {
            // 1. Usuarios
            if (!await context.Usuarios.AnyAsync())
            {
                var admin = new Usuario("Admin", "admin@soberana.com", "123456");
                context.Usuarios.Add(admin);
                await context.SaveChangesAsync();
            }

            var usuario = await context.Usuarios.FirstAsync();

            // 2. Obras
            if (!await context.Obras.AnyAsync())
            {
                context.Obras.AddRange(
                    new Obra("Residencial Alpha", "Rua das Flores, 123"),
                    new Obra("Torre Empresarial Sul", "Av. Paulista, 1000"),
                    new Obra("Reforma Almoxarifado Central", "Rua Industrial, 50")
                );
                await context.SaveChangesAsync();
            }

            var obra1 = await context.Obras.FirstAsync(o => o.Nome == "Residencial Alpha");
            var obra2 = await context.Obras.FirstAsync(o => o.Nome == "Torre Empresarial Sul");

            // 3. Fornecedores
            if (!await context.Fornecedores.AnyAsync())
            {
                context.Fornecedores.AddRange(
                    new Fornecedor("11.111.111/0001-11", "Votorantim Cimentos", "João - 11999999999", "São Paulo, SP"),
                    new Fornecedor("22.222.222/0001-22", "Tintas Coral S.A.", "Maria - 11888888888", "Mauá, SP"),
                    new Fornecedor("33.333.333/0001-33", "Gerdau Aços", "Carlos - 11777777777", "Guarulhos, SP")
                );
                await context.SaveChangesAsync();
            }

            // 4. Categorias e Unidades de Medida
            if (!await context.Categorias.AnyAsync())
            {
                context.Categorias.AddRange(
                    new Categoria("Alvenaria"),
                    new Categoria("Acabamento"),
                    new Categoria("Estrutural"),
                    new Categoria("Geral")
                );
                await context.SaveChangesAsync();
            }

            if (!await context.UnidadesMedida.AnyAsync())
            {
                context.UnidadesMedida.AddRange(
                    new UnidadeMedida("SC", "Saco"),
                    new UnidadeMedida("LT", "Litro"),
                    new UnidadeMedida("KG", "Quilograma"),
                    new UnidadeMedida("UN", "Unidade")
                );
                await context.SaveChangesAsync();
            }

            var catAlvenaria = await context.Categorias.FirstAsync(c => c.Nome == "Alvenaria");
            var catAcabamento = await context.Categorias.FirstAsync(c => c.Nome == "Acabamento");
            var catEstrutural = await context.Categorias.FirstAsync(c => c.Nome == "Estrutural");

            var unSc = await context.UnidadesMedida.FirstAsync(u => u.Sigla == "SC");
            var unLt = await context.UnidadesMedida.FirstAsync(u => u.Sigla == "LT");
            var unKg = await context.UnidadesMedida.FirstAsync(u => u.Sigla == "KG");

            // 5. Produtos
            if (!await context.Produtos.AnyAsync())
            {
                var p1 = new Produto("CIM-001", "Cimento CP II 50kg", catAlvenaria.Id, unSc.Id, 100);
                p1.AtualizarPrecoMedio(35.50m);

                var p2 = new Produto("TIN-001", "Tinta Acrilica Branca 18L", catAcabamento.Id, unLt.Id, 20);
                p2.AtualizarPrecoMedio(250.00m);

                var p3 = new Produto("ACO-001", "Vergalhao 10mm 12m", catEstrutural.Id, unKg.Id, 500);
                p3.AtualizarPrecoMedio(8.90m);

                context.Produtos.AddRange(p1, p2, p3);
                await context.SaveChangesAsync();
            }

            var cimento = await context.Produtos.FirstAsync(p => p.CodigoInterno == "CIM-001");
            var tinta = await context.Produtos.FirstAsync(p => p.CodigoInterno == "TIN-001");

            // 6. Estoque e Movimentação (Transações iniciais)
            if (!await context.Estoques.AnyAsync())
            {
                // Estoque Obra 1
                context.Estoques.Add(new Estoque(cimento.Id, obra1.Id, 200));
                context.Estoques.Add(new Estoque(tinta.Id, obra1.Id, 15));

                // Estoque Obra 2
                context.Estoques.Add(new Estoque(cimento.Id, obra2.Id, 50));

                // Movimentações de Saldo Inicial
                context.Movimentacoes.AddRange(
                    new Movimentacao(cimento.Id, null, obra1.Id, TipoMovimentacao.Entrada, 200, 35.50m, usuario.Id),
                    new Movimentacao(tinta.Id, null, obra1.Id, TipoMovimentacao.Entrada, 15, 250.00m, usuario.Id),
                    new Movimentacao(cimento.Id, null, obra2.Id, TipoMovimentacao.Entrada, 50, 35.50m, usuario.Id)
                );

                await context.SaveChangesAsync();
            }
        }
    }
}

import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { ProdutosService } from '../../../core/services/produtos.service';

@Component({
  selector: 'app-produtos-list',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './produtos-list.component.html',
  styleUrl: './produtos-list.component.css'
})
export class ProdutosListComponent implements OnInit {
  produtos: any[] = [];
  categorias: any[] = [];
  unidades: any[] = [];
  isLoading = true;
  showModal = false;
  isSaving = false;
  errorMessage = '';

  novoProduto = { codigoInterno: '', nome: '', categoriaId: '', unidadeMedidaId: '', estoqueMinimo: 0 };

  constructor(private service: ProdutosService) {}

  ngOnInit(): void {
    this.load();
    this.service.getCategorias().subscribe(d => this.categorias = d);
    this.service.getUnidadesMedida().subscribe(d => this.unidades = d);
  }

  load(): void {
    this.isLoading = true;
    this.service.getProdutos().subscribe({
      next: (data) => { this.produtos = data; this.isLoading = false; },
      error: () => { this.isLoading = false; }
    });
  }

  openModal(): void {
    this.novoProduto = { codigoInterno: '', nome: '', categoriaId: '', unidadeMedidaId: '', estoqueMinimo: 0 };
    this.errorMessage = '';
    this.showModal = true;
  }

  closeModal(): void { this.showModal = false; }

  salvar(): void {
    if (!this.novoProduto.nome.trim() || !this.novoProduto.categoriaId || !this.novoProduto.unidadeMedidaId) {
      this.errorMessage = 'Nome, Categoria e Unidade são obrigatórios.';
      return;
    }
    this.isSaving = true;
    this.service.criarProduto(this.novoProduto).subscribe({
      next: () => { this.isSaving = false; this.closeModal(); this.load(); },
      error: () => { this.isSaving = false; this.errorMessage = 'Erro ao salvar. Tente novamente.'; }
    });
  }
}

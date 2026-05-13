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
  showDeleteConfirm = false;
  isSaving = false;
  isDeleting = false;
  errorMessage = '';
  touched = false;

  editingId: string | null = null;
  deletingId: string | null = null;
  deletingNome = '';

  form = { codigoInterno: '', nome: '', categoriaId: '', unidadeMedidaId: '', estoqueMinimo: 0 };

  get isEditMode() { return !!this.editingId; }

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

  openCreate(): void {
    this.editingId = null;
    this.form = { codigoInterno: '', nome: '', categoriaId: '', unidadeMedidaId: '', estoqueMinimo: 0 };
    this.errorMessage = '';
    this.touched = false;
    this.showModal = true;
  }

  openEdit(prod: any): void {
    this.editingId = prod.id;
    this.form = {
      codigoInterno: prod.codigoInterno ?? '',
      nome: prod.nome ?? '',
      categoriaId: prod.categoriaId ?? '',
      unidadeMedidaId: prod.unidadeMedidaId ?? '',
      estoqueMinimo: prod.estoqueMinimo ?? 0
    };
    this.errorMessage = '';
    this.touched = false;
    this.showModal = true;
  }

  closeModal(): void { this.showModal = false; }

  confirmDelete(prod: any): void {
    this.deletingId = prod.id;
    this.deletingNome = prod.nome;
    this.showDeleteConfirm = true;
  }

  cancelDelete(): void { this.showDeleteConfirm = false; this.deletingId = null; }

  deletar(): void {
    if (!this.deletingId) return;
    this.isDeleting = true;
    this.service.deletarProduto(this.deletingId).subscribe({
      next: () => { this.isDeleting = false; this.cancelDelete(); this.load(); },
      error: () => { this.isDeleting = false; this.cancelDelete(); }
    });
  }

  salvar(): void {
    this.touched = true;
    if (!this.form.nome.trim() || !this.form.categoriaId || !this.form.unidadeMedidaId) {
      this.errorMessage = 'Nome, Categoria e Unidade são obrigatórios.';
      return;
    }
    this.isSaving = true;
    const op = this.isEditMode
      ? this.service.atualizarProduto(this.editingId!, this.form)
      : this.service.criarProduto(this.form);
    op.subscribe({
      next: () => { this.isSaving = false; this.closeModal(); this.load(); },
      error: () => { this.isSaving = false; this.errorMessage = 'Erro ao salvar. Tente novamente.'; }
    });
  }
}

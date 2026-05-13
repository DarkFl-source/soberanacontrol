import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { FornecedoresService } from '../../../core/services/fornecedores.service';
import { MaskDirective } from '../../../shared/directives/mask.directive';

@Component({
  selector: 'app-fornecedores-list',
  standalone: true,
  imports: [CommonModule, FormsModule, MaskDirective],
  templateUrl: './fornecedores-list.component.html',
  styleUrl: './fornecedores-list.component.css'
})
export class FornecedoresListComponent implements OnInit {
  fornecedores: any[] = [];
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

  form = { cnpj: '', razaoSocial: '', contato: '', endereco: '' };

  get isEditMode() { return !!this.editingId; }

  constructor(private service: FornecedoresService) {}

  ngOnInit(): void { this.load(); }

  load(): void {
    this.isLoading = true;
    this.service.getFornecedores().subscribe({
      next: (data) => { this.fornecedores = data; this.isLoading = false; },
      error: () => { this.isLoading = false; }
    });
  }

  openCreate(): void {
    this.editingId = null;
    this.form = { cnpj: '', razaoSocial: '', contato: '', endereco: '' };
    this.errorMessage = '';
    this.touched = false;
    this.showModal = true;
  }

  openEdit(forn: any): void {
    this.editingId = forn.id;
    this.form = { cnpj: forn.cnpj ?? '', razaoSocial: forn.razaoSocial ?? '', contato: forn.contato ?? '', endereco: forn.endereco ?? '' };
    this.errorMessage = '';
    this.touched = false;
    this.showModal = true;
  }

  closeModal(): void { this.showModal = false; }

  confirmDelete(forn: any): void {
    this.deletingId = forn.id;
    this.deletingNome = forn.razaoSocial;
    this.showDeleteConfirm = true;
  }

  cancelDelete(): void { this.showDeleteConfirm = false; this.deletingId = null; }

  deletar(): void {
    if (!this.deletingId) return;
    this.isDeleting = true;
    this.service.deletarFornecedor(this.deletingId).subscribe({
      next: () => { this.isDeleting = false; this.cancelDelete(); this.load(); },
      error: () => { this.isDeleting = false; this.cancelDelete(); }
    });
  }

  salvar(): void {
    this.touched = true;
    if (!this.form.razaoSocial.trim() || !this.form.cnpj.trim()) {
      this.errorMessage = 'Razão Social e CNPJ são obrigatórios.';
      return;
    }
    this.isSaving = true;
    const op = this.isEditMode
      ? this.service.atualizarFornecedor(this.editingId!, this.form)
      : this.service.criarFornecedor(this.form);
    op.subscribe({
      next: () => { this.isSaving = false; this.closeModal(); this.load(); },
      error: () => { this.isSaving = false; this.errorMessage = 'Erro ao salvar. Tente novamente.'; }
    });
  }
}

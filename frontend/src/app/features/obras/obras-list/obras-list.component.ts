import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { ObrasService } from '../../../core/services/obras.service';

@Component({
  selector: 'app-obras-list',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './obras-list.component.html',
  styleUrl: './obras-list.component.css'
})
export class ObrasListComponent implements OnInit {
  obras: any[] = [];
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

  form = { nome: '', endereco: '' };

  get isEditMode() { return !!this.editingId; }

  constructor(private service: ObrasService) {}

  ngOnInit(): void { this.load(); }

  load(): void {
    this.isLoading = true;
    this.service.getObras().subscribe({
      next: (data) => { this.obras = data; this.isLoading = false; },
      error: () => { this.isLoading = false; }
    });
  }

  openCreate(): void {
    this.editingId = null;
    this.form = { nome: '', endereco: '' };
    this.errorMessage = '';
    this.touched = false;
    this.showModal = true;
  }

  openEdit(obra: any): void {
    this.editingId = obra.id;
    this.form = { nome: obra.nome ?? '', endereco: obra.endereco ?? '' };
    this.errorMessage = '';
    this.touched = false;
    this.showModal = true;
  }

  closeModal(): void { this.showModal = false; }

  confirmDelete(obra: any): void {
    this.deletingId = obra.id;
    this.deletingNome = obra.nome;
    this.showDeleteConfirm = true;
  }

  cancelDelete(): void { this.showDeleteConfirm = false; this.deletingId = null; }

  deletar(): void {
    if (!this.deletingId) return;
    this.isDeleting = true;
    this.service.deletarObra(this.deletingId).subscribe({
      next: () => { this.isDeleting = false; this.cancelDelete(); this.load(); },
      error: () => { this.isDeleting = false; this.cancelDelete(); }
    });
  }

  salvar(): void {
    this.touched = true;
    if (!this.form.nome.trim()) {
      this.errorMessage = 'O nome da obra é obrigatório.';
      return;
    }
    this.isSaving = true;
    const op = this.isEditMode
      ? this.service.atualizarObra(this.editingId!, this.form)
      : this.service.criarObra(this.form);
    op.subscribe({
      next: () => { this.isSaving = false; this.closeModal(); this.load(); },
      error: () => { this.isSaving = false; this.errorMessage = 'Erro ao salvar. Tente novamente.'; }
    });
  }
}

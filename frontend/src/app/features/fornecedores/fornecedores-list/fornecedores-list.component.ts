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
  isSaving = false;
  errorMessage = '';

  touched = false;
  novoFornecedor = { cnpj: '', razaoSocial: '', contato: '', endereco: '' };

  constructor(private service: FornecedoresService) {}

  ngOnInit(): void { this.load(); }

  load(): void {
    this.isLoading = true;
    this.service.getFornecedores().subscribe({
      next: (data) => { this.fornecedores = data; this.isLoading = false; },
      error: () => { this.isLoading = false; }
    });
  }

  openModal(): void {
    this.novoFornecedor = { cnpj: '', razaoSocial: '', contato: '', endereco: '' };
    this.errorMessage = '';
    this.touched = false;
    this.showModal = true;
  }

  closeModal(): void { this.showModal = false; }

  salvar(): void {
    this.touched = true;
    if (!this.novoFornecedor.razaoSocial.trim() || !this.novoFornecedor.cnpj.trim()) {
      this.errorMessage = 'Razão Social e CNPJ são obrigatórios.';
      return;
    }
    this.isSaving = true;
    this.service.criarFornecedor(this.novoFornecedor).subscribe({
      next: () => { this.isSaving = false; this.closeModal(); this.load(); },
      error: () => { this.isSaving = false; this.errorMessage = 'Erro ao salvar. Tente novamente.'; }
    });
  }
}

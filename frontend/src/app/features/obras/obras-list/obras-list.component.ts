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
  isSaving = false;
  errorMessage = '';

  novaObra = { nome: '', endereco: '' };

  constructor(private obrasService: ObrasService) {}

  ngOnInit(): void {
    this.load();
  }

  load(): void {
    this.isLoading = true;
    this.obrasService.getObras().subscribe({
      next: (data) => { this.obras = data; this.isLoading = false; },
      error: () => { this.isLoading = false; }
    });
  }

  openModal(): void {
    this.novaObra = { nome: '', endereco: '' };
    this.errorMessage = '';
    this.showModal = true;
  }

  closeModal(): void {
    this.showModal = false;
  }

  salvar(): void {
    if (!this.novaObra.nome.trim()) {
      this.errorMessage = 'O nome da obra é obrigatório.';
      return;
    }
    this.isSaving = true;
    this.obrasService.criarObra(this.novaObra).subscribe({
      next: () => {
        this.isSaving = false;
        this.closeModal();
        this.load();
      },
      error: () => {
        this.isSaving = false;
        this.errorMessage = 'Erro ao salvar. Tente novamente.';
      }
    });
  }
}

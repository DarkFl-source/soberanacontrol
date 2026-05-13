import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { NfeService } from '../../../core/services/nfe.service';

@Component({
  selector: 'app-nfe-upload',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './nfe-upload.component.html'
})
export class NfeUploadComponent {
  selectedFile: File | null = null;
  obraDestinoId: string = '';
  isDragging = false;
  isUploading = false;
  uploadSuccess = false;
  errorMessage = '';
  successMessage = '';

  // Mocking obras list for MVP
  obras = [
    { id: '11111111-1111-1111-1111-111111111111', nome: 'Residencial Alpha' },
    { id: '22222222-2222-2222-2222-222222222222', nome: 'Torre Empresarial Sul' }
  ];

  constructor(private nfeService: NfeService) {}

  onDragOver(event: DragEvent) {
    event.preventDefault();
    this.isDragging = true;
  }

  onDragLeave(event: DragEvent) {
    event.preventDefault();
    this.isDragging = false;
  }

  onDrop(event: DragEvent) {
    event.preventDefault();
    this.isDragging = false;
    
    if (event.dataTransfer?.files && event.dataTransfer.files.length > 0) {
      const file = event.dataTransfer.files[0];
      this.handleFile(file);
    }
  }

  onFileSelected(event: any) {
    if (event.target.files && event.target.files.length > 0) {
      const file = event.target.files[0];
      this.handleFile(file);
    }
  }

  private handleFile(file: File) {
    if (file.name.toLowerCase().endsWith('.xml')) {
      this.selectedFile = file;
      this.errorMessage = '';
    } else {
      this.selectedFile = null;
      this.errorMessage = 'Por favor, selecione um arquivo XML válido.';
    }
  }

  uploadFile() {
    if (!this.selectedFile) {
      this.errorMessage = 'Nenhum arquivo selecionado.';
      return;
    }

    if (!this.obraDestinoId) {
      this.errorMessage = 'Selecione uma Obra de destino.';
      return;
    }

    this.isUploading = true;
    this.errorMessage = '';
    this.uploadSuccess = false;

    this.nfeService.uploadNFe(this.selectedFile, this.obraDestinoId).subscribe({
      next: (response) => {
        this.isUploading = false;
        this.uploadSuccess = true;
        this.successMessage = response.mensagem;
        this.selectedFile = null;
      },
      error: (error) => {
        this.isUploading = false;
        this.errorMessage = error.error || 'Erro ao fazer upload da NFe.';
      }
    });
  }
}

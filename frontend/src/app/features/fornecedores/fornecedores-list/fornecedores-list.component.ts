import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FornecedoresService } from '../../../core/services/fornecedores.service';

@Component({
  selector: 'app-fornecedores-list',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './fornecedores-list.component.html',
  styleUrl: './fornecedores-list.component.css'
})
export class FornecedoresListComponent implements OnInit {
  fornecedores: any[] = [];
  isLoading = true;

  constructor(private fornecedoresService: FornecedoresService) {}

  ngOnInit(): void {
    this.fornecedoresService.getFornecedores().subscribe({
      next: (data) => {
        this.fornecedores = data;
        this.isLoading = false;
      },
      error: (err) => {
        console.error('Erro ao buscar fornecedores', err);
        this.isLoading = false;
      }
    });
  }
}

import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ProdutosService } from '../../../core/services/produtos.service';

@Component({
  selector: 'app-produtos-list',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './produtos-list.component.html',
  styleUrl: './produtos-list.component.css'
})
export class ProdutosListComponent implements OnInit {
  produtos: any[] = [];
  isLoading = true;

  constructor(private produtosService: ProdutosService) {}

  ngOnInit(): void {
    this.produtosService.getProdutos().subscribe({
      next: (data) => {
        this.produtos = data;
        this.isLoading = false;
      },
      error: (err) => {
        console.error('Erro ao buscar produtos', err);
        this.isLoading = false;
      }
    });
  }
}

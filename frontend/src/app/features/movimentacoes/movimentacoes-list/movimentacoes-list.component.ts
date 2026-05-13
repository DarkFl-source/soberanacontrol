import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MovimentacoesService, Movimentacao } from '../../../core/services/movimentacoes.service';

@Component({
  selector: 'app-movimentacoes-list',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './movimentacoes-list.component.html',
  styleUrls: ['./movimentacoes-list.component.css']
})
export class MovimentacoesListComponent implements OnInit {
  movimentacoes: Movimentacao[] = [];
  isLoading = true;

  constructor(private movService: MovimentacoesService) {}

  ngOnInit(): void {
    this.loadMovimentacoes();
  }

  loadMovimentacoes(): void {
    this.isLoading = true;
    this.movService.getMovimentacoes().subscribe({
      next: (data) => {
        this.movimentacoes = data;
        this.isLoading = false;
      },
      error: (err) => {
        console.error('Erro ao carregar movimentações', err);
        this.isLoading = false;
      }
    });
  }

  getTipoBadgeClass(tipo: string): string {
    switch (tipo) {
      case 'Entrada': return 'bg-emerald-50 text-emerald-600 border-emerald-100';
      case 'Saida': return 'bg-red-50 text-red-600 border-red-100';
      case 'Transferencia': return 'bg-blue-50 text-blue-600 border-blue-100';
      default: return 'bg-slate-50 text-slate-600 border-slate-100';
    }
  }
}

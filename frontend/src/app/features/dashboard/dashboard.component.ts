import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-dashboard',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './dashboard.component.html',
  styleUrl: './dashboard.component.css'
})
export class DashboardComponent implements OnInit {
  stats = [
    { label: 'Valor Total em Estoque', value: 'R$ 1.245.890,00', icon: 'currency', color: 'bg-emerald-500', trend: '+12%', trendUp: true },
    { label: 'Obras em Andamento', value: '14', icon: 'office', color: 'bg-blue-500', trend: '+2', trendUp: true },
    { label: 'Alertas de Estoque', value: '08', icon: 'alert', color: 'bg-amber-500', trend: '-15%', trendUp: false },
    { label: 'NFes Processadas', value: '256', icon: 'document', color: 'bg-purple-500', trend: '+34', trendUp: true }
  ];

  estoquePorObra = [
    { obra: 'Residencial Aurora', total: 450000, porcentagem: 85 },
    { obra: 'Torre Empresarial Sul', total: 320000, porcentagem: 60 },
    { obra: 'Shopping Central', total: 180000, porcentagem: 35 },
    { obra: 'Condomínio Vale Verde', total: 295000, porcentagem: 55 }
  ];

  recentes = [
    { id: '#NF-1245', data: '2026-05-12', obra: 'Residencial Aurora', fornecedor: 'Votorantim S.A.', valor: 'R$ 12.450,00', status: 'Processado' },
    { id: '#NF-1244', data: '2026-05-12', obra: 'Shopping Central', fornecedor: 'Gerdau Metais', valor: 'R$ 8.900,00', status: 'Processado' },
    { id: '#NF-1243', data: '2026-05-11', obra: 'Torre Empresarial', fornecedor: 'Leroy Merlin', valor: 'R$ 2.150,00', status: 'Processado' },
    { id: '#NF-1242', data: '2026-05-11', obra: 'Residencial Aurora', fornecedor: 'C&C Materiais', valor: 'R$ 5.400,00', status: 'Erro' }
  ];

  ngOnInit(): void {}
}

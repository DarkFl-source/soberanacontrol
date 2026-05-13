import { Routes } from '@angular/router';
import { BaseLayoutComponent } from './layout/base-layout/base-layout.component';
import { ObrasListComponent } from './features/obras/obras-list/obras-list.component';
import { FornecedoresListComponent } from './features/fornecedores/fornecedores-list/fornecedores-list.component';
import { DashboardComponent } from './features/dashboard/dashboard.component';
import { ProdutosListComponent } from './features/produtos/produtos-list/produtos-list.component';
import { MovimentacoesListComponent } from './features/movimentacoes/movimentacoes-list/movimentacoes-list.component';

export const routes: Routes = [
  {
    path: '',
    component: BaseLayoutComponent,
    children: [
      { path: 'dashboard', component: DashboardComponent },
      { path: 'obras', component: ObrasListComponent },
      { path: 'fornecedores', component: FornecedoresListComponent },
      { path: 'produtos', component: ProdutosListComponent },
      { path: 'movimentacoes', component: MovimentacoesListComponent },
      { path: '', redirectTo: 'dashboard', pathMatch: 'full' }
    ]
  }
];

import { Routes } from '@angular/router';
import { BaseLayoutComponent } from './layout/base-layout/base-layout.component';
import { ObrasListComponent } from './features/obras/obras-list/obras-list.component';
import { FornecedoresListComponent } from './features/fornecedores/fornecedores-list/fornecedores-list.component';
import { ProdutosListComponent } from './features/produtos/produtos-list/produtos-list.component';

export const routes: Routes = [
  {
    path: '',
    component: BaseLayoutComponent,
    children: [
      { path: 'obras', component: ObrasListComponent },
      { path: 'fornecedores', component: FornecedoresListComponent },
      { path: 'produtos', component: ProdutosListComponent },
      { path: '', redirectTo: 'obras', pathMatch: 'full' }
    ]
  }
];

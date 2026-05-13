import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';

export interface Movimentacao {
  id?: string;
  produtoId: string;
  produtoNome?: string;
  obraOrigemId?: string;
  obraOrigemNome?: string;
  obraDestinoId?: string;
  obraDestinoNome?: string;
  tipo: number | string;
  quantidade: number;
  valorUnitario: number;
  dataHora?: string;
}

@Injectable({
  providedIn: 'root'
})
export class MovimentacoesService {
  private apiUrl = `${environment.apiUrl}/movimentacoes`;

  constructor(private http: HttpClient) { }

  getMovimentacoes(): Observable<Movimentacao[]> {
    return this.http.get<Movimentacao[]>(this.apiUrl);
  }

  registrarMovimentacao(mov: Movimentacao): Observable<any> {
    return this.http.post(this.apiUrl, mov);
  }
}

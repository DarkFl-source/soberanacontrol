import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';

@Injectable({
  providedIn: 'root'
})
export class ProdutosService {
  private apiUrl = `${environment.apiUrl}/produtos`;

  constructor(private http: HttpClient) { }

  getProdutos(): Observable<any[]> {
    return this.http.get<any[]>(this.apiUrl);
  }

  criarProduto(data: any): Observable<any> {
    return this.http.post<any>(this.apiUrl, data);
  }

  getCategorias(): Observable<any[]> {
    return this.http.get<any[]>(`${environment.apiUrl}/categorias`);
  }

  getUnidadesMedida(): Observable<any[]> {
    return this.http.get<any[]>(`${environment.apiUrl}/unidadesmedida`);
  }
}

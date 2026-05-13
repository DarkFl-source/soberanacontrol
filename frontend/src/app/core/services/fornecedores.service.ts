import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';

@Injectable({
  providedIn: 'root'
})
export class FornecedoresService {
  private apiUrl = `${environment.apiUrl}/fornecedores`;

  constructor(private http: HttpClient) { }

  getFornecedores(): Observable<any[]> {
    return this.http.get<any[]>(this.apiUrl);
  }

  criarFornecedor(data: any): Observable<any> {
    return this.http.post<any>(this.apiUrl, data);
  }

  atualizarFornecedor(id: string, data: any): Observable<any> {
    return this.http.put<any>(`${this.apiUrl}/${id}`, data);
  }

  deletarFornecedor(id: string): Observable<any> {
    return this.http.delete<any>(`${this.apiUrl}/${id}`);
  }
}

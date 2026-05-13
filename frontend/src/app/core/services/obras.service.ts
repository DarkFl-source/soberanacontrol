import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';

@Injectable({
  providedIn: 'root'
})
export class ObrasService {
  private apiUrl = `${environment.apiUrl}/obras`;

  constructor(private http: HttpClient) { }

  getObras(): Observable<any[]> {
    return this.http.get<any[]>(this.apiUrl);
  }

  criarObra(data: { nome: string; endereco: string }): Observable<any> {
    return this.http.post<any>(this.apiUrl, data);
  }
}

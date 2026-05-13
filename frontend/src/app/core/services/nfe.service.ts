import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class NfeService {
  private apiUrl = 'https://soberana-api.onrender.com/api/nfe';

  constructor(private http: HttpClient) { }

  uploadNFe(file: File, obraId: string): Observable<any> {
    const formData = new FormData();
    formData.append('file', file);
    formData.append('obraId', obraId);

    return this.http.post(`${this.apiUrl}/upload`, formData);
  }
}

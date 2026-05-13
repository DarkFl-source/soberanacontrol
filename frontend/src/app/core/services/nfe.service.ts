import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

import { environment } from '../../../environments/environment';

@Injectable({
  providedIn: 'root'
})
export class NfeService {
  private apiUrl = `${environment.apiUrl}/nfe`;

  constructor(private http: HttpClient) { }

  uploadNFe(file: File, obraId: string): Observable<any> {
    const formData = new FormData();
    formData.append('file', file);
    formData.append('obraId', obraId);

    return this.http.post(`${this.apiUrl}/upload`, formData);
  }
}

import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ObrasService } from '../../../core/services/obras.service';

@Component({
  selector: 'app-obras-list',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './obras-list.component.html',
  styleUrl: './obras-list.component.css'
})
export class ObrasListComponent implements OnInit {
  obras: any[] = [];
  isLoading = true;

  constructor(private obrasService: ObrasService) {}

  ngOnInit(): void {
    this.obrasService.getObras().subscribe({
      next: (data) => {
        this.obras = data;
        this.isLoading = false;
      },
      error: (err) => {
        console.error('Erro ao buscar obras', err);
        this.isLoading = false;
      }
    });
  }
}

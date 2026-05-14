import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { MovimentacoesService, Movimentacao } from '../../../core/services/movimentacoes.service';
import { ProdutosService } from '../../../core/services/produtos.service';
import { ObrasService } from '../../../core/services/obras.service';

@Component({
  selector: 'app-movimentacao-form',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule],
  templateUrl: './movimentacao-form.component.html'
})
export class MovimentacaoFormComponent implements OnInit {
  @Output() onSave = new EventEmitter<void>();
  @Output() onClose = new EventEmitter<void>();

  form: FormGroup;
  produtos: any[] = [];
  obras: any[] = [];
  isSaving = false;

  tipos = [
    { value: 1, label: 'Entrada (Ajuste/Avulsa)' },
    { value: 2, label: 'Saída (Consumo/Obra)' },
    { value: 3, label: 'Transferência entre Obras' }
  ];

  constructor(
    private fb: FormBuilder,
    private movService: MovimentacoesService,
    private prodService: ProdutosService,
    private obraService: ObrasService
  ) {
    this.form = this.fb.group({
      produtoId: ['', Validators.required],
      tipo: [1, Validators.required],
      obraOrigemId: [''],
      obraDestinoId: [''],
      quantidade: [0, [Validators.required, Validators.min(0.01)]],
      valorUnitario: [0, [Validators.required, Validators.min(0)]]
    });
  }

  ngOnInit(): void {
    this.loadInitialData();
    
    // Validar obras dinamicamente com base no tipo
    this.form.get('tipo')?.valueChanges.subscribe(tipo => {
      this.updateValidators(tipo);
    });
    
    this.updateValidators(this.form.get('tipo')?.value);
  }

  loadInitialData(): void {
    this.prodService.getProdutos().subscribe(data => this.produtos = data);
    this.obraService.getObras().subscribe(data => this.obras = data);
  }

  updateValidators(tipo: number): void {
    const orig = this.form.get('obraOrigemId');
    const dest = this.form.get('obraDestinoId');

    orig?.clearValidators();
    dest?.clearValidators();

    if (tipo == 1) { // Entrada
      dest?.setValidators([Validators.required]);
    } else if (tipo == 2) { // Saida
      orig?.setValidators([Validators.required]);
    } else if (tipo == 3) { // Transferencia
      orig?.setValidators([Validators.required]);
      dest?.setValidators([Validators.required]);
    }

    orig?.updateValueAndValidity();
    dest?.updateValueAndValidity();
  }

  save(): void {
    if (this.form.invalid) return;

    const raw = this.form.value;

    // .NET Guid? cannot deserialize empty string '' — must be null
    const payload = {
      produtoId: raw.produtoId || null,
      obraOrigemId: raw.obraOrigemId || null,
      obraDestinoId: raw.obraDestinoId || null,
      tipo: Number(raw.tipo),
      quantidade: Number(raw.quantidade),
      valorUnitario: Number(raw.valorUnitario)
    };

    this.isSaving = true;
    this.movService.registrarMovimentacao(payload).subscribe({
      next: () => {
        this.isSaving = false;
        this.onSave.emit();
      },
      error: (err) => {
        const msg = err.error?.message || err.error?.title || JSON.stringify(err.error) || 'Erro ao registrar movimentação';
        alert(msg);
        this.isSaving = false;
      }
    });
  }
}

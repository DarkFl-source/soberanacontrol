import { Directive, ElementRef, HostListener, Input } from '@angular/core';

/**
 * Diretiva de máscara reutilizável.
 * Uso: <input appMask="cnpj">, <input appMask="telefone">, <input appMask="cep">
 */
@Directive({
  selector: '[appMask]',
  standalone: true
})
export class MaskDirective {
  @Input('appMask') maskType: string = '';

  constructor(private el: ElementRef<HTMLInputElement>) {}

  @HostListener('input', ['$event'])
  onInput(event: InputEvent): void {
    const input = this.el.nativeElement;
    const raw = input.value.replace(/\D/g, '');
    const cursor = input.selectionStart ?? 0;

    let masked = '';

    switch (this.maskType) {
      case 'cnpj':
        masked = this.applyMask(raw, '##.###.###/####-##');
        break;
      case 'telefone':
        masked = raw.length <= 10
          ? this.applyMask(raw, '(##) ####-####')
          : this.applyMask(raw, '(##) #####-####');
        break;
      case 'cep':
        masked = this.applyMask(raw, '#####-###');
        break;
      default:
        return;
    }

    input.value = masked;

    // Dispara ngModel update
    input.dispatchEvent(new Event('input', { bubbles: true }));
  }

  @HostListener('blur')
  onBlur(): void {
    // Adiciona classe de validação visual
    const input = this.el.nativeElement;
    const raw = input.value.replace(/\D/g, '');
    
    if (this.maskType === 'cnpj' && raw.length > 0 && raw.length < 14) {
      input.classList.add('border-red-400', 'ring-red-200');
      input.classList.remove('border-slate-300');
    } else if (this.maskType === 'telefone' && raw.length > 0 && raw.length < 10) {
      input.classList.add('border-red-400', 'ring-red-200');
      input.classList.remove('border-slate-300');
    } else {
      input.classList.remove('border-red-400', 'ring-red-200');
      if (!input.classList.contains('border-slate-300')) {
        input.classList.add('border-slate-300');
      }
    }
  }

  private applyMask(raw: string, pattern: string): string {
    let result = '';
    let rawIndex = 0;

    for (let i = 0; i < pattern.length && rawIndex < raw.length; i++) {
      if (pattern[i] === '#') {
        result += raw[rawIndex++];
      } else {
        result += pattern[i];
        // Se o próximo char do raw bate com o separador, pula no raw
        if (raw[rawIndex] === pattern[i]) rawIndex++;
      }
    }

    return result;
  }
}

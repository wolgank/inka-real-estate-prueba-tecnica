import { Component, inject, signal, OnInit, PLATFORM_ID } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http'; // Añadimos HttpHeaders
import { CommonModule, CurrencyPipe, isPlatformBrowser } from '@angular/common';
import { HlmTableImports } from '@spartan-ng/helm/table';
import { HlmButtonImports } from '@spartan-ng/helm/button';
import { environment } from '../../../../environments/environment';

@Component({
  selector: 'app-product-list',
  standalone: true,
  imports: [CommonModule, HlmTableImports, HlmButtonImports, CurrencyPipe],
  template: `
    <div class="p-8 max-w-6xl mx-auto">
      <div class="flex justify-between items-center mb-8">
        <h1 class="text-3xl font-bold tracking-tight text-zinc-900">Inventario</h1>
        <button hlmBtn variant="outline" (click)="downloadReport()" class="border-zinc-300">
          Descargar Reporte PDF
        </button>
      </div>

      <div class="border border-zinc-200 rounded-lg bg-white overflow-hidden shadow-sm">
        <table hlmTable class="w-full">
          <thead hlmTrow class="bg-zinc-50 border-b border-zinc-200">
            <th hlmTh class="p-4 text-left font-semibold text-zinc-700">Producto</th>
            <th hlmTh class="p-4 text-left font-semibold text-zinc-700">Categoría</th>
            <th hlmTh class="p-4 text-right font-semibold text-zinc-700">Precio</th>
            <th hlmTh class="p-4 text-right font-semibold text-zinc-700">Stock</th>
          </thead>
          <tbody hlmTbody>
            @for (p of products(); track p.id) {
              <tr hlmTrow class="hover:bg-zinc-50 transition-colors border-b border-zinc-100 last:border-0">
                <td hlmTd class="p-4 font-medium text-zinc-900">{{ p.name }}</td>
                <td hlmTd class="p-4 text-zinc-600">{{ p.category }}</td>
                <td hlmTd class="p-4 text-right font-mono text-zinc-700">{{ p.price | currency }}</td>
                <td hlmTd class="p-4 text-right font-bold" [class.text-red-500]="p.stock < 5">
                   {{ p.stock }}
                </td>
              </tr>
            }
          </tbody>
        </table>
      </div>
    </div>
  `
})
export class ProductListComponent implements OnInit {
  private http = inject(HttpClient);
  private platformId = inject(PLATFORM_ID);
  
  // Tipado correcto para evitar el error de "any" que me pediste
  products = signal<{id: number, name: string, category: string, price: number, stock: number}[]>([]);

  ngOnInit() {
    // Solo cargamos productos si estamos en el navegador para evitar errores de SSR
    if (isPlatformBrowser(this.platformId)) {
      this.http.get<any[]>(`${environment.apiUrl}/Products`).subscribe({
        next: (data) => this.products.set(data),
        error: (err) => console.error('Error al cargar productos', err)
      });
    }
  }

  downloadReport() {
    if (!isPlatformBrowser(this.platformId)) return;

    const token = localStorage.getItem('token');
    const headers = new HttpHeaders().set('Authorization', `Bearer ${token}`);

    // Hacemos la petición como 'blob' para manejar el PDF binario
    this.http.get(`${environment.apiUrl}/Products/low-stock/report`, {
      headers,
      responseType: 'blob' 
    }).subscribe({
      next: (blob) => {
        // Creamos un link invisible para descargar el archivo
        const url = window.URL.createObjectURL(blob);
        const link = document.createElement('a');
        link.href = url;
        link.download = 'reporte-inka-real-estate.pdf';
        link.click();
        window.URL.revokeObjectURL(url);
      },
      error: (err) => alert('No tienes permisos o hubo un error al generar el PDF')
    });
  }
}
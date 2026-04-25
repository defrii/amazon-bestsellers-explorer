import { Component, OnInit, signal } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './app.html',
  styleUrls: ['./app.scss']
})
export class App implements OnInit {
  // Tworzymy sygnał z początkową pustą tablicą
  public forecasts = signal<any[]>([]);

  constructor(private http: HttpClient) { }

  ngOnInit() {
    this.http.get<any[]>('/api/weatherforecast').subscribe({
      next: (result) => {
        this.forecasts.set(result);
        console.log('Dane załadowane do sygnału:', result);
      },
      error: (err) => console.error('Błąd pobierania:', err)
    });
  }
}

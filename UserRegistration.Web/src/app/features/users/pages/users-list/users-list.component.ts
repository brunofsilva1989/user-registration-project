import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { UsersService, UserDto } from '../../users.service';
import { RouterLink } from '@angular/router';

@Component({
  selector: 'app-users-list',
  standalone: true,
  imports: [CommonModule, RouterLink], 
  template: `
  <div class="wrap">
    <h2>Usuários</h2>
    <p *ngIf="loading">Carregando...</p>
    <p *ngIf="error" class="err">{{ error }}</p>

    <table *ngIf="!loading && users.length" class="grid">
      <thead><tr>
        <th>Login</th><th>Nome</th><th>Email</th><th>Criado em</th>
      </tr></thead>
      <tbody>
        <tr *ngFor="let u of users">
          <td>{{ u.login }}</td>
          <td>{{ u.firstName }} {{ u.lastName }}</td>
          <td>{{ u.email }}</td>
          <td>{{ u.createdAt | date:'short' }}</td>
        </tr>
      </tbody>
    </table>

    <p *ngIf="!loading && !users.length">Nenhum usuário encontrado.</p>

    <div class="mt-3" *ngIf="!loading">
      <a routerLink="/" class="btn btn-outline-secondary">
        <i class="bi bi-house"></i> Voltar para Home
      </a>
    </div>
  </div>
  `,
  styles: [`
    .wrap{max-width:960px;margin:24px auto}
    .grid{width:100%;border-collapse:collapse}
    .grid th,.grid td{border:1px solid #e5e7eb;padding:8px}
    .err{color:#c62828}
  `]
})
export class UsersListComponent implements OnInit {
  users: UserDto[] = [];
  loading = true;
  error = '';

  constructor(private svc: UsersService) {}

  ngOnInit(): void {
    this.svc.getAll().subscribe({
      next: data => { this.users = data; this.loading = false; },
      error: () => { this.error = 'Falha ao carregar usuários'; this.loading = false; }
    });
  }
}

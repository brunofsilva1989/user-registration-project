import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Router, RouterLink } from '@angular/router';
import { AuthService } from '../../auth.service';

@Component({
  selector: 'app-login',
  standalone: true,
  imports: [CommonModule, FormsModule, RouterLink],
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.scss']
})
export class LoginComponent {
  email = '';
  password = '';
  error = '';

  constructor(private auth: AuthService, private router: Router) {}

  submit(): void {
    this.error = '';
    if (!this.email || !this.password) {
      this.error = 'Informe email e senha.';
      return;
    }

    this.auth.login(this.email, this.password).subscribe({
      next: (res) => {
        
        this.router.navigate(['/'], {
          state: { loginOk: true, user: res.user?.firstName ?? res.user?.login ?? res.user?.email }
        });
      },
      error: (err: any) => {
        this.error = err?.error?.message ?? 'Email ou senha inválidos.';
      }
    });
  }
}

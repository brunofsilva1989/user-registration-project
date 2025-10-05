import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router, RouterLink } from '@angular/router';
import {
  FormBuilder,
  Validators,
  FormGroup,
  AbstractControl,
  ValidationErrors,
  ReactiveFormsModule
} from '@angular/forms';
import { UsersService } from '../../users.service';

@Component({
  selector: 'app-user-form',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, RouterLink],
  templateUrl: './user-form.component.html',
  styleUrls: ['./user-form.component.scss']
})
export class UserFormComponent {
  form!: FormGroup;
  saving = false;
  error = '';

  constructor(private fb: FormBuilder, private svc: UsersService, private router: Router) {
    this.form = this.fb.group({
      firstName: ['', [Validators.required, Validators.minLength(2)]],
      lastName: ['', [Validators.required, Validators.minLength(2)]],
      login: ['', [Validators.required, Validators.minLength(3)]],
      email: ['', [Validators.required, Validators.email]],
      passwordGroup: this.fb.group(
        {
          password: ['', [Validators.required, Validators.minLength(8)]],
          confirmPassword: ['', [Validators.required]]
        },
        { validators: this.passwordsMatch }
      )
    });
  }

  get passwordGroup(): FormGroup {
    return this.form.get('passwordGroup') as FormGroup;
  }

  private passwordsMatch(group: AbstractControl): ValidationErrors | null {
    const p = group.get('password')?.value;
    const c = group.get('confirmPassword')?.value;
    return p && c && p !== c ? { passwordsMismatch: true } : null;
  }

  submit(): void {
    if (this.form.invalid) {
      this.form.markAllAsTouched();
      return;
    }

    const { firstName, lastName, login, email, passwordGroup } = this.form.value;
    const payload = {
      firstName,
      lastName,
      login,
      email,
      password: passwordGroup.password
    };

    this.saving = true;
    this.error = '';

    this.svc.create(payload).subscribe({
      next: () => {
        this.saving = false;
        this.router.navigate(['/users']);
      },
      error: (err) => {
        this.saving = false;
        this.error = err?.error?.message ?? 'Email already exists, impossible to register, use a different email.';
      }
    });
  }
}

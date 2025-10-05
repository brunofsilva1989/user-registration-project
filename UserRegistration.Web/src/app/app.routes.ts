import { Routes } from '@angular/router';
import { canActivateAuth } from './core/guards/auth.guard';
import { HomeComponent } from './core/home/home.component';

export const routes: Routes = [
  { path: '', component: HomeComponent },
  {
    path: 'login',
    loadChildren: () => import('./features/auth/auth.module').then(m => m.AuthModule)
  },
  {
    path: 'users',
    canActivate: [canActivateAuth],
    loadChildren: () => import('./features/users/users.module').then(m => m.UsersModule)
  },
  { path: '**', redirectTo: '' }
];

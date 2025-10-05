import { HttpInterceptorFn, HttpErrorResponse } from '@angular/common/http';
import { inject, PLATFORM_ID } from '@angular/core';
import { isPlatformBrowser } from '@angular/common';
import { Router } from '@angular/router';
import { environment } from '../../../environments/environment';
import { AuthService } from '../../features/auth/auth.service';
import { catchError, throwError } from 'rxjs';

export const authInterceptor: HttpInterceptorFn = (req, next) => {
  const platformId = inject(PLATFORM_ID);
  const router = inject(Router);
  const auth = inject(AuthService);

  const token = isPlatformBrowser(platformId) ? localStorage.getItem('jwt_token') : null;

  let cloned = req.clone({ setHeaders: { 'Accept': 'application/json' } });

  const base = environment.apiBaseUrl;
  const url = cloned.url;
  const isApiCall = url.startsWith(`${base}/api/`);
  const isAuth = url.startsWith(`${base}/api/auth/`);

  if (isApiCall && !isAuth && token) {
    cloned = cloned.clone({ setHeaders: { Authorization: `Bearer ${token}` } });
  }

  return next(cloned).pipe(
    catchError((err: unknown) => {
      if (err instanceof HttpErrorResponse && err.status === 401) {
        auth.logout();
        router.navigate(['/'], { state: { sessionExpired: true } });
      }
      return throwError(() => err);
    })
  );
};

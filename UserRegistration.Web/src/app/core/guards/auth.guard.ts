import { inject } from '@angular/core';
import { CanActivateFn, Router } from '@angular/router';
import { AuthService } from '../../features/auth/auth.service';
import { map } from 'rxjs/operators';

export const canActivateAuth: CanActivateFn = (_r, state) => {
  const auth = inject(AuthService);
  const router = inject(Router);

  return auth.session$.pipe(
    map(s => {
      if (s.logged) return true;
      router.navigate(['/login'], { state: { returnUrl: state.url } });
      return false;
    })
  );
};

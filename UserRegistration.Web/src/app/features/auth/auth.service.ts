import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { BehaviorSubject, Observable, tap } from 'rxjs';
import { environment } from '../../../environments/environment';

export type SessionState = {
  logged: boolean;
  token?: string;
  user?: { id?: number; login?: string; username?: string; email?: string; firstName?: string; lastName?: string };
};

type LoginRequest = { email: string; password: string };
type LoginResponse = { token: string; user?: any };

const KEY = 'app.session';
const JWT_KEY = 'jwt_token';

@Injectable({ providedIn: 'root' })
export class AuthService {
  private readonly sessionSubject = new BehaviorSubject<SessionState>(loadFromStorage());
  readonly session$ = this.sessionSubject.asObservable();

  constructor(private http: HttpClient) {}

  get session(): SessionState {
    return this.sessionSubject.value;
  }

  login(email: string, password: string): Observable<LoginResponse> {
    return this.http
      .post<LoginResponse>(`${environment.apiBaseUrl}/api/auth/login`, { email, password } as LoginRequest)
      .pipe(
        tap((res) => {
          const claims = res.user ?? parseJwt(res.token);
          const u = extractUser(claims);

          const state: SessionState = { logged: true, token: res.token, user: u };
          this.sessionSubject.next(state);

          localStorage.setItem(KEY, JSON.stringify(state));
          localStorage.setItem(JWT_KEY, res.token);
        })
      );
  }

  logout(): void {
    const state: SessionState = { logged: false };
    this.sessionSubject.next(state);
    localStorage.setItem(KEY, JSON.stringify(state));
    localStorage.removeItem(JWT_KEY);
  }
}

/* ===== Helpers ===== */

function loadFromStorage(): SessionState {
  try {
    const raw = localStorage.getItem(KEY);
    if (!raw) return { logged: false };

    const state = JSON.parse(raw) as SessionState;

    if (state.logged && !state.user && state.token) {
      state.user = extractUser(parseJwt(state.token));
      
      localStorage.setItem(KEY, JSON.stringify(state));
    }
    return state;
  } catch {
    return { logged: false };
  }
}

function parseJwt(token?: string): any | undefined {
  if (!token) return undefined;
  try {
    const base64 = token.split('.')[1]?.replace(/-/g, '+').replace(/_/g, '/');
    const json = atob(base64);
    return JSON.parse(decodeURIComponent(Array.prototype.map.call(json, (c: string) =>
      '%' + ('00' + c.charCodeAt(0).toString(16)).slice(-2)
    ).join('')));
  } catch {
    try { return JSON.parse(atob(token.split('.')[1] || '')); } catch { return undefined; }
  }
}

function extractUser(c: any): SessionState['user'] | undefined {
  if (!c) return undefined;
  const firstName = c.given_name ?? c.first_name ?? c.firstName ?? '';
  const lastName  = c.family_name ?? c.last_name ?? c.lastName ?? '';
  const login     = c.preferred_username ?? c.unique_name ?? c.username ?? c.name ?? c.sub ?? '';
  const email     = c.email ?? c.upn ?? '';

  const u: any = {};
  if (firstName) u.firstName = firstName;
  if (lastName)  u.lastName  = lastName;
  if (login)     u.login     = login;
  if (email)     u.email     = email;
  return u;
}

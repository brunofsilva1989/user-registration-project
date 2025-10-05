import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { environment } from '../../../environments/environment';
import { Observable } from 'rxjs';
import { map } from 'rxjs/operators';
import { formatDate } from '@angular/common';

export interface CreateUserRequest {
  firstName: string;
  lastName: string;
  login: string;
  email: string;
  password: string;
}

export interface UserDto {
  id: number;
  login: string;
  firstName: string;
  lastName: string;
  email: string;
  createdAt: Date | null;
  createdAtText: string; 
}

@Injectable({ providedIn: 'root' })
export class UsersService {
  private readonly base = `${environment.apiBaseUrl}/api/users`;

  constructor(private http: HttpClient) {}

  private toDate(v: any): Date | null {
    if (!v) return null;
    if (v instanceof Date) return v;
    if (typeof v === 'number') return new Date(v);
    if (typeof v === 'string') {
      const s = v.includes('T') ? v : v.replace(' ', 'T');
      const d = new Date(s);
      return isNaN(d.getTime()) ? null : d;
    }
    return null;
  }

  private mapUser = (r: any): UserDto => {
    const d = this.toDate(
      r?.createdAt ?? r?.createAt ?? r?.created_at ?? r?.createdOn ?? r?.created_on ?? r?.createdDate ?? r?.created
    );
    return {
      id: r?.id ?? r?.userId ?? 0,
      login: r?.login ?? r?.userName ?? r?.username ?? '',
      firstName: r?.firstName ?? r?.first_name ?? '',
      lastName: r?.lastName ?? r?.last_name ?? '',
      email: r?.email ?? '',
      createdAt: d,
      createdAtText: d ? formatDate(d, 'short', 'pt-BR') : '-'
    };
  };

  getAll(): Observable<UserDto[]> {
    return this.http.get<any[]>(this.base).pipe(
      map(list => (Array.isArray(list) ? list.map(this.mapUser) : []))
    );
  }

  create(payload: CreateUserRequest): Observable<UserDto> {
    return this.http.post<any>(this.base, payload).pipe(
      map(res => (res ? this.mapUser(res) : this.mapUser({ ...payload, id: 0 })))
    );
  }
}

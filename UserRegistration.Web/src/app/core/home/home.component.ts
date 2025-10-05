import { Component, Inject, PLATFORM_ID } from '@angular/core';
import { CommonModule, isPlatformBrowser } from '@angular/common';
import { RouterLink } from '@angular/router';
import { AuthService, SessionState } from '../../features/auth/auth.service';

@Component({
  selector: 'app-home',
  standalone: true,
  imports: [CommonModule, RouterLink],
  templateUrl: './home.component.html',
  styleUrls: ['./home.component.scss']
})
export class HomeComponent {
  showSuccess = false;
  user = '';
  session: SessionState = { logged: false };

  get displayName(): string {
    const u: any = this.session?.user || {};
    const first = u.firstName ?? u.first_name ?? u.given_name ?? '';
    const last  = u.lastName  ?? u.last_name  ?? u.family_name ?? '';
    const full  = `${first} ${last}`.trim();

    return full || u.login || u.username || u.email || this.user || '';
  }

  constructor(public auth: AuthService, @Inject(PLATFORM_ID) private pid: Object) {
    auth.session$.subscribe((s: SessionState) => this.session = s);

    if (isPlatformBrowser(this.pid)) {
      const st: any = history.state;
      if (st?.loginOk) {
        this.user = st.user ?? '';
        this.showSuccess = true;
        setTimeout(() => (this.showSuccess = false), 4000);
      }
    }
  }

  logout() { this.auth.logout(); }
}

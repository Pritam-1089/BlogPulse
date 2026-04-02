import { Component } from '@angular/core';
import { RouterOutlet, RouterLink } from '@angular/router';
import { AuthService } from './services/auth.service';

@Component({
  selector: 'app-root',
  imports: [RouterOutlet, RouterLink],
  template: `
    <nav class="navbar">
      <a routerLink="/" class="brand">BlogPulse</a>
      <div class="nav-right">
        @if (auth.isLoggedIn()) {
          <a routerLink="/create" class="btn-write">Write</a>
          <a [routerLink]="['/profile', auth.user()?.userId]" class="nav-link">{{ auth.user()?.fullName }}</a>
          <button class="btn-logout" (click)="auth.logout()">Logout</button>
        } @else {
          <a routerLink="/login" class="nav-link">Login</a>
          <a routerLink="/register" class="btn-primary">Get Started</a>
        }
      </div>
    </nav>
    <main><router-outlet /></main>
  `,
  styles: [`
    .navbar { display: flex; align-items: center; justify-content: space-between; padding: 0 32px; height: 60px; background: #fff; border-bottom: 1px solid #e5e7eb; }
    .brand { font-size: 22px; font-weight: 800; color: #7c3aed; text-decoration: none; }
    .nav-right { display: flex; align-items: center; gap: 16px; }
    .nav-link { color: #374151; text-decoration: none; font-size: 14px; }
    .nav-link:hover { color: #7c3aed; }
    .btn-write { background: #7c3aed; color: #fff; padding: 8px 20px; border-radius: 20px; text-decoration: none; font-size: 14px; font-weight: 600; }
    .btn-primary { background: #7c3aed; color: #fff; padding: 8px 20px; border-radius: 20px; text-decoration: none; font-size: 14px; }
    .btn-logout { background: none; border: 1px solid #d1d5db; padding: 6px 14px; border-radius: 20px; cursor: pointer; font-size: 13px; color: #64748b; }
    main { max-width: 900px; margin: 0 auto; padding: 24px; }
  `]
})
export class App {
  constructor(public auth: AuthService) {}
}

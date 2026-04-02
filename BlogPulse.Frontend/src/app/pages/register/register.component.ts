import { Component, ChangeDetectorRef } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { Router, RouterLink } from '@angular/router';
import { AuthService } from '../../services/auth.service';
@Component({ selector: 'app-register', imports: [FormsModule, RouterLink], template: `
<div class="auth-page"><div class="auth-card">
  <h2>Join BlogPulse</h2><p class="sub">Start sharing your stories</p>
  @if (error) { <div class="error">{{ error }}</div> }
  <form (ngSubmit)="onSubmit()">
    <input type="text" [(ngModel)]="fullName" name="fullName" placeholder="Full Name" required />
    <input type="email" [(ngModel)]="email" name="email" placeholder="Email" required />
    <input type="password" [(ngModel)]="password" name="password" placeholder="Password" required />
    <button type="submit" [disabled]="loading">{{ loading ? 'Creating...' : 'Create Account' }}</button>
  </form>
  <p class="link">Have an account? <a routerLink="/login">Sign In</a></p>
</div></div>`,
styles: [`
  .auth-page { display:flex; justify-content:center; align-items:center; min-height:80vh; }
  .auth-card { background:#fff; padding:40px; border-radius:12px; box-shadow:0 2px 12px rgba(0,0,0,0.06); width:400px; }
  h2 { margin:0 0 4px; } .sub { color:#64748b; margin-bottom:24px; font-size:14px; }
  .error { background:#fef2f2; color:#dc2626; padding:10px; border-radius:8px; margin-bottom:16px; font-size:14px; }
  input { width:100%; padding:12px; border:1px solid #d1d5db; border-radius:8px; font-size:14px; margin-bottom:12px; box-sizing:border-box; }
  input:focus { outline:none; border-color:#7c3aed; }
  button { width:100%; padding:12px; background:#7c3aed; color:#fff; border:none; border-radius:8px; font-size:15px; font-weight:600; cursor:pointer; }
  button:disabled { opacity:0.7; }
  .link { text-align:center; margin-top:16px; font-size:14px; color:#64748b; } .link a { color:#7c3aed; text-decoration:none; }
`] })
export class RegisterComponent {
  fullName=''; email=''; password=''; error=''; loading=false;
  constructor(private auth: AuthService, private router: Router, private cdr: ChangeDetectorRef) {}
  onSubmit() { this.loading=true; this.error=''; this.auth.register(this.fullName, this.email, this.password).subscribe({ next:()=>this.router.navigate(['/']), error:(e)=>{this.error=e.error?.message||'Failed.';this.loading=false;this.cdr.markForCheck();}}); }
}

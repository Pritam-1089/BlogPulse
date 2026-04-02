import { Routes } from '@angular/router';
import { authGuard } from './guards/auth.guard';
export const routes: Routes = [
  { path: '', loadComponent: () => import('./pages/home/home.component').then(m => m.HomeComponent) },
  { path: 'login', loadComponent: () => import('./pages/login/login.component').then(m => m.LoginComponent) },
  { path: 'register', loadComponent: () => import('./pages/register/register.component').then(m => m.RegisterComponent) },
  { path: 'post/:slug', loadComponent: () => import('./pages/post-detail/post-detail.component').then(m => m.PostDetailComponent) },
  { path: 'create', loadComponent: () => import('./pages/create-post/create-post.component').then(m => m.CreatePostComponent), canActivate: [authGuard] },
  { path: 'profile/:id', loadComponent: () => import('./pages/profile/profile.component').then(m => m.ProfileComponent) },
];

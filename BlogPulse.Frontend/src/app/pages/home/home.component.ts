import { Component, OnInit, ChangeDetectorRef } from '@angular/core';
import { RouterLink } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { DatePipe } from '@angular/common';
import { BlogService } from '../../services/blog.service';
import { PostListDto, CategoryDto } from '../../models/api.models';

@Component({
  selector: 'app-home',
  imports: [RouterLink, FormsModule, DatePipe],
  template: `
    <div class="home">
      <div class="hero">
        <h1>Stay curious.</h1>
        <p>Discover stories, thinking, and expertise from writers on any topic.</p>
        <div class="search-bar">
          <input type="text" [(ngModel)]="search" placeholder="Search posts..." (keyup.enter)="loadPosts()" />
        </div>
      </div>
      <div class="categories">
        <button [class.active]="!selectedCategory" (click)="selectedCategory = undefined; loadPosts()">All</button>
        @for (cat of categories; track cat.id) {
          <button [class.active]="selectedCategory === cat.id" (click)="selectedCategory = cat.id; loadPosts()">{{ cat.name }}</button>
        }
      </div>
      <div class="posts-grid">
        @for (post of posts; track post.id) {
          <a [routerLink]="['/post', post.slug]" class="post-card">
            @if (post.coverImage) {
              <img [src]="post.coverImage" class="cover" />
            }
            <div class="card-body">
              <span class="category-tag">{{ post.categoryName }}</span>
              <h3>{{ post.title }}</h3>
              <div class="meta">
                <a [routerLink]="['/profile', post.authorId]" class="author" (click)="$event.stopPropagation()">{{ post.authorName }}</a>
                <span>{{ post.publishedAt | date:'mediumDate' }}</span>
              </div>
              <div class="stats">
                <span>{{ post.likeCount }} likes</span>
                <span>{{ post.tags }}</span>
              </div>
            </div>
          </a>
        } @empty {
          <div class="empty">No posts found.</div>
        }
      </div>
    </div>
  `,
  styles: [`
    .hero { text-align: center; padding: 48px 0 32px; }
    h1 { font-size: 48px; font-weight: 800; color: #1a1a2e; margin-bottom: 8px; }
    .hero p { color: #64748b; font-size: 18px; margin-bottom: 24px; }
    .search-bar input { width: 100%; max-width: 500px; padding: 14px 20px; border: 1px solid #d1d5db; border-radius: 24px; font-size: 15px; }
    .search-bar input:focus { outline: none; border-color: #7c3aed; box-shadow: 0 0 0 3px rgba(124,58,237,0.1); }
    .categories { display: flex; gap: 8px; flex-wrap: wrap; margin-bottom: 32px; justify-content: center; }
    .categories button { padding: 8px 18px; border: 1px solid #e5e7eb; background: #fff; border-radius: 20px; cursor: pointer; font-size: 13px; color: #64748b; }
    .categories button.active { background: #7c3aed; color: #fff; border-color: #7c3aed; }
    .posts-grid { display: grid; grid-template-columns: repeat(auto-fill, minmax(280px, 1fr)); gap: 24px; }
    .post-card { background: #fff; border-radius: 12px; overflow: hidden; text-decoration: none; color: inherit; box-shadow: 0 1px 4px rgba(0,0,0,0.06); transition: transform 0.2s, box-shadow 0.2s; }
    .post-card:hover { transform: translateY(-2px); box-shadow: 0 8px 24px rgba(0,0,0,0.1); }
    .cover { width: 100%; height: 180px; object-fit: cover; }
    .card-body { padding: 20px; }
    .category-tag { display: inline-block; background: #f3f0ff; color: #7c3aed; padding: 4px 10px; border-radius: 12px; font-size: 12px; font-weight: 600; margin-bottom: 8px; }
    h3 { font-size: 18px; margin-bottom: 12px; line-height: 1.3; }
    .meta { display: flex; justify-content: space-between; font-size: 13px; color: #64748b; margin-bottom: 8px; }
    .author { color: #7c3aed; text-decoration: none; font-weight: 600; }
    .stats { display: flex; gap: 12px; font-size: 12px; color: #94a3b8; }
    .empty { text-align: center; padding: 48px; color: #94a3b8; grid-column: 1/-1; }
  `]
})
export class HomeComponent implements OnInit {
  posts: PostListDto[] = [];
  categories: CategoryDto[] = [];
  search = '';
  selectedCategory?: number;

  constructor(private blog: BlogService, private cdr: ChangeDetectorRef) {}

  ngOnInit() {
    this.blog.getCategories().subscribe(c => { this.categories = c; this.cdr.markForCheck(); });
    this.loadPosts();
  }

  loadPosts() {
    this.blog.getPosts(1, 20, this.selectedCategory, this.search || undefined).subscribe(r => {
      this.posts = r.items;
      this.cdr.markForCheck();
    });
  }
}

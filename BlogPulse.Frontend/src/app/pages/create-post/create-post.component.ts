import { Component, OnInit, ChangeDetectorRef } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { Router } from '@angular/router';
import { BlogService } from '../../services/blog.service';
import { CategoryDto } from '../../models/api.models';

@Component({
  selector: 'app-create-post',
  imports: [FormsModule],
  template: `
    <div class="create-page">
      <h2>Write a Post</h2>
      <form (ngSubmit)="onSubmit()">
        <input type="text" [(ngModel)]="title" name="title" placeholder="Post title..." class="title-input" required />
        <input type="text" [(ngModel)]="coverImage" name="cover" placeholder="Cover image URL (optional)" />
        <select [(ngModel)]="categoryId" name="category" required>
          <option [ngValue]="0" disabled>Select category</option>
          @for (cat of categories; track cat.id) {
            <option [ngValue]="cat.id">{{ cat.name }}</option>
          }
        </select>
        <textarea [(ngModel)]="content" name="content" placeholder="Write your story... (supports **bold** and ## headings)" rows="15" required></textarea>
        <input type="text" [(ngModel)]="tags" name="tags" placeholder="Tags (comma-separated: dotnet, angular, csharp)" />
        <div class="actions">
          <button type="submit" class="btn-publish" [disabled]="loading">{{ loading ? 'Publishing...' : 'Publish' }}</button>
          <button type="button" class="btn-draft" (click)="saveDraft()">Save Draft</button>
        </div>
      </form>
    </div>
  `,
  styles: [`
    .create-page { background: #fff; padding: 40px; border-radius: 12px; box-shadow: 0 1px 4px rgba(0,0,0,0.06); }
    h2 { margin-bottom: 24px; }
    .title-input { font-size: 28px; font-weight: 700; border: none; border-bottom: 2px solid #e5e7eb; padding: 12px 0; margin-bottom: 16px; }
    .title-input:focus { outline: none; border-color: #7c3aed; }
    input, select, textarea { width: 100%; padding: 12px; border: 1px solid #d1d5db; border-radius: 8px; font-size: 14px; margin-bottom: 16px; box-sizing: border-box; font-family: inherit; }
    textarea { resize: vertical; line-height: 1.7; }
    input:focus, select:focus, textarea:focus { outline: none; border-color: #7c3aed; }
    .actions { display: flex; gap: 12px; }
    .btn-publish { padding: 12px 32px; background: #7c3aed; color: #fff; border: none; border-radius: 8px; font-size: 15px; font-weight: 600; cursor: pointer; }
    .btn-draft { padding: 12px 32px; background: #f1f5f9; color: #374151; border: none; border-radius: 8px; font-size: 15px; cursor: pointer; }
  `]
})
export class CreatePostComponent implements OnInit {
  title = ''; content = ''; coverImage = ''; tags = ''; categoryId = 0; loading = false;
  categories: CategoryDto[] = [];

  constructor(private blog: BlogService, private router: Router, private cdr: ChangeDetectorRef) {}

  ngOnInit() {
    this.blog.getCategories().subscribe(c => { this.categories = c; this.cdr.markForCheck(); });
  }

  onSubmit() {
    this.loading = true;
    this.blog.createPost({ title: this.title, content: this.content, coverImage: this.coverImage || null, tags: this.tags, categoryId: this.categoryId, isPublished: true }).subscribe(p => {
      this.router.navigate(['/post', p.slug]);
    });
  }

  saveDraft() {
    this.blog.createPost({ title: this.title, content: this.content, coverImage: this.coverImage || null, tags: this.tags, categoryId: this.categoryId, isPublished: false }).subscribe(() => {
      this.router.navigate(['/']);
    });
  }
}

import { Component, OnInit, ChangeDetectorRef } from '@angular/core';
import { ActivatedRoute, RouterLink } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { DatePipe } from '@angular/common';
import { BlogService } from '../../services/blog.service';
import { AuthService } from '../../services/auth.service';
import { PostDto, CommentDto } from '../../models/api.models';

@Component({
  selector: 'app-post-detail',
  imports: [RouterLink, FormsModule, DatePipe],
  template: `
    @if (post) {
      <article class="post">
        @if (post.coverImage) { <img [src]="post.coverImage" class="cover" /> }
        <span class="category-tag">{{ post.categoryName }}</span>
        <h1>{{ post.title }}</h1>
        <div class="meta">
          <a [routerLink]="['/profile', post.authorId]" class="author">{{ post.authorName }}</a>
          <span>{{ post.publishedAt | date:'mediumDate' }}</span>
          <span>{{ post.viewCount }} views</span>
        </div>
        <div class="content" [innerHTML]="formatContent(post.content)"></div>
        <div class="tags">
          @for (tag of post.tags.split(','); track tag) {
            <span class="tag">#{{ tag.trim() }}</span>
          }
        </div>
        <div class="actions">
          <button class="like-btn" [class.liked]="post.isLiked" (click)="toggleLike()">
            {{ post.isLiked ? '❤️' : '🤍' }} {{ post.likeCount }}
          </button>
          <span class="comment-count">💬 {{ post.commentCount }} comments</span>
        </div>
        <div class="comments-section">
          <h3>Comments</h3>
          @if (auth.isLoggedIn()) {
            <div class="comment-form">
              <input [(ngModel)]="newComment" placeholder="Write a comment..." (keyup.enter)="addComment()" />
              <button (click)="addComment()" [disabled]="!newComment.trim()">Post</button>
            </div>
          }
          @for (c of comments; track c.id) {
            <div class="comment">
              <div class="comment-header">
                <strong>{{ c.userName }}</strong>
                <span>{{ c.createdAt | date:'short' }}</span>
              </div>
              <p>{{ c.content }}</p>
            </div>
          } @empty {
            <p class="no-comments">No comments yet. Be the first!</p>
          }
        </div>
      </article>
    } @else { <p>Loading...</p> }
  `,
  styles: [`
    .post { background: #fff; padding: 40px; border-radius: 12px; box-shadow: 0 1px 4px rgba(0,0,0,0.06); }
    .cover { width: 100%; max-height: 400px; object-fit: cover; border-radius: 8px; margin-bottom: 24px; }
    .category-tag { display: inline-block; background: #f3f0ff; color: #7c3aed; padding: 4px 12px; border-radius: 12px; font-size: 13px; font-weight: 600; margin-bottom: 12px; }
    h1 { font-size: 36px; font-weight: 800; line-height: 1.2; margin-bottom: 16px; }
    .meta { display: flex; gap: 16px; color: #64748b; font-size: 14px; margin-bottom: 32px; }
    .author { color: #7c3aed; text-decoration: none; font-weight: 600; }
    .content { font-size: 17px; line-height: 1.8; color: #374151; margin-bottom: 24px; }
    .content h2 { font-size: 24px; margin: 24px 0 12px; }
    .content strong { font-weight: 700; }
    .tags { display: flex; gap: 8px; flex-wrap: wrap; margin-bottom: 24px; }
    .tag { background: #f1f5f9; color: #64748b; padding: 4px 12px; border-radius: 12px; font-size: 13px; }
    .actions { display: flex; gap: 24px; align-items: center; padding: 16px 0; border-top: 1px solid #e5e7eb; border-bottom: 1px solid #e5e7eb; margin-bottom: 32px; }
    .like-btn { background: none; border: 1px solid #e5e7eb; padding: 8px 16px; border-radius: 20px; cursor: pointer; font-size: 14px; }
    .like-btn.liked { border-color: #ef4444; background: #fef2f2; }
    .comment-count { font-size: 14px; color: #64748b; }
    h3 { margin-bottom: 16px; }
    .comment-form { display: flex; gap: 10px; margin-bottom: 24px; }
    .comment-form input { flex: 1; padding: 10px 16px; border: 1px solid #d1d5db; border-radius: 8px; font-size: 14px; }
    .comment-form button { padding: 10px 20px; background: #7c3aed; color: #fff; border: none; border-radius: 8px; font-weight: 600; cursor: pointer; }
    .comment { padding: 12px 0; border-bottom: 1px solid #f1f5f9; }
    .comment-header { display: flex; justify-content: space-between; margin-bottom: 4px; font-size: 13px; color: #64748b; }
    .comment p { font-size: 14px; }
    .no-comments { color: #94a3b8; font-size: 14px; }
  `]
})
export class PostDetailComponent implements OnInit {
  post?: PostDto;
  comments: CommentDto[] = [];
  newComment = '';

  constructor(private route: ActivatedRoute, public auth: AuthService, private blog: BlogService, private cdr: ChangeDetectorRef) {}

  ngOnInit() {
    const slug = this.route.snapshot.params['slug'];
    this.blog.getPost(slug).subscribe(p => {
      this.post = p;
      this.cdr.markForCheck();
    });
  }

  formatContent(content: string): string {
    return content.replace(/\n## /g, '<h2>').replace(/\n/g, '<br>').replace(/\*\*(.*?)\*\*/g, '<strong>$1</strong>');
  }

  toggleLike() {
    if (!this.post || !this.auth.isLoggedIn()) return;
    this.blog.toggleLike(this.post.id).subscribe(r => {
      this.post!.isLiked = r.liked;
      this.post!.likeCount += r.liked ? 1 : -1;
      this.cdr.markForCheck();
    });
  }

  addComment() {
    if (!this.newComment.trim() || !this.post) return;
    this.blog.addComment(this.post.id, this.newComment).subscribe(c => {
      this.comments = [c, ...this.comments];
      this.post!.commentCount++;
      this.newComment = '';
      this.cdr.markForCheck();
    });
  }
}

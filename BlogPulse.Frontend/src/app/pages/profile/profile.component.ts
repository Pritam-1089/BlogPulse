import { Component, OnInit, ChangeDetectorRef } from '@angular/core';
import { ActivatedRoute, RouterLink } from '@angular/router';
import { DatePipe } from '@angular/common';
import { BlogService } from '../../services/blog.service';
import { AuthService } from '../../services/auth.service';
import { UserProfileDto } from '../../models/api.models';

@Component({
  selector: 'app-profile',
  imports: [RouterLink, DatePipe],
  template: `
    @if (profile) {
      <div class="profile-page">
        <div class="profile-header">
          <div class="avatar">{{ profile.fullName.charAt(0) }}</div>
          <div class="info">
            <h2>{{ profile.fullName }}</h2>
            @if (profile.bio) { <p class="bio">{{ profile.bio }}</p> }
            <div class="stats">
              <span><strong>{{ profile.postCount }}</strong> posts</span>
              <span><strong>{{ profile.followerCount }}</strong> followers</span>
              <span><strong>{{ profile.followingCount }}</strong> following</span>
            </div>
          </div>
          @if (auth.isLoggedIn() && auth.user()?.userId !== profile.id) {
            <button class="follow-btn" [class.following]="profile.isFollowing" (click)="toggleFollow()">
              {{ profile.isFollowing ? 'Following' : 'Follow' }}
            </button>
          }
        </div>
        <h3>Posts by {{ profile.fullName }}</h3>
        <div class="posts-list">
          @for (post of profile.posts; track post.id) {
            <a [routerLink]="['/post', post.slug]" class="post-item">
              @if (post.coverImage) { <img [src]="post.coverImage" class="thumb" /> }
              <div class="post-info">
                <span class="cat">{{ post.categoryName }}</span>
                <h4>{{ post.title }}</h4>
                <div class="post-meta">
                  <span>{{ post.publishedAt | date:'mediumDate' }}</span>
                  <span>{{ post.likeCount }} likes</span>
                </div>
              </div>
            </a>
          } @empty {
            <p class="empty">No published posts yet.</p>
          }
        </div>
      </div>
    } @else { <p>Loading...</p> }
  `,
  styles: [`
    .profile-header { display: flex; align-items: center; gap: 24px; background: #fff; padding: 32px; border-radius: 12px; box-shadow: 0 1px 4px rgba(0,0,0,0.06); margin-bottom: 32px; }
    .avatar { width: 80px; height: 80px; border-radius: 50%; background: #7c3aed; color: #fff; display: flex; align-items: center; justify-content: center; font-size: 32px; font-weight: 700; flex-shrink: 0; }
    .info { flex: 1; }
    h2 { margin: 0 0 4px; }
    .bio { color: #64748b; font-size: 14px; margin-bottom: 12px; }
    .stats { display: flex; gap: 24px; font-size: 14px; color: #64748b; }
    .stats strong { color: #1e293b; }
    .follow-btn { padding: 10px 28px; border-radius: 20px; font-weight: 600; cursor: pointer; font-size: 14px; background: #7c3aed; color: #fff; border: none; }
    .follow-btn.following { background: #fff; color: #7c3aed; border: 2px solid #7c3aed; }
    h3 { margin-bottom: 16px; font-size: 18px; }
    .posts-list { display: flex; flex-direction: column; gap: 16px; }
    .post-item { display: flex; gap: 16px; background: #fff; padding: 16px; border-radius: 10px; text-decoration: none; color: inherit; box-shadow: 0 1px 3px rgba(0,0,0,0.04); transition: box-shadow 0.2s; }
    .post-item:hover { box-shadow: 0 4px 12px rgba(0,0,0,0.08); }
    .thumb { width: 120px; height: 80px; object-fit: cover; border-radius: 8px; }
    .post-info { flex: 1; }
    .cat { color: #7c3aed; font-size: 12px; font-weight: 600; }
    h4 { margin: 4px 0 8px; font-size: 16px; }
    .post-meta { font-size: 13px; color: #94a3b8; display: flex; gap: 16px; }
    .empty { color: #94a3b8; text-align: center; padding: 32px; }
  `]
})
export class ProfileComponent implements OnInit {
  profile?: UserProfileDto;

  constructor(private route: ActivatedRoute, public auth: AuthService, private blog: BlogService, private cdr: ChangeDetectorRef) {}

  ngOnInit() {
    const id = +this.route.snapshot.params['id'];
    this.blog.getProfile(id).subscribe(p => { this.profile = p; this.cdr.markForCheck(); });
  }

  toggleFollow() {
    if (!this.profile) return;
    this.blog.toggleFollow(this.profile.id).subscribe(r => {
      this.profile!.isFollowing = r.following;
      this.profile!.followerCount += r.following ? 1 : -1;
      this.cdr.markForCheck();
    });
  }
}

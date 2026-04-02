import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { PostDto, PostListDto, CommentDto, CategoryDto, UserProfileDto, PaginatedResult } from '../models/api.models';

@Injectable({ providedIn: 'root' })
export class BlogService {
  private api = 'https://localhost:7003/api';
  constructor(private http: HttpClient) {}

  getPosts(page = 1, size = 10, categoryId?: number, search?: string, tag?: string): Observable<PaginatedResult<PostListDto>> {
    let url = this.api + '/posts?page=' + page + '&size=' + size;
    if (categoryId) url += '&categoryId=' + categoryId;
    if (search) url += '&search=' + search;
    if (tag) url += '&tag=' + tag;
    return this.http.get<PaginatedResult<PostListDto>>(url);
  }
  getPost(slug: string): Observable<PostDto> { return this.http.get<PostDto>(this.api + '/posts/' + slug); }
  createPost(data: any): Observable<PostDto> { return this.http.post<PostDto>(this.api + '/posts', data); }
  updatePost(id: number, data: any): Observable<PostDto> { return this.http.put<PostDto>(this.api + '/posts/' + id, data); }
  deletePost(id: number): Observable<void> { return this.http.delete<void>(this.api + '/posts/' + id); }
  addComment(postId: number, content: string): Observable<CommentDto> { return this.http.post<CommentDto>(this.api + '/posts/' + postId + '/comments', { content }); }
  toggleLike(postId: number): Observable<{liked: boolean}> { return this.http.post<{liked: boolean}>(this.api + '/posts/' + postId + '/like', {}); }
  getFeed(page = 1): Observable<PostListDto[]> { return this.http.get<PostListDto[]>(this.api + '/posts/feed?page=' + page); }
  getCategories(): Observable<CategoryDto[]> { return this.http.get<CategoryDto[]>(this.api + '/categories'); }
  getProfile(id: number): Observable<UserProfileDto> { return this.http.get<UserProfileDto>(this.api + '/profile/' + id); }
  toggleFollow(id: number): Observable<{following: boolean}> { return this.http.post<{following: boolean}>(this.api + '/profile/' + id + '/follow', {}); }
}

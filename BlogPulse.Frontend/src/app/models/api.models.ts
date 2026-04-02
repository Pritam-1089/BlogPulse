export interface AuthResponse { token: string; userId: number; fullName: string; email: string; role: string; }
export interface PostDto { id: number; title: string; slug: string; content: string; coverImage?: string; tags: string; isPublished: boolean; authorName: string; authorId: number; categoryName: string; categoryId: number; viewCount: number; likeCount: number; commentCount: number; isLiked: boolean; createdAt: string; publishedAt?: string; }
export interface PostListDto { id: number; title: string; slug: string; coverImage?: string; tags: string; authorName: string; authorId: number; categoryName: string; likeCount: number; commentCount: number; publishedAt?: string; }
export interface CommentDto { id: number; content: string; userName: string; userId: number; createdAt: string; }
export interface CategoryDto { id: number; name: string; slug: string; }
export interface UserProfileDto { id: number; fullName: string; email: string; bio?: string; postCount: number; followerCount: number; followingCount: number; isFollowing: boolean; posts: PostListDto[]; }
export interface PaginatedResult<T> { items: T[]; total: number; page: number; pageSize: number; }

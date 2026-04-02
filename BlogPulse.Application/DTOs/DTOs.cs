namespace BlogPulse.Application.DTOs;

public record RegisterDto(string FullName, string Email, string Password);
public record LoginDto(string Email, string Password);
public record AuthResponseDto(string Token, int UserId, string FullName, string Email, string Role);

public record CreatePostDto(string Title, string Content, string? CoverImage, string Tags, int CategoryId, bool IsPublished);
public record UpdatePostDto(string? Title, string? Content, string? CoverImage, string? Tags, int? CategoryId, bool? IsPublished);
public record PostDto(int Id, string Title, string Slug, string Content, string? CoverImage, string Tags, bool IsPublished, string AuthorName, int AuthorId, string CategoryName, int CategoryId, int ViewCount, int LikeCount, int CommentCount, bool IsLiked, DateTime CreatedAt, DateTime? PublishedAt);
public record PostListDto(int Id, string Title, string Slug, string? CoverImage, string Tags, string AuthorName, int AuthorId, string CategoryName, int LikeCount, int CommentCount, DateTime? PublishedAt);

public record CommentDto(int Id, string Content, string UserName, int UserId, DateTime CreatedAt);
public record CreateCommentDto(string Content);

public record CategoryDto(int Id, string Name, string Slug);
public record CreateCategoryDto(string Name);

public record UserProfileDto(int Id, string FullName, string Email, string? Bio, int PostCount, int FollowerCount, int FollowingCount, bool IsFollowing, List<PostListDto> Posts);
public record PaginatedResult<T>(IEnumerable<T> Items, int Total, int Page, int PageSize);

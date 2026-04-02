namespace BlogPulse.Application.Interfaces;
using BlogPulse.Application.DTOs;

public interface IAuthService
{
    Task<AuthResponseDto> RegisterAsync(RegisterDto dto);
    Task<AuthResponseDto> LoginAsync(LoginDto dto);
}

public interface IBlogService
{
    Task<PostDto> CreatePostAsync(CreatePostDto dto, int userId);
    Task<PostDto> UpdatePostAsync(int postId, UpdatePostDto dto, int userId);
    Task DeletePostAsync(int postId, int userId);
    Task<PostDto> GetPostBySlugAsync(string slug, int? userId);
    Task<PaginatedResult<PostListDto>> GetPostsAsync(int page, int size, int? categoryId, string? search, string? tag);
    Task<IEnumerable<PostListDto>> GetFeedAsync(int userId, int page, int size);
    Task<CommentDto> AddCommentAsync(int postId, CreateCommentDto dto, int userId);
    Task DeleteCommentAsync(int commentId, int userId);
    Task<bool> ToggleLikeAsync(int postId, int userId);
    Task<IEnumerable<CategoryDto>> GetCategoriesAsync();
    Task<CategoryDto> CreateCategoryAsync(CreateCategoryDto dto);
    Task<UserProfileDto> GetProfileAsync(int profileUserId, int? currentUserId);
    Task<bool> ToggleFollowAsync(int targetUserId, int currentUserId);
}

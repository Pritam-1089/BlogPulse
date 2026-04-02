namespace BlogPulse.Application.Services;
using System.Text.RegularExpressions;
using BlogPulse.Application.DTOs;
using BlogPulse.Application.Interfaces;
using BlogPulse.Core.Entities;
using BlogPulse.Core.Interfaces;

public class BlogService : IBlogService
{
    private readonly IPostRepository _postRepo;
    private readonly IUserRepository _userRepo;
    private readonly ICategoryRepository _catRepo;
    private readonly ICommentRepository _commentRepo;
    private readonly ILikeRepository _likeRepo;
    private readonly IFollowRepository _followRepo;

    public BlogService(IPostRepository postRepo, IUserRepository userRepo, ICategoryRepository catRepo, ICommentRepository commentRepo, ILikeRepository likeRepo, IFollowRepository followRepo)
    { _postRepo = postRepo; _userRepo = userRepo; _catRepo = catRepo; _commentRepo = commentRepo; _likeRepo = likeRepo; _followRepo = followRepo; }

    public async Task<PostDto> CreatePostAsync(CreatePostDto dto, int userId)
    {
        var slug = GenerateSlug(dto.Title);
        var post = new Post { Title = dto.Title, Slug = slug, Content = dto.Content, CoverImage = dto.CoverImage, Tags = dto.Tags, CategoryId = dto.CategoryId, AuthorId = userId, IsPublished = dto.IsPublished, PublishedAt = dto.IsPublished ? DateTime.UtcNow : null };
        await _postRepo.AddAsync(post);
        var full = await _postRepo.GetByIdAsync(post.Id);
        return MapPost(full!, false);
    }

    public async Task<PostDto> UpdatePostAsync(int postId, UpdatePostDto dto, int userId)
    {
        var post = await _postRepo.GetByIdAsync(postId) ?? throw new KeyNotFoundException("Post not found.");
        if (post.AuthorId != userId) throw new UnauthorizedAccessException("Not your post.");
        if (dto.Title != null) { post.Title = dto.Title; post.Slug = GenerateSlug(dto.Title); }
        if (dto.Content != null) post.Content = dto.Content;
        if (dto.CoverImage != null) post.CoverImage = dto.CoverImage;
        if (dto.Tags != null) post.Tags = dto.Tags;
        if (dto.CategoryId.HasValue) post.CategoryId = dto.CategoryId.Value;
        if (dto.IsPublished.HasValue) { post.IsPublished = dto.IsPublished.Value; if (dto.IsPublished.Value && !post.PublishedAt.HasValue) post.PublishedAt = DateTime.UtcNow; }
        await _postRepo.UpdateAsync(post);
        return MapPost(post, false);
    }

    public async Task DeletePostAsync(int postId, int userId)
    {
        var post = await _postRepo.GetByIdAsync(postId) ?? throw new KeyNotFoundException("Post not found.");
        if (post.AuthorId != userId) throw new UnauthorizedAccessException("Not your post.");
        await _postRepo.DeleteAsync(post);
    }

    public async Task<PostDto> GetPostBySlugAsync(string slug, int? userId)
    {
        var post = await _postRepo.GetBySlugAsync(slug) ?? throw new KeyNotFoundException("Post not found.");
        post.ViewCount++;
        await _postRepo.UpdateAsync(post);
        var isLiked = userId.HasValue && post.Likes.Any(l => l.UserId == userId);
        return MapPost(post, isLiked);
    }

    public async Task<PaginatedResult<PostListDto>> GetPostsAsync(int page, int size, int? categoryId, string? search, string? tag)
    {
        var (posts, total) = await _postRepo.GetAllAsync(page, size, categoryId, search, tag);
        return new PaginatedResult<PostListDto>(posts.Select(p => MapPostList(p)), total, page, size);
    }

    public async Task<IEnumerable<PostListDto>> GetFeedAsync(int userId, int page, int size)
    {
        var posts = await _postRepo.GetFeedAsync(userId, page, size);
        return posts.Select(p => MapPostList(p));
    }

    public async Task<CommentDto> AddCommentAsync(int postId, CreateCommentDto dto, int userId)
    {
        var user = await _userRepo.GetByIdAsync(userId);
        var comment = new Comment { Content = dto.Content, PostId = postId, UserId = userId };
        await _commentRepo.AddAsync(comment);
        return new CommentDto(comment.Id, comment.Content, user!.FullName, userId, comment.CreatedAt);
    }

    public async Task DeleteCommentAsync(int commentId, int userId)
    {
        var comment = await _commentRepo.GetByIdAsync(commentId) ?? throw new KeyNotFoundException("Comment not found.");
        if (comment.UserId != userId) throw new UnauthorizedAccessException("Not your comment.");
        await _commentRepo.DeleteAsync(comment);
    }

    public async Task<bool> ToggleLikeAsync(int postId, int userId)
    {
        var existing = await _likeRepo.GetAsync(postId, userId);
        if (existing != null) { await _likeRepo.DeleteAsync(existing); return false; }
        await _likeRepo.AddAsync(new PostLike { PostId = postId, UserId = userId });
        return true;
    }

    public async Task<IEnumerable<CategoryDto>> GetCategoriesAsync() =>
        (await _catRepo.GetAllAsync()).Select(c => new CategoryDto(c.Id, c.Name, c.Slug));

    public async Task<CategoryDto> CreateCategoryAsync(CreateCategoryDto dto)
    {
        var cat = new Category { Name = dto.Name, Slug = GenerateSlug(dto.Name) };
        await _catRepo.AddAsync(cat);
        return new CategoryDto(cat.Id, cat.Name, cat.Slug);
    }

    public async Task<UserProfileDto> GetProfileAsync(int profileUserId, int? currentUserId)
    {
        var user = await _userRepo.GetProfileAsync(profileUserId) ?? throw new KeyNotFoundException("User not found.");
        var followers = await _followRepo.GetFollowerCountAsync(profileUserId);
        var following = await _followRepo.GetFollowingCountAsync(profileUserId);
        var isFollowing = currentUserId.HasValue && await _followRepo.GetAsync(currentUserId.Value, profileUserId) != null;
        var posts = user.Posts.Select(p => MapPostList(p)).ToList();
        return new UserProfileDto(user.Id, user.FullName, user.Email, user.Bio, posts.Count, followers, following, isFollowing, posts);
    }

    public async Task<bool> ToggleFollowAsync(int targetUserId, int currentUserId)
    {
        if (targetUserId == currentUserId) throw new InvalidOperationException("Cannot follow yourself.");
        var existing = await _followRepo.GetAsync(currentUserId, targetUserId);
        if (existing != null) { await _followRepo.DeleteAsync(existing); return false; }
        await _followRepo.AddAsync(new UserFollow { FollowerId = currentUserId, FollowingId = targetUserId });
        return true;
    }

    private static PostDto MapPost(Post p, bool isLiked) => new(p.Id, p.Title, p.Slug, p.Content, p.CoverImage, p.Tags, p.IsPublished, p.Author.FullName, p.AuthorId, p.Category.Name, p.CategoryId, p.ViewCount, p.Likes.Count, p.Comments.Count, isLiked, p.CreatedAt, p.PublishedAt);
    private static PostListDto MapPostList(Post p) => new(p.Id, p.Title, p.Slug, p.CoverImage, p.Tags, p.Author?.FullName ?? "", p.AuthorId, p.Category?.Name ?? "", p.Likes.Count, 0, p.PublishedAt);
    private static string GenerateSlug(string title) => Regex.Replace(title.ToLower().Trim(), @"[^a-z0-9]+", "-").Trim('-') + "-" + Guid.NewGuid().ToString()[..6];
}

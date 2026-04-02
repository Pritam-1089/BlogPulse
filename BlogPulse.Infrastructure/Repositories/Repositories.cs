namespace BlogPulse.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using BlogPulse.Core.Entities;
using BlogPulse.Core.Interfaces;
using BlogPulse.Infrastructure.Data;

public class UserRepository : IUserRepository
{
    private readonly AppDbContext _db;
    public UserRepository(AppDbContext db) => _db = db;
    public async Task<User?> GetByIdAsync(int id) => await _db.Users.FindAsync(id);
    public async Task<User?> GetByEmailAsync(string email) => await _db.Users.FirstOrDefaultAsync(u => u.Email == email);
    public async Task<User> AddAsync(User user) { _db.Users.Add(user); await _db.SaveChangesAsync(); return user; }
    public async Task<User?> GetProfileAsync(int id) => await _db.Users.Include(u => u.Posts.Where(p => p.IsPublished)).ThenInclude(p => p.Category).FirstOrDefaultAsync(u => u.Id == id);
    public async Task<IEnumerable<User>> SearchAsync(string query) => await _db.Users.Where(u => u.FullName.Contains(query)).Take(20).ToListAsync();
}

public class PostRepository : IPostRepository
{
    private readonly AppDbContext _db;
    public PostRepository(AppDbContext db) => _db = db;
    public async Task<Post> AddAsync(Post post) { _db.Posts.Add(post); await _db.SaveChangesAsync(); return post; }
    public async Task UpdateAsync(Post post) { _db.Posts.Update(post); await _db.SaveChangesAsync(); }
    public async Task DeleteAsync(Post post) { _db.Posts.Remove(post); await _db.SaveChangesAsync(); }
    public async Task<Post?> GetByIdAsync(int id) => await _db.Posts.Include(p => p.Author).Include(p => p.Category).Include(p => p.Comments).ThenInclude(c => c.User).Include(p => p.Likes).FirstOrDefaultAsync(p => p.Id == id);
    public async Task<Post?> GetBySlugAsync(string slug) => await _db.Posts.Include(p => p.Author).Include(p => p.Category).Include(p => p.Comments).ThenInclude(c => c.User).Include(p => p.Likes).FirstOrDefaultAsync(p => p.Slug == slug);
    public async Task<(IEnumerable<Post> Posts, int Total)> GetAllAsync(int page, int size, int? categoryId = null, string? search = null, string? tag = null)
    {
        var q = _db.Posts.Where(p => p.IsPublished).AsQueryable();
        if (categoryId.HasValue) q = q.Where(p => p.CategoryId == categoryId);
        if (!string.IsNullOrEmpty(search)) q = q.Where(p => p.Title.Contains(search) || p.Content.Contains(search));
        if (!string.IsNullOrEmpty(tag)) q = q.Where(p => p.Tags.Contains(tag));
        var total = await q.CountAsync();
        var posts = await q.Include(p => p.Author).Include(p => p.Category).Include(p => p.Likes).OrderByDescending(p => p.PublishedAt).Skip((page - 1) * size).Take(size).ToListAsync();
        return (posts, total);
    }
    public async Task<IEnumerable<Post>> GetByAuthorAsync(int authorId) => await _db.Posts.Where(p => p.AuthorId == authorId && p.IsPublished).Include(p => p.Category).Include(p => p.Likes).OrderByDescending(p => p.PublishedAt).ToListAsync();
    public async Task<IEnumerable<Post>> GetFeedAsync(int userId, int page, int size)
    {
        var followingIds = await _db.UserFollows.Where(f => f.FollowerId == userId).Select(f => f.FollowingId).ToListAsync();
        return await _db.Posts.Where(p => p.IsPublished && followingIds.Contains(p.AuthorId)).Include(p => p.Author).Include(p => p.Category).Include(p => p.Likes).OrderByDescending(p => p.PublishedAt).Skip((page - 1) * size).Take(size).ToListAsync();
    }
}

public class CategoryRepository : ICategoryRepository
{
    private readonly AppDbContext _db;
    public CategoryRepository(AppDbContext db) => _db = db;
    public async Task<IEnumerable<Category>> GetAllAsync() => await _db.Categories.ToListAsync();
    public async Task<Category> AddAsync(Category c) { _db.Categories.Add(c); await _db.SaveChangesAsync(); return c; }
}

public class CommentRepository : ICommentRepository
{
    private readonly AppDbContext _db;
    public CommentRepository(AppDbContext db) => _db = db;
    public async Task<Comment> AddAsync(Comment c) { _db.Comments.Add(c); await _db.SaveChangesAsync(); return c; }
    public async Task DeleteAsync(Comment c) { _db.Comments.Remove(c); await _db.SaveChangesAsync(); }
    public async Task<Comment?> GetByIdAsync(int id) => await _db.Comments.FindAsync(id);
    public async Task<IEnumerable<Comment>> GetByPostIdAsync(int postId) => await _db.Comments.Where(c => c.PostId == postId).Include(c => c.User).OrderByDescending(c => c.CreatedAt).ToListAsync();
}

public class LikeRepository : ILikeRepository
{
    private readonly AppDbContext _db;
    public LikeRepository(AppDbContext db) => _db = db;
    public async Task<PostLike?> GetAsync(int postId, int userId) => await _db.PostLikes.FirstOrDefaultAsync(l => l.PostId == postId && l.UserId == userId);
    public async Task<PostLike> AddAsync(PostLike l) { _db.PostLikes.Add(l); await _db.SaveChangesAsync(); return l; }
    public async Task DeleteAsync(PostLike l) { _db.PostLikes.Remove(l); await _db.SaveChangesAsync(); }
    public async Task<int> GetCountAsync(int postId) => await _db.PostLikes.CountAsync(l => l.PostId == postId);
}

public class FollowRepository : IFollowRepository
{
    private readonly AppDbContext _db;
    public FollowRepository(AppDbContext db) => _db = db;
    public async Task<UserFollow?> GetAsync(int followerId, int followingId) => await _db.UserFollows.FirstOrDefaultAsync(f => f.FollowerId == followerId && f.FollowingId == followingId);
    public async Task<UserFollow> AddAsync(UserFollow f) { _db.UserFollows.Add(f); await _db.SaveChangesAsync(); return f; }
    public async Task DeleteAsync(UserFollow f) { _db.UserFollows.Remove(f); await _db.SaveChangesAsync(); }
    public async Task<int> GetFollowerCountAsync(int userId) => await _db.UserFollows.CountAsync(f => f.FollowingId == userId);
    public async Task<int> GetFollowingCountAsync(int userId) => await _db.UserFollows.CountAsync(f => f.FollowerId == userId);
    public async Task<IEnumerable<int>> GetFollowingIdsAsync(int userId) => await _db.UserFollows.Where(f => f.FollowerId == userId).Select(f => f.FollowingId).ToListAsync();
}

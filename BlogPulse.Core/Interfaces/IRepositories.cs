namespace BlogPulse.Core.Interfaces;
using BlogPulse.Core.Entities;

public interface IUserRepository
{
    Task<User?> GetByIdAsync(int id);
    Task<User?> GetByEmailAsync(string email);
    Task<User> AddAsync(User user);
    Task<User?> GetProfileAsync(int id);
    Task<IEnumerable<User>> SearchAsync(string query);
}

public interface IPostRepository
{
    Task<Post> AddAsync(Post post);
    Task UpdateAsync(Post post);
    Task DeleteAsync(Post post);
    Task<Post?> GetByIdAsync(int id);
    Task<Post?> GetBySlugAsync(string slug);
    Task<(IEnumerable<Post> Posts, int Total)> GetAllAsync(int page, int size, int? categoryId = null, string? search = null, string? tag = null);
    Task<IEnumerable<Post>> GetByAuthorAsync(int authorId);
    Task<IEnumerable<Post>> GetFeedAsync(int userId, int page, int size);
}

public interface ICategoryRepository
{
    Task<IEnumerable<Category>> GetAllAsync();
    Task<Category> AddAsync(Category category);
}

public interface ICommentRepository
{
    Task<Comment> AddAsync(Comment comment);
    Task DeleteAsync(Comment comment);
    Task<Comment?> GetByIdAsync(int id);
    Task<IEnumerable<Comment>> GetByPostIdAsync(int postId);
}

public interface ILikeRepository
{
    Task<PostLike?> GetAsync(int postId, int userId);
    Task<PostLike> AddAsync(PostLike like);
    Task DeleteAsync(PostLike like);
    Task<int> GetCountAsync(int postId);
}

public interface IFollowRepository
{
    Task<UserFollow?> GetAsync(int followerId, int followingId);
    Task<UserFollow> AddAsync(UserFollow follow);
    Task DeleteAsync(UserFollow follow);
    Task<int> GetFollowerCountAsync(int userId);
    Task<int> GetFollowingCountAsync(int userId);
    Task<IEnumerable<int>> GetFollowingIdsAsync(int userId);
}

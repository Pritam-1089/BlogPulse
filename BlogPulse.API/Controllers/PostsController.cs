namespace BlogPulse.API.Controllers;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using BlogPulse.Application.DTOs;
using BlogPulse.Application.Interfaces;

[ApiController, Route("api/[controller]")]
public class PostsController : ControllerBase
{
    private readonly IBlogService _blog;
    public PostsController(IBlogService blog) => _blog = blog;
    private int UserId => int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
    private int? OptionalUserId => User.Identity?.IsAuthenticated == true ? UserId : null;

    [HttpGet] public async Task<ActionResult> GetAll([FromQuery] int page = 1, [FromQuery] int size = 10, [FromQuery] int? categoryId = null, [FromQuery] string? search = null, [FromQuery] string? tag = null)
        => Ok(await _blog.GetPostsAsync(page, size, categoryId, search, tag));

    [HttpGet("{slug}")] public async Task<ActionResult<PostDto>> GetBySlug(string slug) => Ok(await _blog.GetPostBySlugAsync(slug, OptionalUserId));

    [HttpPost, Authorize] public async Task<ActionResult<PostDto>> Create(CreatePostDto dto) => Ok(await _blog.CreatePostAsync(dto, UserId));
    [HttpPut("{id}"), Authorize] public async Task<ActionResult<PostDto>> Update(int id, UpdatePostDto dto) => Ok(await _blog.UpdatePostAsync(id, dto, UserId));
    [HttpDelete("{id}"), Authorize] public async Task<IActionResult> Delete(int id) { await _blog.DeletePostAsync(id, UserId); return NoContent(); }

    [HttpPost("{id}/comments"), Authorize] public async Task<ActionResult<CommentDto>> AddComment(int id, CreateCommentDto dto) => Ok(await _blog.AddCommentAsync(id, dto, UserId));
    [HttpDelete("comments/{commentId}"), Authorize] public async Task<IActionResult> DeleteComment(int commentId) { await _blog.DeleteCommentAsync(commentId, UserId); return NoContent(); }

    [HttpPost("{id}/like"), Authorize] public async Task<ActionResult> ToggleLike(int id) => Ok(new { liked = await _blog.ToggleLikeAsync(id, UserId) });

    [HttpGet("feed"), Authorize] public async Task<ActionResult> GetFeed([FromQuery] int page = 1, [FromQuery] int size = 10) => Ok(await _blog.GetFeedAsync(UserId, page, size));
}

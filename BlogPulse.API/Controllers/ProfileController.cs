namespace BlogPulse.API.Controllers;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using BlogPulse.Application.DTOs;
using BlogPulse.Application.Interfaces;

[ApiController, Route("api/[controller]")]
public class ProfileController : ControllerBase
{
    private readonly IBlogService _blog;
    public ProfileController(IBlogService blog) => _blog = blog;
    private int UserId => int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
    private int? OptionalUserId => User.Identity?.IsAuthenticated == true ? UserId : null;

    [HttpGet("{id}")] public async Task<ActionResult<UserProfileDto>> Get(int id) => Ok(await _blog.GetProfileAsync(id, OptionalUserId));
    [HttpPost("{id}/follow"), Authorize] public async Task<ActionResult> ToggleFollow(int id) => Ok(new { following = await _blog.ToggleFollowAsync(id, UserId) });
}

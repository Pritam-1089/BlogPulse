namespace BlogPulse.API.Controllers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using BlogPulse.Application.DTOs;
using BlogPulse.Application.Interfaces;

[ApiController, Route("api/[controller]")]
public class CategoriesController : ControllerBase
{
    private readonly IBlogService _blog;
    public CategoriesController(IBlogService blog) => _blog = blog;

    [HttpGet] public async Task<ActionResult<IEnumerable<CategoryDto>>> GetAll() => Ok(await _blog.GetCategoriesAsync());
    [HttpPost, Authorize(Roles = "Admin")] public async Task<ActionResult<CategoryDto>> Create(CreateCategoryDto dto) => Ok(await _blog.CreateCategoryAsync(dto));
}

using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using BlogPulse.Application.Interfaces;
using BlogPulse.Application.Services;
using BlogPulse.Core.Entities;
using BlogPulse.Core.Interfaces;
using BlogPulse.Infrastructure.Data;
using BlogPulse.Infrastructure.Repositories;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<AppDbContext>(o => o.UseInMemoryDatabase("BlogPulseDb"));
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IPostRepository, PostRepository>();
builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();
builder.Services.AddScoped<ICommentRepository, CommentRepository>();
builder.Services.AddScoped<ILikeRepository, LikeRepository>();
builder.Services.AddScoped<IFollowRepository, FollowRepository>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IBlogService, BlogService>();

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(o =>
    {
        o.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true, ValidateAudience = true, ValidateLifetime = true, ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"], ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]!))
        };
    });

builder.Services.AddCors(o => o.AddPolicy("Allow", p => p.WithOrigins("http://localhost:4200").AllowAnyHeader().AllowAnyMethod().AllowCredentials()));
builder.Services.AddControllers().AddJsonOptions(o => o.JsonSerializerOptions.Converters.Add(new System.Text.Json.Serialization.JsonStringEnumConverter()));
builder.Services.AddProblemDetails();

var app = builder.Build();

app.UseExceptionHandler(err => err.Run(async ctx =>
{
    ctx.Response.ContentType = "application/json";
    var ex = ctx.Features.Get<Microsoft.AspNetCore.Diagnostics.IExceptionHandlerFeature>()?.Error;
    ctx.Response.StatusCode = ex switch { UnauthorizedAccessException => 401, KeyNotFoundException => 404, InvalidOperationException => 400, _ => 500 };
    await ctx.Response.WriteAsJsonAsync(new { message = ex?.Message ?? "Error" });
}));

// Seed
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    if (!db.Users.Any())
    {
        var admin = new User { FullName = "Pritam Chavan", Email = "pritam@blog.com", PasswordHash = BCrypt.Net.BCrypt.HashPassword("123456"), Role = "Admin", Bio = "Full Stack Developer at Metafic. .NET & Angular enthusiast." };
        var mayuri = new User { FullName = "Mayuri Kawar", Email = "mayuri@blog.com", PasswordHash = BCrypt.Net.BCrypt.HashPassword("123456"), Bio = "Backend developer passionate about Clean Architecture." };
        var shivani = new User { FullName = "Shivani Patil", Email = "shivani@blog.com", PasswordHash = BCrypt.Net.BCrypt.HashPassword("123456"), Bio = "Learning .NET and Angular. Tech blogger." };
        db.Users.AddRange(admin, mayuri, shivani);
        db.SaveChanges();

        var cats = new[] {
            new Category { Name = "Technology", Slug = "technology" },
            new Category { Name = "Web Development", Slug = "web-development" },
            new Category { Name = "DevOps", Slug = "devops" },
            new Category { Name = "Career", Slug = "career" }
        };
        db.Categories.AddRange(cats);
        db.SaveChanges();

        // Follows
        db.UserFollows.AddRange(
            new UserFollow { FollowerId = mayuri.Id, FollowingId = admin.Id },
            new UserFollow { FollowerId = shivani.Id, FollowingId = admin.Id },
            new UserFollow { FollowerId = admin.Id, FollowingId = mayuri.Id });
        db.SaveChanges();

        var posts = new[] {
            new Post { Title = "Getting Started with Clean Architecture in .NET", Slug = "clean-architecture-dotnet", Content = "Clean Architecture is a software design philosophy that separates code into layers with clear dependencies. In this post, we explore how to implement it in .NET Core with Entity Framework.\n\n## Why Clean Architecture?\n- Separation of concerns\n- Testability\n- Framework independence\n\n## Layers\n1. **Core** - Entities and interfaces\n2. **Application** - Business logic and DTOs\n3. **Infrastructure** - Data access and external services\n4. **API** - Controllers and middleware\n\nThis approach ensures your business logic is never dependent on frameworks or databases.", CoverImage = "https://images.unsplash.com/photo-1555066931-4365d14bab8c?w=800", Tags = "dotnet,clean-architecture,csharp", AuthorId = admin.Id, CategoryId = cats[1].Id, IsPublished = true, PublishedAt = DateTime.UtcNow.AddDays(-10), ViewCount = 245 },
            new Post { Title = "JWT Authentication in ASP.NET Core - Complete Guide", Slug = "jwt-auth-aspnet-core", Content = "JSON Web Tokens (JWT) provide a secure way to handle authentication in modern web applications. This guide covers everything from setup to advanced scenarios.\n\n## Setup\n1. Install `Microsoft.AspNetCore.Authentication.JwtBearer`\n2. Configure in Program.cs\n3. Create token generation service\n\n## Best Practices\n- Use strong signing keys\n- Set appropriate expiration\n- Implement refresh tokens\n- Store tokens securely on the client", CoverImage = "https://images.unsplash.com/photo-1614064641938-3bbee52942c7?w=800", Tags = "jwt,authentication,security,aspnet", AuthorId = admin.Id, CategoryId = cats[1].Id, IsPublished = true, PublishedAt = DateTime.UtcNow.AddDays(-7), ViewCount = 189 },
            new Post { Title = "Angular Signals - The Future of Change Detection", Slug = "angular-signals-guide", Content = "Angular Signals represent a new reactive primitive that simplifies state management and improves performance by enabling fine-grained reactivity.\n\n## What are Signals?\nSignals are wrapper around values that notify consumers when the value changes.\n\n## Benefits\n- No Zone.js dependency\n- Better performance\n- Simpler mental model\n- Works with computed and effects", CoverImage = "https://images.unsplash.com/photo-1627398242454-45a1465c2479?w=800", Tags = "angular,signals,frontend,typescript", AuthorId = mayuri.Id, CategoryId = cats[0].Id, IsPublished = true, PublishedAt = DateTime.UtcNow.AddDays(-5), ViewCount = 156 },
            new Post { Title = "Docker for .NET Developers - From Zero to Production", Slug = "docker-dotnet-developers", Content = "Containerization has become essential for modern development. Learn how to containerize your .NET applications with Docker.\n\n## Dockerfile for .NET\n```dockerfile\nFROM mcr.microsoft.com/dotnet/aspnet:9.0\nCOPY publish/ App/\nWORKDIR /App\nENTRYPOINT [\"dotnet\", \"MyApp.dll\"]\n```\n\n## Docker Compose\nUse docker-compose for multi-container setups with SQL Server and your API.", CoverImage = "https://images.unsplash.com/photo-1605745341112-85968b19335b?w=800", Tags = "docker,dotnet,devops,containers", AuthorId = admin.Id, CategoryId = cats[2].Id, IsPublished = true, PublishedAt = DateTime.UtcNow.AddDays(-3), ViewCount = 98 },
            new Post { Title = "Entity Framework Core - Performance Tips & Tricks", Slug = "ef-core-performance-tips", Content = "Entity Framework Core is powerful but can be slow if not used correctly. Here are proven tips to improve performance.\n\n## Tips\n1. Use `AsNoTracking()` for read-only queries\n2. Avoid N+1 queries with `Include()`\n3. Use projections with `Select()`\n4. Implement pagination\n5. Use compiled queries for hot paths", CoverImage = "https://images.unsplash.com/photo-1504639725590-34d0984388bd?w=800", Tags = "efcore,dotnet,performance,database", AuthorId = mayuri.Id, CategoryId = cats[1].Id, IsPublished = true, PublishedAt = DateTime.UtcNow.AddDays(-2), ViewCount = 134 },
            new Post { Title = "My Journey from Fresher to Full Stack Developer", Slug = "fresher-to-fullstack", Content = "Three months ago I started learning .NET and Angular. Here is what I learned and the mistakes I made along the way.\n\n## Month 1: C# Basics\n- Variables, OOP, LINQ\n- Console applications\n\n## Month 2: ASP.NET Core\n- Web API, REST, Entity Framework\n- Authentication\n\n## Month 3: Angular\n- Components, Services, Routing\n- HTTP Client, Forms\n\nThe key is consistency and building real projects.", CoverImage = "https://images.unsplash.com/photo-1498050108023-c5249f4df085?w=800", Tags = "career,learning,dotnet,angular", AuthorId = shivani.Id, CategoryId = cats[3].Id, IsPublished = true, PublishedAt = DateTime.UtcNow.AddDays(-1), ViewCount = 312 },
        };
        db.Posts.AddRange(posts);
        db.SaveChanges();

        // Comments and Likes
        db.Comments.AddRange(
            new Comment { PostId = posts[0].Id, UserId = mayuri.Id, Content = "Great explanation of Clean Architecture! Very helpful." },
            new Comment { PostId = posts[0].Id, UserId = shivani.Id, Content = "This helped me understand the layered approach. Thanks!" },
            new Comment { PostId = posts[5].Id, UserId = admin.Id, Content = "Great progress Shivani! Keep going." },
            new Comment { PostId = posts[2].Id, UserId = admin.Id, Content = "Signals are game changer for Angular performance." });
        db.PostLikes.AddRange(
            new PostLike { PostId = posts[0].Id, UserId = mayuri.Id },
            new PostLike { PostId = posts[0].Id, UserId = shivani.Id },
            new PostLike { PostId = posts[1].Id, UserId = mayuri.Id },
            new PostLike { PostId = posts[2].Id, UserId = admin.Id },
            new PostLike { PostId = posts[5].Id, UserId = admin.Id },
            new PostLike { PostId = posts[5].Id, UserId = mayuri.Id });
        db.SaveChanges();
    }
}

app.UseCors("Allow");
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.Run();

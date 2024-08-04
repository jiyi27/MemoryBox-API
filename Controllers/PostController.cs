using System.Security.Claims;
using MemoryBox_API.Models;
using MemoryBox_API.Models.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace MemoryBox_API.Controllers;


[ApiController]
[Route("api/boxes/{boxId:int}/posts")]
public class PostController(DatabaseContext context) : ControllerBase
{
    [Authorize]
    [HttpGet]
    public async Task<IActionResult> GetPosts(int boxId)
    {
        var userId = GetCurrentUserId();
        if (userId == -1)
        {
            return Unauthorized();
        }
        
        var posts = await context.Posts
            .Where(p => p.BoxId == boxId)
            .Select(p => new
            {
                p.PostId,
                p.BoxId,
                p.Title,
                p.Content,
                p.CreatedDate,
                ImageUrls = p.Images.Select(i => i.Url).ToList(),
            })
            .ToListAsync();
        
        return Ok(posts);
    }
    
    [Authorize]
    [HttpPost]
    public async Task<IActionResult> CreatePost(int boxId, Post post)
    {
        var userId = GetCurrentUserId();
        if (userId == -1)
        {
            return Unauthorized();
        }
        
        // Check if the post has content, title or at least one image.
        if (string.IsNullOrWhiteSpace(post.Content) && string.IsNullOrWhiteSpace(post.Title) && post.Images.Count == 0)
        {
            return BadRequest("Post must have content, title or at least one image.");
        }
        
        // Check if the box exists or the current user is the owner of the box.
        var box = await context.Boxes.FindAsync(boxId);
        if (box == null || box.OwnerId != userId)
        {
            return NotFound("Box not found or you are not the owner of the box.");
        }
        
        post.BoxId = boxId;
        context.Posts.Add(post);
        await context.SaveChangesAsync();
        
        // EF Core will update the local entity with the generated PostId after SaveChanges.
        // Load the Images if needed
        await context.Entry(post).Collection(p => p.Images).LoadAsync();
        return Ok(post);
    }
    
    [Authorize]
    [HttpDelete("{postId:int}")]
    public async Task<IActionResult> DeletePost(int boxId, int postId)
    {
        var userId = GetCurrentUserId();
        if (userId == -1)
        {
            return Unauthorized();
        }
        
        var post = await context.Posts.FindAsync(postId);
        if (post == null || post.BoxId != boxId)
        {
            return NotFound("Post not found or not in the box.");
        }
        
        // Check if the current user is the owner of the box.
        var box = await context.Boxes.FindAsync(boxId);
        if (box == null)
        {
            return NotFound("Box not found.");
        }
        
        if (box.OwnerId != userId)
        {
            return Unauthorized("You are not the owner of the box.");
        }
        
        context.Posts.Remove(post);
        await context.SaveChangesAsync();
        return Ok("Post deleted successfully.");
    }
    
    private int GetCurrentUserId()
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (userId == null || !int.TryParse(userId, out var userIdInt))
        {
            return -1;
        }
        return userIdInt;
    }
}
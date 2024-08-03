using System.Security.Claims;
using MemoryBox_API.Models;
using MemoryBox_API.Models.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MemoryBox_API.Models.Dto;
namespace MemoryBox_API.Controllers;

[ApiController]
[Route("api/boxes")]
public class BoxController(DatabaseContext context) : ControllerBase
{
    [Authorize]
    [HttpGet]
    public async Task<IActionResult> GetUserBoxes()
    {
        var userId = GetCurrentUserId();
        if (userId == -1)
        {
            return Unauthorized();
        }
        
        var boxes = await context.Boxes
            .Where(b => b.OwnerId == userId)
            .Select(b => new
            {
                b.BoxId,
                b.BoxName,
                b.IsPrivate,
                b.OwnerId,
                b.CreatedDate,
                Owner = b.Owner !=null ? new
                {
                    b.Owner.FullName,
                    b.Owner.ProfilePictureUrl
                } : null
            })
            .ToListAsync();
        return Ok(boxes);
    }
    
    [Authorize]
    [HttpPost]
    public async Task<IActionResult> CreateBox(Box box)
    {
        var userId = GetCurrentUserId();
        if (userId == -1)
        {
            return Unauthorized();
        }
        
        box.OwnerId = userId;
        context.Boxes.Add(box);
        await context.SaveChangesAsync();
        
        // EF Core will update the local entity with the generated BoxId after SaveChanges.
        // Load the Owner if needed
        await context.Entry(box).Reference(b => b.Owner).LoadAsync();
        
        var createdBox = new
        {
            box.BoxId,
            box.BoxName,
            box.IsPrivate,
            box.CreatedDate,
            Owner = box.Owner != null ? new
            {
                box.Owner.FullName,
            } : null
        };

        return StatusCode(201, createdBox);
    }
    
    [Authorize]
    [HttpPut("{boxId:int}")]
    public async Task<IActionResult> UpdateBox(int boxId, BoxUpdateDto boxDto)
    {
        var userId = GetCurrentUserId();
        if (userId == -1)
        {
            return Unauthorized();
        }

        // Fetch the existing box from the database
        var existingBox = await context.Boxes
            .FirstOrDefaultAsync(b => b.BoxId == boxId && b.OwnerId == userId);

        if (existingBox == null)
        {
            return NotFound("Box not found or you don't have permission to update it.");
        }

        // Update only the allowed properties
        existingBox.BoxName = boxDto.BoxName;
        existingBox.IsPrivate = boxDto.IsPrivate;

        await context.SaveChangesAsync();
        return NoContent();
    }
    
    [Authorize]
    [HttpDelete("{boxId:int}")]
    public async Task<IActionResult> DeleteBox(int boxId)
    {
        var userId = GetCurrentUserId();
        if (userId == -1)
        {
            return Unauthorized();
        }
        
        var box = await context.Boxes.FirstOrDefaultAsync(b => b.BoxId == boxId && b.OwnerId == userId);
        if (box == null)
        {
            return NotFound();
        }
        
        context.Boxes.Remove(box);
        await context.SaveChangesAsync();

        return NoContent();
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


namespace MemoryBox_API.Models.Dto;

public class BoxUpdateDto
{
    public required string BoxName { get; set; }
    public required bool IsPrivate { get; set; }
}
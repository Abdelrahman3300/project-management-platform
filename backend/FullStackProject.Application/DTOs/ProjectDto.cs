
using System.ComponentModel.DataAnnotations;

public class ProjectDto
{
    public Guid Id { get; set; }
    
    [Required(ErrorMessage = "Project name is required.")]
    [StringLength(100, MinimumLength = 2,ErrorMessage = "Project name must be between 2 and 100 characters.")]
    public string Name { get; set; } = string.Empty;
    
    [StringLength(2000,ErrorMessage = "Description cannot exceed 2000 characters.")]
    public string? Description { get; set; }

    public Guid OwnerId { get; set; }

    public string OwnerName { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }
}

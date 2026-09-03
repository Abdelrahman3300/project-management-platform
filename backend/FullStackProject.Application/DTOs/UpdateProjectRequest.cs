using System.ComponentModel.DataAnnotations;

namespace FullStackProject.Application.DTOs;

public class UpdateProjectRequest
{
    [Required]
    [MaxLength(150)]
    public string Name { get; set; } = string.Empty;

    [MaxLength(1000)]
    public string? Description { get; set; }
}

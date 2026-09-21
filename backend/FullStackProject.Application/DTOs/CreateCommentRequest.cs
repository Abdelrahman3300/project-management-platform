using System.ComponentModel.DataAnnotations;

namespace FullStackProject.Application.DTOs;

public class CreateCommentRequest
{
    [Required]
    [MaxLength(2000)]
    public string Content { get; set; } = string.Empty;
}
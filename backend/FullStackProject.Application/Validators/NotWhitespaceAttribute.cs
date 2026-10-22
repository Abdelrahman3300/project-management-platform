using System.ComponentModel.DataAnnotations;

namespace FullStackProject.Application.Validation;

[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
public sealed class NotWhitespaceAttribute : ValidationAttribute
{
    public NotWhitespaceAttribute()
    {
        ErrorMessage = "The field cannot be empty or contain only whitespace.";
    }

    public override bool IsValid(object? value)
    {
        return value is string text && !string.IsNullOrWhiteSpace(text);
    }
}
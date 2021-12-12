using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

public class User
{
    [Key]
    [Required]
    public int Id { get; set; }
    [Required]
    public string? Name { get; set; }
    [Required(ErrorMessage = "Name is a required field.")]
    public string? Email { get; set; }
    public UserAttributes? Attributes { get; set; }
}

[Keyless]
public class UserAttributes
{    
    public int Age { get; set; }
    public bool IsEnabled { get; set; }
}
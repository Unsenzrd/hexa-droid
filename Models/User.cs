using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

public class User
{
    public User()
    {
        Attributes = new UserAttributes();
    }

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
[NotMapped]
public class UserAttributes
{
    public int Age { get; set; }
    public bool IsEnabled { get; set; }
}
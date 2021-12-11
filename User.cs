public class User
{
    public int Id { get; set; }
    public string? Name { get; set; }
    public string? Email { get; set; }
    public Dictionary<string, string>? Attributes { get; set; }
}
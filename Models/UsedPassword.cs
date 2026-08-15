namespace VoidPass.Models;

public class UsedPassword
{
    public string Hash { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; }
}
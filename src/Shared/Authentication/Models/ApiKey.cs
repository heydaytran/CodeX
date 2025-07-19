namespace Authentication.Models;

public class ApiKey
{
    public int Id { get; set; } 
    
    public string? ApplicationName { get; set; }
    public string? ApiKeyHash { get; set; } 
    
    public string? ApiKeyName { get; set; } 
    public bool IsActive { get; set; } 
    
    public DateTime? ExpiresAt { get; set; } 
    
    public DateTime CreatedAt { get; set; } 
    public List<string>? Roles { get; set; } = new List<string>();
}
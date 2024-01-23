namespace SampleMvcApp.Models;

public sealed class User
{
    public int Id { get; set; }
    public string Username { get; set; }
    public string PasswordHash { get; set; }
    public string Timezone { get; set; }
}
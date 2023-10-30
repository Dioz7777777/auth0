namespace SampleMvcApp;

public static class AuthState
{
    public static bool IsAuthenticated => !string.IsNullOrEmpty(AccessToken);
    public static string? AccessToken { get; set; }
}
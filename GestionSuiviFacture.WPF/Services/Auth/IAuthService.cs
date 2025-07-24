namespace GestionSuiviFacture.WPF.Services;

public interface IAuthService
{
    int UserID { get; }
    string Username { get; }
    bool IsAuthenticated { get; }
    event EventHandler? LogoutRequired;
    Task<string?> GetValidTokenAsync();
    Task<bool> LoginAsync(string username, string password, bool rememberMe);
    void Logout();
    Task<bool> TryAutoLoginAsync();
}
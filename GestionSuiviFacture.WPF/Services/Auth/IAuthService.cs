namespace GestionSuiviFacture.WPF.Services;

public interface IAuthService
{
    int UserID { get; }
    string Username { get; }
    bool IsAuthenticated { get; }

    Task<string?> GetValidTokenAsync();
    Task<bool> LoginAsync(string username, string password);
    void Logout();
}
namespace GestionSuiviFacture.WPF.Services;

public interface IAuthService
{
    public Task<bool> LoginAsync(string username, string password);
}

using Microsoft.Win32;
using System.Security.Cryptography;
using System.Text;

namespace GestionSuiviFacture.WPF.Services.Auth;

public class StorageCredential
{
    private const string REGISTRY_KEY = @"SOFTWARE\GestionSuiviFacture\Credentials";
    private const string USERNAME_KEY = "Username";
    private const string PASSWORD_KEY = "Password";

    public void SaveCredentials(string username, string password)
    {
        try
        {
            using (var key = Registry.CurrentUser.CreateSubKey(REGISTRY_KEY))
            {
                key.SetValue(USERNAME_KEY, username);
                string encryptedPassword = ProtectData(password);
                key.SetValue(PASSWORD_KEY, encryptedPassword);
            }
        }
        catch (Exception)
        {
        }
    }

    public (string username, string password) LoadCredentials()
    {
        try
        {
            using (var key = Registry.CurrentUser.OpenSubKey(REGISTRY_KEY))
            {
                if (key == null) return (string.Empty, string.Empty);

                string username = key.GetValue(USERNAME_KEY) as string ?? string.Empty;
                string encryptedPassword = key.GetValue(PASSWORD_KEY) as string ?? string.Empty;

                if (string.IsNullOrEmpty(encryptedPassword))
                    return (username, string.Empty);

                string password = UnprotectData(encryptedPassword);
                return (username, password);
            }
        }
        catch (Exception)
        {
            return (string.Empty, string.Empty);
        }
    }

    public void ClearCredentials()
    {
        try
        {
            Registry.CurrentUser.DeleteSubKeyTree(REGISTRY_KEY, false);
        }
        catch (Exception)
        {
        }
    }

    public bool HasStoredCredentials()
    {
        try
        {
            using (var key = Registry.CurrentUser.OpenSubKey(REGISTRY_KEY))
            {
                return key != null &&
                       !string.IsNullOrEmpty(key.GetValue(USERNAME_KEY) as string) &&
                       !string.IsNullOrEmpty(key.GetValue(PASSWORD_KEY) as string);
            }
        }
        catch
        {
            return false;
        }
    }

    private string ProtectData(string data)
    {
        byte[] dataBytes = Encoding.UTF8.GetBytes(data);
        byte[] protectedBytes = ProtectedData.Protect(dataBytes, null, DataProtectionScope.CurrentUser);
        return Convert.ToBase64String(protectedBytes);
    }

    private string UnprotectData(string protectedData)
    {
        byte[] protectedBytes = Convert.FromBase64String(protectedData);
        byte[] dataBytes = ProtectedData.Unprotect(protectedBytes, null, DataProtectionScope.CurrentUser);
        return Encoding.UTF8.GetString(dataBytes);
    }
}

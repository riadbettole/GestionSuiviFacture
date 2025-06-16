using System.Text.Json;

namespace GestionSuiviFacture.WPF.Configuration
{
    public static class JsonConfig
    {
        public static readonly JsonSerializerOptions DefaultOptions = new()
        {
            PropertyNameCaseInsensitive = true,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };
    }
}

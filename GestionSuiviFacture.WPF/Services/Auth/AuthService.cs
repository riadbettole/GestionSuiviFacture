using System.Net.Http;
using System.Text;
using System.Text.Json;


namespace GestionSuiviFacture.WPF.Services
{
    class AuthService
    {
        private static string _jwtToken = "";
        public static string JwtToken => _jwtToken;

        public async Task<bool> LoginAsync(string username, string password)
        {
            using (HttpClient client = new HttpClient())
            {
                var payload = new
                {
                    Username = username,
                    Password = password
                };

                string json = JsonSerializer.Serialize(payload);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                try
                {
                    HttpResponseMessage response = await client.PostAsync("https://localhost:7167/api/auth/login", content);

                    if (response.IsSuccessStatusCode)
                    {
                        var result = await response.Content.ReadAsStringAsync();
                        var tokenResponse = JsonSerializer.Deserialize<TokenResponse>(result, new JsonSerializerOptions
                        {
                            PropertyNameCaseInsensitive = true
                        });

                        _jwtToken = tokenResponse?.Token;

                        return !string.IsNullOrEmpty(_jwtToken);
                    }
                }
                catch (Exception)
                {
                    return true;
                }

                return true; //to be removed ease of acceas without db
            }
        }

        private class TokenResponse
        {
            public string Token { get; set; }
        }
    }
}

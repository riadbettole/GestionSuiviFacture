using System.IdentityModel.Tokens.Jwt;
using System.Net.Http;
using System.Security.Claims;
using System.Text;
using System.Text.Json;


namespace GestionSuiviFacture.WPF.Services
{
    class AuthService
    {
        private static string _jwtToken = "";
        private static int _userId = 0;
        private static string _username = "";
        private static string _role = "";
        public static string JwtToken => _jwtToken;
        public static int UserID => _userId;
        public static string Username => _username;
        public static string Role => _role;

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
                
                    HttpResponseMessage response = await client.PostAsync("https://localhost:7167/api/v1/auth/login", content);

                if (response.IsSuccessStatusCode)
                {
                    var result = await response.Content.ReadAsStringAsync();
                    var tokenResponse = JsonSerializer.Deserialize<TokenResponse>(result, new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });

                    _jwtToken = tokenResponse?.Token ?? string.Empty;

                    var handler = new JwtSecurityTokenHandler();
                    var jwtToken = handler.ReadJwtToken(_jwtToken);

                    // Extract claims
                    var claims = jwtToken.Claims.ToList();

                    _userId = Convert.ToInt16(claims.FirstOrDefault(c => c.Type == "id")?.Value);

                    var nameClaim = claims.FirstOrDefault(c => c.Type == ClaimTypes.Name)?.Value;
                    _username = nameClaim ?? string.Empty;

                    var roleClaim = claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value;
                    _role = roleClaim ?? string.Empty;



                    return !string.IsNullOrEmpty(_jwtToken);
                }

                return false; //to be removed ease of acceas without db
            }
        }

        private class TokenResponse
        {
            public string? Token { get; set; }
        }
    }
}

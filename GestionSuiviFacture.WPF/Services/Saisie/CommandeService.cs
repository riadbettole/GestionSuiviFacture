using GestionSuiviFacture.WPF.DTOs;
using GestionSuiviFacture.WPF.Models;
using GestionSuiviFacture.WPF.ViewModels.Facture;
using System.Diagnostics;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Web;

namespace GestionSuiviFacture.WPF.Services
{
    class CommandeService
    {
        private readonly HttpClient _httpClient;
        public Commande? _commande;
        public IEnumerable<BonDeLivraison>? _bonDeLivraison;
        private const string BaseUrl = "https://localhost:7167/api";

        public CommandeService()
        {
            _httpClient = new HttpClient();
            RefreshAuthorizationHeader();
        }

        public Task<Commande?> GetCommande()
        {
            return Task.FromResult(_commande);
        }

        public Task<IEnumerable<BonDeLivraison>?> GetBonLivraison()
        {
            return Task.FromResult(_bonDeLivraison);
        }

        public async Task<Commande?> GetCommandeByFilterAsync(
            DateTime? dateFacture = null,
            string? numSite = null,
            string? numCommande = null)
        {
            var queryString = BuildFilterQueryString(dateFacture, numSite, numCommande);
            RefreshAuthorizationHeader();

            var commandeDto = await FetchCommandDTOs($"{BaseUrl}/BonLivraison/INFOGOLD{queryString}");

            if (commandeDto != null)
            {
                _commande = MapToModel(commandeDto);
                _bonDeLivraison = MapToBonDeLivraison(commandeDto);
            }

            return _commande;
        }

        private async Task<CommandeDTO?> FetchCommandDTOs(string url)
        {
            try
            {
                var response = await _httpClient.GetAsync(url);
                response.EnsureSuccessStatusCode();

                var jsonString = await response.Content.ReadAsStringAsync();
                Debug.WriteLine($"JSON Response: {jsonString}");

                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                };

                var commandeDto = JsonSerializer.Deserialize<CommandeDTO>(jsonString, options);
                Debug.WriteLine($"Deserialized DTO: {commandeDto?.LibelleFournisseur}");
                Debug.WriteLine($"Bon Livraisons Count: {commandeDto?.BonsLivraison?.Values?.Count ?? 0}");

                return commandeDto;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error fetching commandes: {ex.Message}");
                throw;
            }
        }

        private string BuildFilterQueryString(
            DateTime? dateFacture = null,
            string? numSite = null,
            string? numCommande = null)
        {
            var parameters = HttpUtility.ParseQueryString(string.Empty);

            if (dateFacture.HasValue)
                parameters["date_facture"] = dateFacture.Value.ToString("yyyy-MM-dd");
            if (!string.IsNullOrEmpty(numSite))
                parameters["n_site"] = numSite;
            if (!string.IsNullOrEmpty(numCommande))
                parameters["n_commande"] = numCommande;

            return parameters.Count > 0 ? "?" + parameters.ToString() : "";
        }

        private Commande MapToModel(CommandeDTO dto)
        {
            return new Commande(
                NomFournisseur: dto.LibelleFournisseur,
                CNUF: dto.Cnuf,
                Site: null, // Not present in your JSON
                Rayon: dto.Rayon,
                MontantTTC: dto.MontantBRV,
                DateCommande: dto.date_Commande,
                DateEcheance: dto.DateEcheance
            );
        }

        private IEnumerable<BonDeLivraison> MapToBonDeLivraison(CommandeDTO dto)
        {
            if (dto.BonsLivraison?.Values == null)
                return Enumerable.Empty<BonDeLivraison>();

            return dto.BonsLivraison.Values.Select(blDto => new BonDeLivraison
            {
                NumeroLivraison = blDto.NumeroLivraison,
                DateReception = blDto.DateReception,
                MontantTTC = blDto.MontantTTC
            }).ToList();
        }

        private void RefreshAuthorizationHeader()
        {
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
                "Bearer",
                AuthService.JwtToken
            );
        }
    }
}
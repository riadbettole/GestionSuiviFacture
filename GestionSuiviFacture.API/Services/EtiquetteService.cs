using GestionSuiviFacture.API.Models;
using GestionSuiviFacture.WPF.ViewModels;
using Newtonsoft.Json;

namespace GestionSuiviFacture.API.Services
{
    public class EtiquetteService : IEtiquetteService
    {
        public async Task<Etiquette> GetEtiquette(string numSequence)
        {
            using (HttpClient client = new HttpClient())
            {
                string uri = "" + numSequence;

                HttpResponseMessage response = await client.GetAsync(uri);
                string jsonResponse = await response.Content.ReadAsStringAsync();

                Etiquette etiquette = JsonConvert.DeserializeObject<Etiquette>(jsonResponse);
                return etiquette;
            }
                //return new Etiquette{ Id = 1 };
        }

        public async Task<EtiquetteViewModel> Load()
        {

        }
    }
}

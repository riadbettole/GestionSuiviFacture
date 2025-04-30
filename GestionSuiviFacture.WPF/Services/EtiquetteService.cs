using System.Net.Http;
using GestionSuiviFacture.WPF.Models;
using Newtonsoft.Json;

namespace GestionSuiviFacture.WPF.Services
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

        public async Task<IEnumerable<Etiquette>> Load()
        {
            return new List<Etiquette>
            {
                new Etiquette("-14894349", "JIBAL", DateTime.Now, StatusEtiquette.NOK, "Aechalla"),
                new Etiquette("-14894350", "JIBAL", DateTime.Now, StatusEtiquette.NOK, "Aechalla"),
                new Etiquette("-14894351", "JIBAL", DateTime.Now, StatusEtiquette.OK, "Aechalla"),
                new Etiquette("-14894352", "JIBAL", DateTime.Now, StatusEtiquette.NOK, "Aechalla"),
                new Etiquette("-14894343", "JIBAL", DateTime.Now, StatusEtiquette.NOK, "Aechalla"),
                new Etiquette("-14894344", "JIBAL", DateTime.Now, StatusEtiquette.NOK, "Aechalla"),
                new Etiquette("-14894345", "JIBAL", DateTime.Now, StatusEtiquette.ANNULE, "Aechalla"),
                new Etiquette("-14894346", "COPAG", DateTime.Now, StatusEtiquette.NOK, "mbenaiad"),
                new Etiquette("-14894347", "COPAG", DateTime.Now, StatusEtiquette.NOK, "mbenaiad")
            };
        }
    }
}

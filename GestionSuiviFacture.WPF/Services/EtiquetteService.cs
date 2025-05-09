using System.Net.Http;
using System.Text.Json.Nodes;
using GestionSuiviFacture.WPF.DTOs;
using GestionSuiviFacture.WPF.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace GestionSuiviFacture.WPF.Services
{
    public class EtiquetteListWrapper
    {
        [JsonProperty("$id")]
        public string Id { get; set; }

        [JsonProperty("$values")]
        public List<EtiquetteDTO> Values { get; set; }
    }

    public class EtiquetteService : IEtiquetteService
    {
        public async Task<IEnumerable<Etiquette>> GetAllEtiquette()
        {
            using (HttpClient client = new HttpClient())
            {
                string uri = "https://localhost:7167/api/etiquette?pageNumber=1&pageSize=10";
                HttpResponseMessage response = await client.GetAsync(uri);
                string jsonResponse = await response.Content.ReadAsStringAsync();

                var jsonObject = JObject.Parse(jsonResponse);

                // Extract the $values array and deserialize it directly
                var etiquetteDTOs = jsonObject["$values"].ToObject<List<EtiquetteDTO>>();

                // Map each EtiquetteDTO to your Etiquette record
                var etiquettes = etiquetteDTOs.Select(dto => new Etiquette(
                    Magasin: dto.site?.n_Site,
                    Cnuf: dto.fournisseur?.cnuf,
                    NumSequence: dto.n_sequence,
                    NumFacture: dto.facture?.n_Facture,
                    NumReception: dto.reception?.n_Reception.ToString(),
                    NumCommande: dto.commande?.n_Commande,
                    DateTraitement: dto.date_Traitement,
                    DateReception: dto.reception?.date_Reception ?? DateTime.MinValue,
                    DateCommande: dto.commande?.date_Commande ?? DateTime.MinValue,
                    DateRapprochement: DateTime.MinValue, // Set this value as needed
                    Status: MapEnumStatus(dto.statut), // Assuming an enum mapping for the status
                    Fournisseur: dto.fournisseur?.libelle_Fournisseur,
                    MontantBF: 0, // You can add logic for MontantBF if needed
                    MontantFacture: dto.facture?.total_TTC ?? 0,
                    Utilisateur: null, // Set appropriate value if needed
                    UtilisateurAnnule: null, // Set appropriate value if needed
                    MotifAnnulation: null, // Set appropriate value if needed
                    Description: null, // Set appropriate value if needed
                    LibelleSite: dto.site?.libelle_Site,
                    GroupeSite: dto.commande?.groupe
                ));

                return etiquettes;
            }
        }

        private StatusEtiquette MapEnumStatus(int statut)
        {
            return statut switch
            {
                0 => StatusEtiquette.OK,
                1 => StatusEtiquette.NOK,
                2 => StatusEtiquette.ANNULE,
                _ => throw new NotImplementedException()
            };
        }

        List<Etiquette> etiquettes;

        public Task<IEnumerable<Etiquette>> GetEtiquetteByFilter(
            string? numSequence = null,
            DateTime? debut = null,
            DateTime? fin = null,
            string? numCommande = null,
            string? cnuf = null,
            StatusEtiquette? statusEtiquette = null
        )
        {
            IQueryable<Etiquette> query = etiquettes.AsQueryable();

            if (!string.IsNullOrEmpty(numSequence))
            {
                return Task.FromResult(
                    (
                        query
                            .Where(e =>
                                string.Equals(
                                    e.NumSequence,
                                    numSequence,
                                    StringComparison.OrdinalIgnoreCase
                                )
                            )
                            .AsEnumerable()
                    )
                );
            }

            if (debut.HasValue)
            {
                query = query.Where(e => e.DateTraitement >= debut);
            }
            if (fin.HasValue)
            {
                query = query.Where(e => e.DateTraitement <= fin);
            }
            if (!string.IsNullOrEmpty(numCommande))
            {
                query = query.Where(e => e.NumCommande.Equals(numCommande));
            }
            if (!string.IsNullOrEmpty(cnuf))
            {
                query = query.Where(e =>
                    string.Equals(e.Cnuf, cnuf, StringComparison.OrdinalIgnoreCase)
                );
            }
            if (statusEtiquette != null)
            {
                query = query.Where(e => e.Status == statusEtiquette);
            }

            return Task.FromResult(query.AsEnumerable());
        }

        public EtiquetteService()
        {
            etiquettes = new List<Etiquette>
            {
                new Etiquette(
                    Magasin: "Magasin A",
                    Cnuf: "CN001",
                    NumSequence: "SEQ123",
                    NumFacture: "F123456",
                    NumReception: "R987654",
                    NumCommande: "C123ABC",
                    DateTraitement: DateTime.Now,
                    DateReception: DateTime.Now.AddDays(-3),
                    DateCommande: DateTime.Now.AddDays(-5),
                    DateRapprochement: DateTime.Now.AddDays(-2),
                    Status: StatusEtiquette.OK,
                    Fournisseur: "Fournisseur A",
                    MontantBF: 500.75,
                    MontantFacture: 480.00,
                    Utilisateur: "User1",
                    UtilisateurAnnule: null,
                    MotifAnnulation: null,
                    Description: "Invoice processed successfully.",
                    LibelleSite: "Site1",
                    GroupeSite: "GroupX"
                ),
                new Etiquette(
                    Magasin: "Magasin B",
                    Cnuf: "CN002",
                    NumSequence: "SEQ124",
                    NumFacture: "F123457",
                    NumReception: "R987655",
                    NumCommande: "C123DEF",
                    DateTraitement: DateTime.Now.AddDays(-1),
                    DateReception: DateTime.Now.AddDays(-4),
                    DateCommande: DateTime.Now.AddDays(-6),
                    DateRapprochement: DateTime.Now.AddDays(-3),
                    Status: StatusEtiquette.NOK,
                    Fournisseur: "Fournisseur B",
                    MontantBF: 1000.50,
                    MontantFacture: 980.00,
                    Utilisateur: "User2",
                    UtilisateurAnnule: null,
                    MotifAnnulation: null,
                    Description: "Issue with invoice, payment pending.",
                    LibelleSite: "Site2",
                    GroupeSite: "GroupY"
                ),
                new Etiquette(
                    Magasin: "Magasin C",
                    Cnuf: "CN003",
                    NumSequence: "SEQ125",
                    NumFacture: "F123458",
                    NumReception: "R987656",
                    NumCommande: "C123GHI",
                    DateTraitement: DateTime.Now.AddDays(-5),
                    DateReception: DateTime.Now.AddDays(-10),
                    DateCommande: DateTime.Now.AddDays(-8),
                    DateRapprochement: DateTime.Now.AddDays(-6),
                    Status: StatusEtiquette.ANNULE,
                    Fournisseur: "Fournisseur C",
                    MontantBF: 750.00,
                    MontantFacture: 730.00,
                    Utilisateur: "User3",
                    UtilisateurAnnule: "User5",
                    MotifAnnulation: "Invoice canceled due to incorrect details.",
                    Description: "The order has been canceled.",
                    LibelleSite: "Site3",
                    GroupeSite: "GroupZ"
                ),
                new Etiquette(
                    Magasin: "Magasin D",
                    Cnuf: "CN004",
                    NumSequence: "SEQ126",
                    NumFacture: "F123459",
                    NumReception: "R987657",
                    NumCommande: "C123JKL",
                    DateTraitement: DateTime.Now,
                    DateReception: DateTime.Now.AddDays(-7),
                    DateCommande: DateTime.Now.AddDays(-10),
                    DateRapprochement: DateTime.Now.AddDays(-5),
                    Status: StatusEtiquette.OK,
                    Fournisseur: "Fournisseur D",
                    MontantBF: 1200.00,
                    MontantFacture: 1150.00,
                    Utilisateur: "User4",
                    UtilisateurAnnule: null,
                    MotifAnnulation: null,
                    Description: "Invoice processed and paid.",
                    LibelleSite: "Site4",
                    GroupeSite: "GroupW"
                ),
                new Etiquette(
                    Magasin: "Magasin E",
                    Cnuf: "CN005",
                    NumSequence: "SEQ127",
                    NumFacture: "F123460",
                    NumReception: "R987658",
                    NumCommande: "C123MNO",
                    DateTraitement: DateTime.Now.AddDays(-2),
                    DateReception: DateTime.Now.AddDays(-1),
                    DateCommande: DateTime.Now.AddDays(-3),
                    DateRapprochement: DateTime.Now.AddDays(-1),
                    Status: StatusEtiquette.NOK,
                    Fournisseur: "Fournisseur E",
                    MontantBF: 1100.00,
                    MontantFacture: 1075.00,
                    Utilisateur: "User5",
                    UtilisateurAnnule: null,
                    MotifAnnulation: null,
                    Description: "Invoice amount mismatch.",
                    LibelleSite: "Site5",
                    GroupeSite: "GroupV"
                ),
                new Etiquette(
                    Magasin: "Magasin F",
                    Cnuf: "CN006",
                    NumSequence: "SEQ128",
                    NumFacture: "F123461",
                    NumReception: "R987659",
                    NumCommande: "C123PQR",
                    DateTraitement: DateTime.Now.AddDays(-3),
                    DateReception: DateTime.Now.AddDays(-8),
                    DateCommande: DateTime.Now.AddDays(-4),
                    DateRapprochement: DateTime.Now.AddDays(-7),
                    Status: StatusEtiquette.ANNULE,
                    Fournisseur: "Fournisseur F",
                    MontantBF: 600.25,
                    MontantFacture: 590.00,
                    Utilisateur: "User6",
                    UtilisateurAnnule: "User2",
                    MotifAnnulation: "Canceled by user due to stock unavailability.",
                    Description: "The invoice was canceled.",
                    LibelleSite: "Site6",
                    GroupeSite: "GroupU"
                ),
                new Etiquette(
                    Magasin: "Magasin G",
                    Cnuf: "CN007",
                    NumSequence: "SEQ129",
                    NumFacture: "F123462",
                    NumReception: "R987660",
                    NumCommande: "C123STU",
                    DateTraitement: DateTime.Now.AddDays(-1),
                    DateReception: DateTime.Now.AddDays(-6),
                    DateCommande: DateTime.Now.AddDays(-9),
                    DateRapprochement: DateTime.Now.AddDays(-3),
                    Status: StatusEtiquette.OK,
                    Fournisseur: "Fournisseur G",
                    MontantBF: 1500.00,
                    MontantFacture: 1480.00,
                    Utilisateur: "User7",
                    UtilisateurAnnule: null,
                    MotifAnnulation: null,
                    Description: "Transaction completed successfully.",
                    LibelleSite: "Site7",
                    GroupeSite: "GroupT"
                ),
                new Etiquette(
                    Magasin: "Magasin H",
                    Cnuf: "CN008",
                    NumSequence: "SEQ130",
                    NumFacture: "F123463",
                    NumReception: "R987661",
                    NumCommande: "C123VWX",
                    DateTraitement: DateTime.Now.AddDays(-4),
                    DateReception: DateTime.Now.AddDays(-9),
                    DateCommande: DateTime.Now.AddDays(-11),
                    DateRapprochement: DateTime.Now.AddDays(-8),
                    Status: StatusEtiquette.NOK,
                    Fournisseur: "Fournisseur H",
                    MontantBF: 2000.50,
                    MontantFacture: 1985.00,
                    Utilisateur: "User8",
                    UtilisateurAnnule: null,
                    MotifAnnulation: null,
                    Description: "Payment pending, issue with processing.",
                    LibelleSite: "Site8",
                    GroupeSite: "GroupS"
                ),
                new Etiquette(
                    Magasin: "Magasin I",
                    Cnuf: "CN009",
                    NumSequence: "SEQ131",
                    NumFacture: "F123464",
                    NumReception: "R987662",
                    NumCommande: "C123XYZ",
                    DateTraitement: DateTime.Now.AddDays(-7),
                    DateReception: DateTime.Now.AddDays(-12),
                    DateCommande: DateTime.Now.AddDays(-10),
                    DateRapprochement: DateTime.Now.AddDays(-9),
                    Status: StatusEtiquette.ANNULE,
                    Fournisseur: "Fournisseur I",
                    MontantBF: 1800.75,
                    MontantFacture: 1750.00,
                    Utilisateur: "User9",
                    UtilisateurAnnule: "User1",
                    MotifAnnulation: "Order canceled due to incorrect shipping details.",
                    Description: "Invoice and order canceled.",
                    LibelleSite: "Site9",
                    GroupeSite: "GroupR"
                ),
                new Etiquette(
                    Magasin: "Magasin J",
                    Cnuf: "CN010",
                    NumSequence: "SEQ132",
                    NumFacture: "F123465",
                    NumReception: "R987663",
                    NumCommande: "C123ABC",
                    DateTraitement: DateTime.Now.AddDays(-8),
                    DateReception: DateTime.Now.AddDays(-13),
                    DateCommande: DateTime.Now.AddDays(-15),
                    DateRapprochement: DateTime.Now.AddDays(-10),
                    Status: StatusEtiquette.OK,
                    Fournisseur: "Fournisseur J",
                    MontantBF: 2500.00,
                    MontantFacture: 2450.00,
                    Utilisateur: "User10",
                    UtilisateurAnnule: null,
                    MotifAnnulation: null,
                    Description: "Successfully processed and paid.",
                    LibelleSite: "Site10",
                    GroupeSite: "GroupQ"
                ),
            };
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GestionSuiviFacture.API.Models;

namespace GestionSuiviFacture.API.Services
{
    public interface IEtiquetteService
    {
        Task<Etiquette> GetEtiquette(String numSequence);
    }
}

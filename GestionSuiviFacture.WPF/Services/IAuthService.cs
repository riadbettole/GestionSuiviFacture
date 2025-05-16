using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GestionSuiviFacture.WPF.Services
{
    public interface IAuthService
    {
        public Task<bool> LoginAsync(string username, string password);
    }
}

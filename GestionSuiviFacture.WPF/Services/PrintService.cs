using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.IO;
using System.Diagnostics;
using GestionSuiviFacture.WPF.Models;

namespace GestionSuiviFacture.WPF.Services
{
    public interface IPrintService
    {
        Task<bool> PrintEtiquetteAsync(Etiquette etiquette, string printerName = null);
        Task<bool> PreviewEtiquetteAsync(Etiquette etiquette);
        bool PrintEtiquette(Etiquette etiquette, string printerName = null);
        bool PreviewEtiquette(Etiquette etiquette);
    }

    public class PrintService : IPrintService
    {
        // Configuration des imprimantes réseau
        private readonly Dictionary<string, PrinterConfig> _printers = new Dictionary<string, PrinterConfig>
        {
            {
                "ZDesigner S4M-203dpiZPL",
                new PrinterConfig { IPAddress = "192.168.1.100", Port = 9100, DPI = 203 }
            },
            {
                "ZDesigner 220xi4 203 dpi",
                new PrinterConfig { IPAddress = "192.168.1.101", Port = 9100, DPI = 203 }
            }
        };

        private const int LABEL_WIDTH_DOTS = 496;  // 6.2cm à 203 DPI (6.2 * 203 / 2.54)
        private const int LABEL_HEIGHT_DOTS = 304; // 3.8cm à 203 DPI (3.8 * 203 / 2.54)

        public async Task<bool> PrintEtiquetteAsync(Etiquette etiquette, string printerName = null)
        {
            return await Task.Run(() => PrintEtiquette(etiquette, printerName));
        }

        public async Task<bool> PreviewEtiquetteAsync(Etiquette etiquette)
        {
            return await Task.Run(() => PreviewEtiquette(etiquette));
        }

        public bool PrintEtiquette(Etiquette etiquette, string printerName = null)
        {
            try
            {
                if (etiquette == null)
                    throw new ArgumentNullException(nameof(etiquette));

                // Si aucune imprimante spécifiée, utiliser la première disponible
                if (string.IsNullOrEmpty(printerName))
                {
                    printerName = _printers.Keys.FirstOrDefault();
                }

                if (!_printers.ContainsKey(printerName))
                {
                    MessageBox.Show($"Imprimante '{printerName}' non configurée.", "Erreur",
                        MessageBoxButton.OK, MessageBoxImage.Error);
                    return false;
                }

                var printerConfig = _printers[printerName];
                var zplCode = GenerateZPLCode(etiquette);

                return SendZPLToPrinter(zplCode, printerConfig);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erreur lors de l'impression: {ex.Message}", "Erreur d'impression",
                    MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
        }

        public bool PreviewEtiquette(Etiquette etiquette)
        {
            try
            {
                if (etiquette == null)
                    throw new ArgumentNullException(nameof(etiquette));

                var zplCode = GenerateZPLCode(etiquette);
                var previewFile = CreateZPLPreview(etiquette, zplCode);

                if (!string.IsNullOrEmpty(previewFile))
                {
                    ShowPreview(previewFile);
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erreur lors de la prévisualisation: {ex.Message}", "Erreur de prévisualisation",
                    MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
        }
        private string GenerateZPLCode(Etiquette etiquette)
        {
            var zpl = new StringBuilder();

            // Début de l'étiquette
            zpl.AppendLine("^XA");

            // Section Localisation (sans espaces)
            zpl.AppendLine("^FO5,20^CF0,12^FDGroupe:^FS");
            zpl.AppendLine($"^FO60,20^CF0,12^FD{TruncateText(etiquette.GroupeSite, 25)}^FS");

            zpl.AppendLine("^FO5,35^CF0,12^FDSite:^FS");
            zpl.AppendLine($"^FO60,35^CF0,12^FD{TruncateText(etiquette.LibelleSite, 25)}^FS");

            // Fournisseur (format avec deux points)
            zpl.AppendLine("^FO5,55^CF0,10^FDFournisseur:^FS");
            zpl.AppendLine($"^FO70,55^CF0,10^FD{TruncateText(etiquette.Fournisseur, 35)}^FS");

            zpl.AppendLine("^FO5,70^CF0,10^FDNCommande:^FS");
            zpl.AppendLine($"^FO80,70^CF0,10^FD{TruncateText(etiquette.NumCommande, 20)}^FS");

            zpl.AppendLine("^FO5,85^CF0,10^FDNFacture:^FS");
            zpl.AppendLine($"^FO80,85^CF0,10^FD{TruncateText(etiquette.NumFacture, 20)}^FS");

            zpl.AppendLine("^FO5,100^CF0,10^FDNSequence:^FS");
            zpl.AppendLine($"^FO80,100^CF0,10^FD{etiquette.NumSequence}^FS");

            // Section Montants (format original avec point décimal)
            zpl.AppendLine("^FO5,120^CF0,11^FDFacture:^FS");
            zpl.AppendLine($"^FO60,120^CF0,11^FD{etiquette.MontantFacture?.ToString("N2") ?? "0.00"} Dh^FS");

            zpl.AppendLine("^FO5,135^CF0,11^FDBRV:^FS");
            zpl.AppendLine($"^FO60,135^CF0,11^FD{etiquette.MontantBRV?.ToString("N2") ?? "0.00"} Dh^FS");

            // Statut
            var statusText = MapStatutToString(etiquette.Status);
            zpl.AppendLine($"^FO60,155^CF0,12^FD{statusText}^FS");

            // Date et utilisateur
            var dateText = etiquette.DateTraitement?.ToString("dd/MM/yy HH:mm") ?? DateTime.Now.ToString("dd/MM/yy HH:mm");
            var userText = TruncateText(etiquette.Utilisateur ?? "N/A", 20);

            zpl.AppendLine($"^FO5,180^CF0,8^FDDate: {dateText}^FS");
            zpl.AppendLine($"^FO5,195^CF0,8^FDAgent: {userText}^FS");

            // Fin de l'étiquette
            zpl.AppendLine("^XZ");

            return zpl.ToString();
        }

        private bool SendZPLToPrinter(string zplCode, PrinterConfig config)
        {
            try
            {
                using (var client = new TcpClient())
                {
                    client.ReceiveTimeout = 5000;
                    client.SendTimeout = 5000;

                    var connectTask = client.ConnectAsync(config.IPAddress, config.Port);
                    if (!connectTask.Wait(5000))
                    {
                        throw new TimeoutException("Timeout lors de la connexion à l'imprimante");
                    }

                    using (var stream = client.GetStream())
                    {
                        byte[] data = Encoding.UTF8.GetBytes(zplCode);
                        stream.Write(data, 0, data.Length);
                        stream.Flush();
                    }
                }

                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erreur de communication avec l'imprimante: {ex.Message}",
                    "Erreur réseau", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
        }

        private string CreateZPLPreview(Etiquette etiquette, string zplCode)
        {
            var previewFile = Path.Combine(Path.GetTempPath(),
                $"EtiquettePreview_{etiquette.NumSequence}_{DateTime.Now:yyyyMMdd_HHmmss}.txt");

            try
            {
                // Créer un fichier texte avec le code ZPL et les informations
                var previewContent = new StringBuilder();
                previewContent.AppendLine("=== CODE ZPL GÉNÉRÉ ===");
                previewContent.AppendLine(zplCode);
                previewContent.AppendLine("=== INSTRUCTIONS ===");
                previewContent.AppendLine("Ce code ZPL peut être testé sur:");
                previewContent.AppendLine("- http://labelary.com/viewer.html");
                previewContent.AppendLine("- Copiez le code ZPL dans l'éditeur en ligne");

                File.WriteAllText(previewFile, previewContent.ToString(), Encoding.UTF8);
                return previewFile;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erreur lors de la création du fichier de prévisualisation: {ex.Message}",
                    "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
                return null;
            }
        }

        private void ShowPreview(string previewFile)
        {
            try
            {
                var processStartInfo = new ProcessStartInfo
                {
                    FileName = previewFile,
                    UseShellExecute = true,
                    Verb = "open"
                };

                Process.Start(processStartInfo);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Impossible d'ouvrir la prévisualisation: {ex.Message}\nFichier sauvegardé à: {previewFile}",
                    "Information", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private static string GetStatusZPLColor(int? status)
        {
            return status switch
            {
                0 => "GREEN",    // OK
                1 => "RED",      // NOK
                2 => "ORANGE",   // ANNULE
                _ => "BLACK"
            };
        }

        private static string MapStatutToString(int? statut)
        {
            return statut switch
            {
                0 => "OK",
                1 => "NOK",
                2 => "ANNULE",
                _ => "INCONNU"
            };
        }

        private static string TruncateText(string text, int maxLength)
        {
            if (string.IsNullOrEmpty(text))
                return "";

            return text.Length <= maxLength ? text : text.Substring(0, maxLength - 3) + "...";
        }
    }

    // Classe de configuration pour les imprimantes
    public class PrinterConfig
    {
        public string IPAddress { get; set; }
        public int Port { get; set; } = 9100;
        public int DPI { get; set; } = 203;
    }
}
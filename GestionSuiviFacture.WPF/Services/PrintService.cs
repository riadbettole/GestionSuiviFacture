using GestionSuiviFacture.WPF.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Xps.Packaging;

namespace GestionSuiviFacture.WPF.Services
{
    public interface IPrintService
    {
        void PrintEtiquette(Etiquette etiquette);
    }
    public class PrintService : IPrintService
    {
        public void PreviewEtiquette(Etiquette etiquette)
        {
            var xpsFile = CreateXpsPreview(etiquette);
            ShowXpsPreview(xpsFile);
        }

        public void PrintEtiquette(Etiquette etiquette)
        {
            var printDialog = new PrintDialog();

            var pageSize = new Size(
                ConvertCmToDeviceUnits(3.0),
                ConvertCmToDeviceUnits(5.0)
            );

            var ticketVisual = CreateTicketVisual(etiquette, pageSize);

            if (printDialog.ShowDialog() == true)
            {
                printDialog.PrintVisual(ticketVisual, "Etiquette Ticket");
            }
        }

        private string CreateXpsPreview(Etiquette etiquette)
        {
            var xpsFile = Path.Combine(Path.GetTempPath(), $"EtiquettePreview_{Guid.NewGuid()}.xps");

            using (var xpsDocument = new XpsDocument(xpsFile, FileAccess.Write))
            {
                var xpsWriter = XpsDocument.CreateXpsDocumentWriter(xpsDocument);

                // Create the visual with exact print dimensions
                var pageSize = new Size(
                    ConvertCmToDeviceUnits(3.0),
                    ConvertCmToDeviceUnits(5.0)
                );

                var ticketVisual = CreateTicketVisual(etiquette, pageSize);

                // Write to XPS
                xpsWriter.Write(ticketVisual);
            }

            return xpsFile;
        }

        private void ShowXpsPreview(string xpsFile)
        {
            try
            {
                // Option 1: Use Windows default XPS viewer
                Process.Start(new ProcessStartInfo
                {
                    FileName = xpsFile,
                    UseShellExecute = true
                });
            }
            catch (Exception ex)
            {
                // Fallback: Try to open with any available viewer
                MessageBox.Show($"Cannot open XPS preview: {ex.Message}\nFile saved at: {xpsFile}");
            }
        }

        public void ShowPrintPreview(Etiquette etiquette)
        {
            var printDialog = new PrintDialog();

            // Create the visual
            var pageSize = new Size(
                ConvertCmToDeviceUnits(3.0),
                ConvertCmToDeviceUnits(5.0)
            );

            var ticketVisual = CreateTicketVisual(etiquette, pageSize);

            // Show print preview dialog
            if (printDialog.ShowDialog() == true)
            {
                // This will show Windows' print preview
                printDialog.PrintVisual(ticketVisual, "Etiquette Preview");
            }
        }
        private double ConvertCmToDeviceUnits(double cm)
        {
            return cm * 37.795275591; // 96 DPI conversion
        }

        private FrameworkElement CreateTicketVisual(Etiquette etiquette, Size pageSize)
        {
            var border = new Border
            {
                Width = pageSize.Width,
                Height = pageSize.Height,
                Background = Brushes.White,
                BorderBrush = Brushes.Black,
                BorderThickness = new Thickness(1),
                Child = new StackPanel
                {
                    Margin = new Thickness(5),
                    Children =
                {
                    new TextBlock
                    {
                        Text = "ETIQUETTE",
                        FontWeight = FontWeights.Bold,
                        FontSize = 8,
                        HorizontalAlignment = HorizontalAlignment.Center,
                        Margin = new Thickness(0, 0, 0, 2)
                    },
                    new TextBlock
                    {
                        Text = $"N°: {etiquette.NumSequence}",
                        FontSize = 7,
                        FontWeight = FontWeights.Bold
                    },
                    new TextBlock
                    {
                        Text = $"Date: {etiquette.DateTraitement:dd/MM/yy HH:mm}",
                        FontSize = 6
                    },
                    new TextBlock
                    {
                        Text = $"Status: {etiquette.Status}",
                        FontSize = 6,
                    },
                    new TextBlock
                    {
                        Text = $"Cmd: {etiquette.NumCommande}",
                        FontSize = 6,
                        TextTrimming = TextTrimming.CharacterEllipsis
                    },
                    new TextBlock
                    {
                        Text = $"Montant: {etiquette.MontantBRV:C}",
                        FontSize = 6,
                        FontWeight = FontWeights.Bold
                    },
                    new TextBlock
                    {
                        Text = $"Fourn: {etiquette.Fournisseur}",
                        FontSize = 5,
                        TextTrimming = TextTrimming.CharacterEllipsis
                    },
                    new TextBlock
                    {
                        Text = $"Site: {etiquette.LibelleSite}",
                        FontSize = 5,
                        TextTrimming = TextTrimming.CharacterEllipsis
                    },
                    new TextBlock
                    {
                        Text = $"Magasin: {etiquette.Magasin}",
                        FontSize = 5,
                        TextTrimming = TextTrimming.CharacterEllipsis
                    },
                    new TextBlock
                    {
                        Text = $"User: {etiquette.Utilisateur}",
                        FontSize = 5,
                        TextTrimming = TextTrimming.CharacterEllipsis,
                        Margin = new Thickness(0, 2, 0, 0)
                    }
                }
                }
            };

            return border;
        }

        ///private Brush GetStatusBrush(StatusEtiquette status)
        //{
        //    return status switch
        //    {
        //        StatusEtiquette.Active => Brushes.Green,
        //        StatusEtiquette.Cancelled => Brushes.Red,
        //        StatusEtiquette.Pending => Brushes.Orange,
        //        _ => Brushes.Black
        //    };
        //}
    }
}
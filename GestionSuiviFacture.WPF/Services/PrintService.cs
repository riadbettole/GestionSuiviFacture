using System;
using System.Diagnostics;
using System.IO;
using System.Printing;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Xps.Packaging;
using GestionSuiviFacture.WPF.Models;

namespace GestionSuiviFacture.WPF.Services;

    public interface IPrintService
    {
        Task<bool> PrintEtiquetteAsync(Etiquette etiquette, string printerName = null);
        Task<bool> PreviewEtiquetteAsync(Etiquette etiquette);
        bool PrintEtiquette(Etiquette etiquette, string printerName = null);
        bool PreviewEtiquette(Etiquette etiquette);
    }

    public class PrintService : IPrintService
    {
        private const double LABEL_WIDTH_CM = 3.0;
        private const double LABEL_HEIGHT_CM = 5.0;
        private const double DPI_CONVERSION = 37.795275591; // 96 DPI

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

                var printDialog = new PrintDialog();

                // Set specific printer if provided
                if (!string.IsNullOrEmpty(printerName))
                {
                    var printQueue = GetPrintQueue(printerName);
                    if (printQueue != null)
                    {
                        printDialog.PrintQueue = printQueue;
                    }
                }

                // Configure for label printing
                ConfigurePrintDialog(printDialog);

                var pageSize = new Size(
                    ConvertCmToDeviceUnits(LABEL_WIDTH_CM),
                    ConvertCmToDeviceUnits(LABEL_HEIGHT_CM)
                );

                var ticketVisual = CreateTicketVisual(etiquette, pageSize);

                if (printDialog.ShowDialog() == true)
                {
                    printDialog.PrintVisual(ticketVisual, $"Etiquette_{etiquette.NumSequence}");
                    return true;
                }

                return false;
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

                var xpsFile = CreateXpsPreview(etiquette);
                if (!string.IsNullOrEmpty(xpsFile))
                {
                    ShowXpsPreview(xpsFile);
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

        private PrintQueue GetPrintQueue(string printerName)
        {
            try
            {
                var printServer = new PrintServer();
                return printServer.GetPrintQueue(printerName);
            }
            catch
            {
                return null;
            }
        }

        private void ConfigurePrintDialog(PrintDialog printDialog)
        {
            // Set page size for label
            var pageSize = new Size(
                ConvertCmToDeviceUnits(LABEL_WIDTH_CM),
                ConvertCmToDeviceUnits(LABEL_HEIGHT_CM)
            );

            // Configure print ticket for label printing
            if (printDialog.PrintTicket != null)
            {
                printDialog.PrintTicket.PageMediaSize = new PageMediaSize(pageSize.Width, pageSize.Height);
                printDialog.PrintTicket.PageOrientation = PageOrientation.Portrait;
            }
        }

        private string CreateXpsPreview(Etiquette etiquette)
        {
            var xpsFile = Path.Combine(Path.GetTempPath(), $"EtiquettePreview_{etiquette.NumSequence}_{DateTime.Now:yyyyMMdd_HHmmss}.xps");

            try
            {
                using (var xpsDocument = new XpsDocument(xpsFile, FileAccess.Write))
                {
                    var xpsWriter = XpsDocument.CreateXpsDocumentWriter(xpsDocument);

                    var pageSize = new Size(
                        ConvertCmToDeviceUnits(LABEL_WIDTH_CM),
                        ConvertCmToDeviceUnits(LABEL_HEIGHT_CM)
                    );

                    var ticketVisual = CreateTicketVisual(etiquette, pageSize);
                //xpsWriter.Write(ticketVisual);
                var fixedPage = new FixedPage
                {
                    Width = pageSize.Width,
                    Height = pageSize.Height,
                    Background = Brushes.White
                };

                FixedPage.SetLeft(ticketVisual, 0);
                FixedPage.SetTop(ticketVisual, 0);
                fixedPage.Children.Add(ticketVisual);

                xpsWriter.Write(fixedPage);
            }

                return xpsFile;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erreur lors de la création du fichier XPS: {ex.Message}", "Erreur",
                    MessageBoxButton.OK, MessageBoxImage.Error);
                return null;
            }
        }

        private void ShowXpsPreview(string xpsFile)
        {
            try
            {
                var processStartInfo = new ProcessStartInfo
                {
                    FileName = xpsFile,
                    UseShellExecute = true,
                    Verb = "open"
                };

                Process.Start(processStartInfo);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Impossible d'ouvrir la prévisualisation XPS: {ex.Message}\nFichier sauvegardé à: {xpsFile}",
                    "Erreur de prévisualisation", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private static double ConvertCmToDeviceUnits(double cm)
        {
            return cm * DPI_CONVERSION;
        }

    private static Border CreateTicketVisual(Etiquette etiquette, Size pageSize)
    {
        var mainGrid = new Grid
        {
            Margin = new Thickness(6),
            Background = Brushes.White
        };

        // Define rows for better spacing
        mainGrid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto }); // Header
        mainGrid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Star) }); // Content
        mainGrid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto }); // Footer

        // Header section with background
        var headerBorder = new Border
        {
            Background = new SolidColorBrush(Color.FromRgb(240, 240, 240)),
            BorderBrush = Brushes.DarkGray,
            BorderThickness = new Thickness(0, 0, 0, 1),
            Padding = new Thickness(4, 2, 4, 2),
            Child = CreateTextBlock("ETIQUETTE", 10, FontWeights.Bold, HorizontalAlignment.Center)
        };
        Grid.SetRow(headerBorder, 0);
        mainGrid.Children.Add(headerBorder);

        // Content section
        var contentStackPanel = new StackPanel
        {
            Margin = new Thickness(2, 4, 2, 2),
            HorizontalAlignment = HorizontalAlignment.Stretch
        };

        // Group information in sections
        contentStackPanel.Children.Add(CreateSectionHeader("LOCALISATION"));
        contentStackPanel.Children.Add(CreateInfoRow("Groupe", etiquette.GroupeSite));
        contentStackPanel.Children.Add(CreateInfoRow("Site", etiquette.LibelleSite));

        contentStackPanel.Children.Add(CreateSeparator());

        contentStackPanel.Children.Add(CreateSectionHeader("COMMANDE"));
        contentStackPanel.Children.Add(CreateInfoRow("Fournisseur", etiquette.Fournisseur));
        contentStackPanel.Children.Add(CreateInfoRow("N° Commande", etiquette.NumCommande));
        contentStackPanel.Children.Add(CreateInfoRow("N° Facture", etiquette.NumFacture));
        contentStackPanel.Children.Add(CreateInfoRow("N° Séquence", etiquette.NumSequence.ToString()));

        contentStackPanel.Children.Add(CreateSeparator());

        contentStackPanel.Children.Add(CreateSectionHeader("MONTANTS"));
        contentStackPanel.Children.Add(CreateAmountRow("Facture", etiquette.MontantFacture));
        contentStackPanel.Children.Add(CreateAmountRow("BRV", etiquette.MontantBRV));

        Grid.SetRow(contentStackPanel, 1);
        mainGrid.Children.Add(contentStackPanel);

        // Footer section
        var footerGrid = new Grid
        {
            Margin = new Thickness(2, 2, 2, 4),
            Background = new SolidColorBrush(Color.FromRgb(250, 250, 250))
        };

        footerGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
        footerGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });

        // Status with colored background
        var statusBorder = new Border
        {
            Background = GetStatusColor(etiquette.Status),
            CornerRadius = new CornerRadius(3),
            Padding = new Thickness(4, 1, 4, 1),
            Child = CreateTextBlock(MapStatutToString(etiquette.Status), 7, FontWeights.Bold, ColorBrush: Brushes.White)
        };
        Grid.SetColumn(statusBorder, 1);
        footerGrid.Children.Add(statusBorder);

        // Date and user info
        var infoPanel = new StackPanel
        {
            Orientation = Orientation.Vertical
        };
        infoPanel.Children.Add(CreateTextBlock($"Date: {etiquette.DateTraitement:dd/MM/yy HH:mm}", 6));
        infoPanel.Children.Add(CreateTextBlock($"Agent: {etiquette.Utilisateur}", 6));
        Grid.SetColumn(infoPanel, 0);
        footerGrid.Children.Add(infoPanel);

        Grid.SetRow(footerGrid, 2);
        mainGrid.Children.Add(footerGrid);

        var border = new Border
        {
            Width = pageSize.Width,
            Height = pageSize.Height,
            Background = Brushes.White,
            BorderBrush = Brushes.Black,
            BorderThickness = new Thickness(1),
            Child = mainGrid
        };

        return border;
    }

    // Helper methods for better visual elements
    private static TextBlock CreateSectionHeader(string text)
    {
        return new TextBlock
        {
            Text = text,
            FontSize = 7,
            FontWeight = FontWeights.Bold,
            Foreground = Brushes.DarkBlue,
            Margin = new Thickness(0, 3, 0, 1),
            HorizontalAlignment = HorizontalAlignment.Left
        };
    }

    private static Grid CreateInfoRow(string label, string value)
    {
        var grid = new Grid
        {
            Margin = new Thickness(0, 1, 0, 1)
        };

        grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(40, GridUnitType.Pixel) });
        grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });

        var labelBlock = CreateTextBlock($"{label}:", 6, FontWeights.Bold);
        Grid.SetColumn(labelBlock, 0);
        grid.Children.Add(labelBlock);

        var valueBlock = CreateTextBlock(value ?? "", 6, textTrimming: TextTrimming.CharacterEllipsis);
        Grid.SetColumn(valueBlock, 1);
        grid.Children.Add(valueBlock);

        return grid;
    }

    private static Grid CreateAmountRow(string label, double? amount)
    {
        var grid = new Grid
        {
            Margin = new Thickness(0, 1, 0, 1)
        };

        grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(40, GridUnitType.Pixel) });
        grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });

        var labelBlock = CreateTextBlock($"{label}:", 6, FontWeights.Bold);
        Grid.SetColumn(labelBlock, 0);
        grid.Children.Add(labelBlock);

        var valueBlock = CreateTextBlock($"{amount:N2} Dh", 6, FontWeights.Bold);
        Grid.SetColumn(valueBlock, 1);
        grid.Children.Add(valueBlock);

        return grid;
    }

    private static Border CreateSeparator()
    {
        return new Border
        {
            Height = 1,
            Background = Brushes.LightGray,
            Margin = new Thickness(0, 2, 0, 2)
        };
    }

    private static Brush GetStatusColor(int? status)
    {
        return status switch
        {
            0 => Brushes.Green,      // OK
            1 => Brushes.Red,        // NOK
            2 => Brushes.Orange,     // ANNULE
            _ => Brushes.Gray
        };
    }
     
    private static TextBlock CreateTextBlock(string text, double fontSize,
            FontWeight fontWeight = default, HorizontalAlignment horizontalAlignment = HorizontalAlignment.Left,
            Thickness margin = default, TextTrimming textTrimming = TextTrimming.None, Brush ColorBrush = null)
        {
            return new TextBlock
            {
                Text = text,
                FontSize = fontSize,
                FontWeight = fontWeight == default ? FontWeights.Normal : fontWeight,
                HorizontalAlignment = horizontalAlignment,
                Margin = margin,
                TextTrimming = textTrimming,
                TextWrapping = TextWrapping.NoWrap,
                Foreground = ColorBrush      ?? Brushes.Black
            };
        }

        private static string MapStatutToString(int? statut)
        {
            return statut switch
            {
                0 => "OK",
                1 => "NOK",
                2 => "ANNULE",
                _ => throw new ArgumentOutOfRangeException(nameof(statut), "Unknown status value"),
            };
        }

    }

      
    
﻿using CommunityToolkit.Mvvm.ComponentModel;
using GestionSuiviFacture.WPF.Models;

namespace GestionSuiviFacture.WPF.ViewModels.Helpers;

public class CommandeViewModel : ObservableObject
{
    private readonly Commande _commande;

    public CommandeViewModel(Commande commande)
    {
        _commande = commande;
    }

    public string? NomFournisseur => _commande.NomFournisseur;
    public string? CNUF => _commande.CNUF;
    public string? Site => _commande.Site;
    public string? Rayon => _commande.Rayon;
    public string? Groupe => _commande.Groupe;
    public double? MontantTTC => _commande.MontantTTC;
    public DateTime? DateCommande => _commande.DateCommande;
    public DateTime? DateEcheance => _commande.DateEcheance;
}

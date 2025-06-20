﻿using CommunityToolkit.Mvvm.ComponentModel;

namespace GestionSuiviFacture.WPF.ViewModels.Helpers;

public partial class TaxDetail : ObservableObject
{
    [ObservableProperty]
    private int _tauxPercentage;

    public TaxDetail(int tauxPercentage, double montantHT)
    {
        TauxPercentage = tauxPercentage;
        MontantHT = montantHT;
        calculTVA();
    }

    [ObservableProperty]
    private double _montantHT;

    [ObservableProperty]
    private double _montantTVA;

    public void calculTVA()
    {
        MontantTVA = MontantHT * TauxPercentage / 100;
    }
}

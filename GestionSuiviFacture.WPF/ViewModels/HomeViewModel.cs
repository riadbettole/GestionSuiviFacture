namespace GestionSuiviFacture.WPF.ViewModels
{
    class HomeViewModel
    {
        public ConsultationViewModel ConsultationViewModel { get; set; }

        public HomeViewModel(ConsultationViewModel consultationViewModel)
        {
            ConsultationViewModel = consultationViewModel;
        }
    }
}

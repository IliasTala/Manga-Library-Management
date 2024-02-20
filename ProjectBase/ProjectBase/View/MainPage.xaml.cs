namespace ProjectBase
{
    public partial class MainPage : ContentPage
    {
        private MainViewModel _viewModel;

        // Constructeur de la classe MainPage qui prend un MainViewModel en paramètre
        public MainPage(MainViewModel viewModel)
        {
            InitializeComponent();
            // Définit le BindingContext sur le MainViewModel passé en paramètre
            BindingContext = viewModel;

            // Stocke le MainViewModel dans une variable privée pour une utilisation ultérieure
            _viewModel = viewModel;

            // Définit la méthode UpdateImageSource du MainViewModel comme la méthode de rafraîchissement de la page
            _viewModel.RefreshPage = UpdateImageSource;

            // Appelle la méthode SetImageSource pour définir la source de l'image en fonction des préférences
            SetImageSource();
        }

        // Méthode privée pour définir la source de l'image en fonction des préférences de couleur
        private void SetImageSource()
        {
            // Récupère la préférence de couleur depuis les préférences par défaut
            string backColor = Preferences.Default.Get("MainPageColor", "Unknown");

            // Vérifie la valeur de la préférence de couleur et détermine la source de l'image en conséquence
            if (backColor == "Dark")
            {
                Blabla.Source = "background_test2_dark.png";
            }
            else
            {
                Blabla.Source = "background_test2.png";
            }
        }

        // Méthode publique appelée par le MainViewModel pour rafraîchir la source de l'image
        public void UpdateImageSource()
        {
            SetImageSource();
        }
    }
}


using System.Collections.ObjectModel;
using ProjectBase.Services;
using System.Text;
using System.Net;
using System.Data;


namespace ProjectBase.ViewModel;

public partial class MainViewModel : BaseViewModel
{
    
    // Propriétés
    [ObservableProperty]
	public string monCode;

    private string _buttonChangeBackground = "Change Background";
    private Color _buttonBackgroundColor = Color.FromRgb(32, 32, 32);

    public string Username { get; set; }
    public string Password { get; set; }
    public bool IsAdmin { get; private set; }
    public bool IsConnected { get; private set; }
    public bool IsNotConnected { get; private set; }

    // Propriétés avec gestion des changements et notification
    public string ButtonChangeBackground
    {
        get => _buttonChangeBackground;
        set
        {
            _buttonChangeBackground = value;
            OnPropertyChanged();
        }
    }
    public UserViewModel UserVM { get; set; }
    public Color ButtonBackgroundColor
    {
        get => _buttonBackgroundColor;
        set
        {
            _buttonBackgroundColor = value;
            OnPropertyChanged();
        }
    }
    public Action RefreshPage { get; set; }

    // Constructeur
    public MainViewModel()
    {
        var myDBService = new UserManagementServices();
        var createUserTables = new CreateUserTables(); // Ajout de cette ligne pour créer les tables avant d'instancier UserViewModel
        UserVM = new UserViewModel(myDBService);

        // Récupérer la couleur actuelle de la page depuis les préférences
        string currentColor = Preferences.Default.Get("MainPageColor", "Unknow");
        if (currentColor == "Dark")
        {
            ButtonChangeBackground = "Light Mode";
            ButtonBackgroundColor = Color.FromRgb(255, 255, 255);
        }
        else
        {
            
            ButtonChangeBackground = "Dark Mode";
            ButtonBackgroundColor = Color.FromRgb(64, 64, 64);
        }
        IsNotConnected = true;
    }

    // Command
    [RelayCommand]
    void LogUserChoice()
    {
        string currentColor = Preferences.Default.Get("MainPageColor", "Unknow");
        
        // Enregistrer le nouveau choix dans les préférences
        if (currentColor == "Dark")
        {
            Preferences.Default.Set("MainPageColor", "Light");
            ButtonChangeBackground = "Dark Mode";
            ButtonBackgroundColor = Color.FromRgb(64,64,64);
        }
        else
        {
            Preferences.Default.Set("MainPageColor", "Dark");
            ButtonChangeBackground = "Light Mode";
            ButtonBackgroundColor = Color.FromRgb(255, 255, 255);
        }

        RefreshPage?.Invoke();
    }

    [RelayCommand]
    async Task GoToDetailsPage()
    {
        await Shell.Current.GoToAsync(nameof(DetailsPage), true);
    }
    [RelayCommand]
    async Task GoToFormPage()
    {
        await Shell.Current.GoToAsync($"FormPage?isAdmin={IsAdmin}", true);
    }
    [RelayCommand]
    async Task Login()
    {
        if (UserVM.CheckUserCredentials(Username, Password))
        {
            // Si les identifiants sont corrects, modifiez les valeurs des booléens
            IsConnected = true;
            IsNotConnected = false;

            // Si l'utilisateur est admin
            if (UserVM.MyUserList.First(user => user.UserName == Username).UserAccessType == 1)
            {
                IsAdmin = true;
            }
            else
            {
                IsAdmin = false;
            }
            RefreshUI();
        }
        else
        {
            // Si les identifiants sont incorrects, affichez une erreur
            await ShowError("Invalid username or password.");
        }
    }

    [RelayCommand]
    async Task GoToUserPage(string data)
    {
        await Shell.Current.GoToAsync(nameof(UserPage), true);
    }
    

    // Méthode pour rafraîchir l'interface utilisateur
    private void RefreshUI()
    {
        OnPropertyChanged(nameof(IsConnected));
        OnPropertyChanged(nameof(IsNotConnected));
        OnPropertyChanged(nameof(IsAdmin));
    }

    // Méthode pour afficher une erreur
    private async Task ShowError(string message)
    {
        await Shell.Current.DisplayAlert("Erreur", message, "OK");
    }
}
    



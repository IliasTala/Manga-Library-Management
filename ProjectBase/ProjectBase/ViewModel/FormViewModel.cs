using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectBase.ViewModel
{
    [QueryProperty(nameof(IsAdmin), "isAdmin")]
    public partial class FormViewModel : BaseViewModel
    {
        DeviceOrientationServices MyDeviceOrientationService;


        [ObservableProperty]
        string targetScanner;

        [ObservableProperty]
        string selectedImagePath;

        [ObservableProperty]
        MangaModel myManga;

        private bool _isAdmin;
        public string IsAdmin
        {
            set
            {
                bool isAdmin;
                bool.TryParse(value, out isAdmin);
                _isAdmin = isAdmin;
                OnPropertyChanged();
            }
            get
            {
                return _isAdmin.ToString();
            }
        }       

        // ObservableCollection pour stocker la liste des mangas à afficher
        public ObservableCollection<MangaModel> MyShownList { get; set; }

        private MangaService _mangaService;

        // La liste complète des mangas.
        private List<MangaModel> _mangas;
        public FormViewModel() 
        {
            this.MyDeviceOrientationService = new DeviceOrientationServices();
            this.MyDeviceOrientationService.ConfigureScanner();
            this.MyDeviceOrientationService.SerialBuffer.Changed += SerialBuffer_Changed;

            MyShownList = new ObservableCollection<MangaModel>();
            _mangaService = new MangaService();
            MyManga = new MangaModel();
            AddManga();
        }
        public async Task AddManga()
        {
            if (IsBusy) return;

            try
            {
                IsBusy = true;
                var mangas = await _mangaService.GetMangaFiles();

                Globals.MyMangaList = mangas.ToList();

                MyShownList.Clear();
                foreach (MangaModel man in mangas)
                {
                    MyShownList.Add(man);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Unable to get Mangas: {ex.Message}");
                await Shell.Current.DisplayAlert("Error!", ex.Message, "OK");
            }
            finally { IsBusy = false; }
        }

        // Gestionnaire d'événements pour le changement du SerialBuffer
        private void SerialBuffer_Changed(object sender, EventArgs e)
        {
            DeviceOrientationServices.QueueBuffer myQueue = (DeviceOrientationServices.QueueBuffer)sender;
            TargetScanner = myQueue.Dequeue().ToString();
        }

        [RelayCommand]
        public async Task SaveManga()
        {
            try
            {
                // Vérifie si un manga avec l'ID spécifié existe déjà
                var existingManga = Globals.MyMangaList.FirstOrDefault(m => m.Id == TargetScanner);

                if (existingManga != null)
                {
                    await Shell.Current.DisplayAlert("Error!", $"A manga with the ID {TargetScanner} already exists.", "OK");
                    return;
                }

                // Crée un nouvel objet MangaModel avec les données entrées
                MangaModel newManga = new MangaModel
                {
                    Id = TargetScanner,
                    Licence = MyManga.Licence,
                    Title = MyManga.Title,
                    Author = MyManga.Author,
                    Publisher = MyManga.Publisher,
                    Price = MyManga.Price,
                    Style = MyManga.Style,
                    Cover = MyManga.Cover
                };

                // Ajoute le nouveau manga à la liste existante
                Globals.MyMangaList.Add(newManga);

                // Ajoute le nouveau manga à la liste affichée
                MyShownList.Add(newManga);

                // Sauvegarde la liste mise à jour dans le fichier
                await _mangaService.SetMangas(Globals.MyMangaList);

                await Shell.Current.DisplayAlert("Success", "Data saved successfully", "OK");

                // Réinitialise l'objet MyManga avec un nouveau MangaModel pour vider le formulaire
                MyManga = new MangaModel();

                // Réinitialise l'ID et l'image affichée
                TargetScanner = string.Empty;
                SelectedImagePath = string.Empty;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error while adding manga to JSON file: {ex.Message}");
                await Shell.Current.DisplayAlert("Error!", ex.Message, "OK");
            }
        }

        [RelayCommand]
        public async Task<bool> DeleteManga(string id)
        {
            try
            {
                Console.WriteLine($"Attempting to delete manga with ID: {id}");

                var mangaToDelete = MyShownList.FirstOrDefault(m => m.Id == id);
                if (mangaToDelete != null)
                {
                    // Supprime le manga de la liste complète
                    Globals.MyMangaList.Remove(mangaToDelete);
                    // Supprime le manga de la liste affichée
                    MyShownList.Remove(mangaToDelete);

                    // Sauvegarde la liste mise à jour dans le fichier
                    await _mangaService.SetMangas(Globals.MyMangaList);

                    await Shell.Current.DisplayAlert("Success", "Data deleted successfully", "OK");


                    return true;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error while deleting manga from JSON file: {ex.Message}");
                await Shell.Current.DisplayAlert("Error!", ex.Message, "OK");
                return false;
            }

            return false;
        }

        [RelayCommand]
        public async Task PickPicture()
        {
            try
            {
                if (MyManga == null)
                {
                    MyManga = new MangaModel();
                }

                var result = await MediaPicker.PickPhotoAsync(new MediaPickerOptions
                {
                    Title = "Pick a photo"
                });

                if (result != null)
                {
                    SelectedImagePath = Path.GetFileName(result.FullPath);
                    MyManga.Cover = SelectedImagePath;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error while picking image: {ex.Message}");
                await Shell.Current.DisplayAlert("Error!", ex.Message, "OK");
            }
        }

    }
}

using Microsoft.Maui.Controls;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Windows.Input;
using ProjectBase.Services;
using System.Text;

namespace ProjectBase.ViewModel
{
    // ViewModel pour la page de détails
   // [QueryProperty(nameof(IsAdmin), "isAdmin")]
    public partial class DetailsViewModel : BaseViewModel
    {

        // Propriétés liées à l'interface utilisateur
       
        [ObservableProperty]
        MangaModel myManga ;

        [ObservableProperty]
        private string sortBy;

        [ObservableProperty]
        private bool sortByPrice;
        [ObservableProperty]
        private bool sortByLicence;
        [ObservableProperty]
        private bool sortByAuthor;

        [ObservableProperty]
        private bool reverse;

        [ObservableProperty]
        private bool choiceShonen;
        [ObservableProperty]
        private bool choiceSeinen;
        [ObservableProperty]
        private bool choiceShojo;

        


        // Les méthodes suivantes sont des gestionnaires d'événements pour les modifications des propriétés ci-dessus.
        // Elles sont appelées automatiquement lorsque les propriétés sont mises à jour.
        partial void OnChoiceShonenChanged(bool value)
        {          
                RefreshMangaList();
        }
        partial void OnChoiceSeinenChanged(bool value)
        {
            RefreshMangaList();
        }
        partial void OnChoiceShojoChanged(bool value)
        {
             RefreshMangaList();
        }
        partial void OnReverseChanged(bool value)
        {
            RefreshMangaList();
        }
        partial void OnSortByPriceChanged(bool value)
        {
            if (value)
            {
                SortByLicence = false;
                SortByAuthor = false;
            }
            RefreshMangaList();
        }
        partial void OnSortByLicenceChanged(bool value)
        {
            if (value)
            {
                SortByPrice = false;
                SortByAuthor = false;
            }
            RefreshMangaList();
        }
        partial void OnSortByAuthorChanged(bool value)
        {
            if (value)
            {
                SortByPrice = false;
                SortByLicence = false;
            }
            RefreshMangaList();
        }

        // ObservableCollection pour stocker la liste des mangas à afficher
        public ObservableCollection<MangaModel> MyShownList { get; set; }

        private MangaService _mangaService;


        public DetailsViewModel()
        {
            
            MyShownList = new ObservableCollection<MangaModel>();
            _mangaService = new MangaService();
            MyManga = new MangaModel();

            LoadManga();
        }


        // Commandes
        [RelayCommand]
        public async Task LoadManga()
        {
            if (IsBusy) return;

            try
            {
                IsBusy = true;
                var mangas = await _mangaService.GetMangaFiles(); // Utilisez GetMangaFiles() pour charger le fichier JSON sur le bureau

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

        [RelayCommand]
        async Task ExportTxt()
        {
            // Générer le contenu du fichier Txt à partir de la liste de mangas
            var txtContent = ConvertMangasToTxt();

            // Créez une instance de FileService pour enregistrer le fichier
            var fileService = new FileService();
            await fileService.Save(txtContent, "MangaList.txt");
        }

        [RelayCommand]
        async Task ExportCsv()
        {
            // Générer le contenu du fichier CSV à partir de la liste de mangas
            var csvContent = ConvertMangasToCsv();

            // Créez une instance de FileService pour enregistrer le fichier
            var fileService = new FileService();
            await fileService.Save(csvContent, "MangaList.csv");
        }

        // Actualise la liste des mangas en fonction des filtres et du tri
        public void RefreshMangaList()
        {
            var reverse = Reverse;
            var styles = new List<string>();

            if (ChoiceShonen) styles.Add("Shonen");
            if (ChoiceSeinen) styles.Add("Seinen");
            if (ChoiceShojo) styles.Add("Shojo");

            var filteredMangas = Globals.MyMangaList.ToList();

            if (styles != null && styles.Any())
            {
                filteredMangas = filteredMangas.Where(m => styles.Contains(m.Style)).ToList();
            }

            string sortBy = "Licence"; // Valeur par défaut

            if (SortByPrice) sortBy = "Price";
            else if (SortByLicence) sortBy = "Licence";
            else if (SortByAuthor) sortBy = "Author";

            filteredMangas = _mangaService.SortMangas(filteredMangas, sortBy, reverse);

            // Définit la liste filtrée comme source de données pour la CollectionView
            MyShownList.Clear();
            foreach (var manga in filteredMangas)
            {
                MyShownList.Add(manga);
            }
        }

        private string ConvertMangasToTxt()
        {
            StringBuilder txtContent = new StringBuilder();
            txtContent.AppendLine("Id,Licence,Title,Author,Publisher,Price,Style,Cover");

            foreach (MangaModel manga in MyShownList)
            {
                txtContent.AppendLine($"{manga.Id},{manga.Licence},{manga.Title},{manga.Author},{manga.Publisher},{manga.Price},{manga.Style},{manga.Cover}");
            }

            return txtContent.ToString();
        }
 
        private string ConvertMangasToCsv()
        {
            StringBuilder csvContent = new StringBuilder();
            csvContent.AppendLine("Id,Licence,Title,Author,Publisher,Price,Style,Cover");

            foreach (MangaModel manga in MyShownList)
            {
                csvContent.AppendLine($"{manga.Id},{manga.Licence},{manga.Title},{manga.Author},{manga.Publisher},{manga.Price},{manga.Style},{manga.Cover}");
            }

            return csvContent.ToString();
        }
    }
}

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
    // ViewModel pour la page de d�tails
   // [QueryProperty(nameof(IsAdmin), "isAdmin")]
    public partial class DetailsViewModel : BaseViewModel
    {

        // Propri�t�s li�es � l'interface utilisateur
       
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

        


        // Les m�thodes suivantes sont des gestionnaires d'�v�nements pour les modifications des propri�t�s ci-dessus.
        // Elles sont appel�es automatiquement lorsque les propri�t�s sont mises � jour.
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

        // ObservableCollection pour stocker la liste des mangas � afficher
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
            // G�n�rer le contenu du fichier Txt � partir de la liste de mangas
            var txtContent = ConvertMangasToTxt();

            // Cr�ez une instance de FileService pour enregistrer le fichier
            var fileService = new FileService();
            await fileService.Save(txtContent, "MangaList.txt");
        }

        [RelayCommand]
        async Task ExportCsv()
        {
            // G�n�rer le contenu du fichier CSV � partir de la liste de mangas
            var csvContent = ConvertMangasToCsv();

            // Cr�ez une instance de FileService pour enregistrer le fichier
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

            string sortBy = "Licence"; // Valeur par d�faut

            if (SortByPrice) sortBy = "Price";
            else if (SortByLicence) sortBy = "Licence";
            else if (SortByAuthor) sortBy = "Author";

            filteredMangas = _mangaService.SortMangas(filteredMangas, sortBy, reverse);

            // D�finit la liste filtr�e comme source de donn�es pour la CollectionView
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

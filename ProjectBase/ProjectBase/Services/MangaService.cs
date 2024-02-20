using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace ProjectBase.Services
{
    public class MangaService
    {
        private List<MangaModel> _mangas;

        public MangaService()
        {

        }
        // Méthode pour récupérer les fichiers de mangas à partir du fichier JSON
        public async Task<List<MangaModel>> GetMangaFiles()
        {
            string filePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "FolderDesktop", "mangas.json");
            using var stream = File.OpenRead(filePath); // Utilisez File.OpenRead() pour ouvrir le fichier en lecture seule
            using var reader = new StreamReader(stream);
            var contents = await reader.ReadToEndAsync();
            _mangas = JsonSerializer.Deserialize<List<MangaModel>>(contents);

            return _mangas;
        }

        // Méthode pour sauvegarder les mangas dans le fichier JSON
        public async Task SetMangas(List<MangaModel> mangas)
        {
            string filePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "FolderDesktop", "mangas.json");
            using FileStream fileStream = File.Create(filePath);

            await JsonSerializer.SerializeAsync(fileStream, mangas);
            await fileStream.DisposeAsync();
        }

        // Méthode pour trier les mangas en fonction du critère de tri et de l'ordre
        public List<MangaModel> SortMangas(List<MangaModel> mangas, string sortBy, bool reverse)
        {
            IEnumerable<MangaModel> orderedMangas;

            switch (sortBy)
            {
                case "Price":
                    if (reverse)
                    {
                        orderedMangas = mangas.OrderByDescending(m =>
                        {
                            if (decimal.TryParse(m.Price, NumberStyles.Any, CultureInfo.InvariantCulture, out decimal price))
                            {
                                return price;

                            }
                            else
                            {
                                return decimal.MinValue;
                            }
                        });
                    }
                    else
                    {
                        orderedMangas = mangas.OrderBy(m =>
                        {
                            if (decimal.TryParse(m.Price, NumberStyles.Any, CultureInfo.InvariantCulture, out decimal price))
                            {
                                return price;
                            }
                            else
                            {
                                return decimal.MaxValue;
                            }
                        });
                    }
                    break;
                case "Licence":
                    orderedMangas = reverse ? mangas.OrderByDescending(m => m.Licence) : mangas.OrderBy(m => m.Licence);
                    break;
                case "Author":
                    orderedMangas = reverse ? mangas.OrderByDescending(m => m.Author) : mangas.OrderBy(m => m.Author);
                    break;
                default:
                    return mangas;
            }

            return orderedMangas.ToList();
        }

        // Méthode pour supprimer un manga en fonction de son identifiant
        public async Task<bool> DeleteManga(string id)
        {
            try
            {
                if (_mangas == null)
                {
                    await GetMangaFiles();
                }

                var mangaToDelete = _mangas.FirstOrDefault(m => m.Id == id);
                if (mangaToDelete != null)
                {
                    _mangas.Remove(mangaToDelete);

                    // Mettre à jour le fichier JSON après la suppression du manga
                    await SetMangas(_mangas);

                    return true;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error while deleting manga from JSON file: {ex.Message}");
                throw;
            }

            return false;
        }


    }
}
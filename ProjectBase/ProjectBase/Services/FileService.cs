using CommunityToolkit.Maui.Storage;
using System.Text;

namespace ProjectBase.Services
{
    internal class FileService
    {
        // Méthode pour enregistrer le contenu dans un fichier
        internal async Task Save(string fileContent, string name)
        {
            CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
            string filePath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);

            // Conversion du contenu en tableau d'octets pour le flux
            using var myStream = new MemoryStream(Encoding.Default.GetBytes(fileContent));

            try
            {
                // Utilisation de FileSaver pour enregistrer le fichier
                var path = await FileSaver.Default.SaveAsync(filePath, name, myStream, cancellationTokenSource.Token);
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlert("FileSaver", ex.Message, "OK");
            }
            finally
            {
                cancellationTokenSource.Dispose();
            }
        }
    }
}

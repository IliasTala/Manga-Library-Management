using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectBase.ViewModel
{
    public partial class UserViewModel : BaseViewModel
    {
        // Propriété observable contenant la liste des utilisateurs
        public ObservableCollection<UserModel> MyUserList { get; set; } = new();

        // Service de gestion des utilisateurs
        public UserManagementServices MyDBService;

        // Propriétés pour les entrées utilisateur
        public string UserNameInput { get; set; }
        public string UserPasswordInput { get; set; }
        public int UserAccessTypeInput { get; set; }

        // Propriétés pour les mises à jour utilisateur
        public string UpdateUserNameInput { get; set; }
        public string UpdateUserPasswordInput { get; set; }
        public int UpdateUserAccessTypeInput { get; set; }

        // Propriété pour la suppression d'un utilisateur
        public string DeleteUserNameInput { get; set; }


        // Constructeur 
        public UserViewModel(UserManagementServices myDBService) 
        {
             this.MyDBService = myDBService;
             MyDBService.ConfigTools();
             FillFromDB();

        }

        // Méthode pour remplir la liste des utilisateurs depuis la base de données
        async void FillFromDB()
        {
            IsBusy= true;
            
            List<UserModel> MyActualList = new();

            // Vérifier si les données sont déjà présentes en mémoire
            if (Globals.userSet.Tables["Access"].Rows.Count == 0)
            {
                await MyDBService.ReadFromDB();
            }

            // Supprimer les anciennes données des utilisateurs si elles existent
            if (Globals.userSet.Tables["Users"].Rows.Count != 0)
            {
                Globals.userSet.Tables["Users"].Clear();
            }

            // Récupérer les utilisateurs depuis la base de données
            await MyDBService.FillUsersFromDB();

            try
            {
                // Convertir les données de la table "Users" en objets UserModel
                MyActualList = Globals.userSet.Tables["Users"].AsEnumerable().Select(m => new UserModel()
                {
                    User_ID = m.Field<Int16>("User_ID"),
                    UserName = m.Field<string>("UserName"),
                    UserPassword = m.Field<string>("UserPassword"),
                    UserAccessType = m.Field<Int16>("UserAccessType"),

                }).ToList();
            }
            catch(Exception ex)
            {
                await Shell.Current.DisplayAlert("Database", ex.Message, "OK");
            }

            // Vider la liste des utilisateurs et ajouter les nouveaux utilisateurs
            MyUserList.Clear();
            foreach(var item in MyActualList)
            {
                MyUserList.Add(item);
            }

            
            IsBusy = false;
        }

        // Méthode pour insérer un nouvel utilisateur dans la base de données
        [RelayCommand]
        async Task InsertUser() 
        {
            IsBusy= true;
            try
            {
                await MyDBService.InsertIntoDB(UserNameInput, UserPasswordInput, UserAccessTypeInput);
                // Effacez les valeurs entrées par l'utilisateur après l'opération réussie
                UserNameInput = string.Empty;
                UserPasswordInput = string.Empty;
                UserAccessTypeInput = 0;
            }
            catch(Exception ex)
            {
                await Shell.Current.DisplayAlert("Database", ex.Message, "OK");
            }
           
            IsBusy= false;
            FillFromDB();
        }

        // Méthode pour mettre à jour un utilisateur dans la base de données
        [RelayCommand]
        async Task UpdateUser()
        {
            IsBusy = true;

            try
            {
                // Récupérer les valeurs des entrées dans les formulaires
                string username = UpdateUserNameInput;
                string newPassword = UpdateUserPasswordInput;
                int newAccessType = UpdateUserAccessTypeInput;

                // Appeler la nouvelle méthode d'update avec les valeurs entrées
                await MyDBService.UpdateDB(username, newPassword, newAccessType);
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlert("Database", ex.Message, "OK");
            }

            IsBusy = false;
            FillFromDB();
        }

        // Méthode pour supprimer un utilisateur de la base de données
        [RelayCommand]
        async Task DeleteUser() 
        {
            IsBusy = true;
            try
            {
                // Récupérer la valeur de l'entrée dans le formulaire
                string username = DeleteUserNameInput;

                // Appeler la méthode de delete avec la valeur entrée
                await MyDBService.DeleteIntoDB(username);
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlert("Database", ex.Message, "OK");
            }

            IsBusy = false;
            FillFromDB();
        }

        // Méthode pour vérifier les informations d'identification de l'utilisateur
        public bool CheckUserCredentials(string username, string password)
        {
            return MyUserList.Any(user => user.UserName == username && user.UserPassword == password);
        }



    }
}

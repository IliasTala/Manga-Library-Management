using System;
using System.Collections.Generic;
using System.Data.OleDb;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectBase.Services
{
    public partial class UserManagementServices
    {
        OleDbConnection Connexion = new();
        OleDbDataAdapter UserAdapter = new();

        // Configuration des outils de gestion de la base de données
        internal void ConfigTools()
        {
            // QualityServer ("QualityServer",) est le repertoire sur le bureau : qu'on mets juste avant :   "UserAccounts.accdb")
            // Connexion à la base de données en utilisant Microsoft Access 
            Connexion.ConnectionString = "Provider=Microsoft.ACE.OLEDB.16.0;" + @"Data Source=" +
                                        Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "FolderDesktop", "UserAccounts.accdb")
                                        + ";Persist Security Info=False";

            // 4 Commandes SQL pour la gestion de la base de données
            string DeleteCommandTxt = " DELETE FROM DB_Users WHERE UserName=@UserName";
            string UpdateCommandTxt = " UPDATE DB_Users SET UserPassword = @UserPassword, UserAccessType=@UserAccessType WHERE UserName=@UserName";
            string InsertCommandTxt = " INSERT INTO DB_Users(UserName, UserPassword, UserAccessType) VALUES (@UserName, @UserPassword, @UserAccessType)";
            string SelectCommandTxt = " SELECT * FROM DB_Users ORDER BY User_ID";

            OleDbCommand DeleteCommand = new OleDbCommand(DeleteCommandTxt, Connexion);
            OleDbCommand UpdateCommand = new OleDbCommand(UpdateCommandTxt, Connexion);
            OleDbCommand InsertCommand = new OleDbCommand(InsertCommandTxt, Connexion);
            OleDbCommand SelectCommand = new OleDbCommand(SelectCommandTxt, Connexion);

            // Assignation des commandes aux adapteurs
            UserAdapter.DeleteCommand = DeleteCommand; 
            UserAdapter.UpdateCommand = UpdateCommand;
            UserAdapter.InsertCommand = InsertCommand;
            UserAdapter.SelectCommand = SelectCommand;

            // Définition des paramètres pour les commandes d'insertion, de suppression et de mise à jour
            UserAdapter.InsertCommand.Parameters.Add("@UserName", OleDbType.VarChar, 40, "UserName");
            UserAdapter.InsertCommand.Parameters.Add("@UserPassword", OleDbType.VarChar, 40, "UserPassword");
            UserAdapter.InsertCommand.Parameters.Add("@UserAccessType", OleDbType.Numeric, 100, "UserAccessType");
            UserAdapter.DeleteCommand.Parameters.Add("@UserName", OleDbType.VarChar, 40, "UserName");
            UserAdapter.UpdateCommand.Parameters.Add("@UserName", OleDbType.VarChar, 40, "UserName");
            UserAdapter.UpdateCommand.Parameters.Add("@UserPassword", OleDbType.VarChar, 40, "UserPassword");
            UserAdapter.UpdateCommand.Parameters.Add("@UserAccessType", OleDbType.Numeric, 100, "UserAccessType");

        }

        // Remplissage des utilisateurs à partir de la base de données
        internal async Task FillUsersFromDB()
        {
            try
            {
                Connexion.Open();

                UserAdapter.Fill(Globals.userSet.Tables["Users"]);
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlert("Database", ex.Message, "OK");
            }
            finally
            {
                Connexion.Close();
            }
        }

        // Lecture des données à partir de la base de données
        internal async Task ReadFromDB()
        {
            OleDbCommand SelectCommand = new OleDbCommand("SELECT * FROM DB_Access;", Connexion);
            try
            {
                Connexion.Open();

                OleDbDataReader reader = SelectCommand.ExecuteReader();
                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        Globals.userSet.Tables["Access"].Rows.Add(reader[0], reader[1], reader[2], reader[3], reader[4], reader[5]);
                    }
                }
                reader.Close();
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlert("Database", ex.Message, "OK");
            }
            finally
            {
                Connexion.Close();
            }
        }

        // Insertion d'un nouvel utilisateur dans la base de données
        internal async Task InsertIntoDB(string name, string pass, Int32 access)
        {
            try
            {
                Connexion.Open();
                UserAdapter.InsertCommand.Parameters[0].Value = name;
                UserAdapter.InsertCommand.Parameters[1].Value = pass;
                UserAdapter.InsertCommand.Parameters[2].Value = access;

                int buffer = UserAdapter.InsertCommand.ExecuteNonQuery();  

                if (buffer != 0) { await Shell.Current.DisplayAlert("Database", "User successfully inserted", "OK"); }
                else await Shell.Current.DisplayAlert("Database", "User not inserted", "OK");
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlert("Database", ex.Message, "OK");
            }
            finally
            {
                Connexion.Close();
            }
        }

        // Suppression d'un utilisateur de la base de données
        internal async Task DeleteIntoDB(string name)
        {
            try
            {
                Connexion.Open();
                UserAdapter.DeleteCommand.Parameters[0].Value = name;

                int buffer = UserAdapter.DeleteCommand.ExecuteNonQuery();

                if (buffer != 0) { await Shell.Current.DisplayAlert("Database", "User successfully deleted", "OK"); }
                else await Shell.Current.DisplayAlert("Database", "User not deleted", "OK");
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlert("Database", ex.Message, "OK");
            }
            finally
            {
                Connexion.Close();
            }
        }

        // Mise à jour des données d'un utilisateur dans la base de données
        internal async Task UpdateDB(string userName, string newPassword, int newAccessType)
        {
            try
            {
                Connexion.Open();

                // Création d'une commande de mise à jour avec les paramètres correspondants
                OleDbCommand updateCommand = new OleDbCommand("UPDATE DB_Users SET UserPassword = @UserPassword, UserAccessType = @UserAccessType WHERE UserName = @UserName", Connexion);
                updateCommand.Parameters.AddWithValue("@UserPassword", newPassword);
                updateCommand.Parameters.AddWithValue("@UserAccessType", newAccessType);
                updateCommand.Parameters.AddWithValue("@UserName", userName);

                int buffer = updateCommand.ExecuteNonQuery();

                if (buffer != 0)
                {
                    await Shell.Current.DisplayAlert("Database", "User successfully updated", "OK");
                }
                else
                {
                    await Shell.Current.DisplayAlert("Database", "User not updated", "OK");
                }
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlert("Database", ex.Message, "OK");
            }
            finally
            {
                Connexion.Close();
            }
        }



    }
}

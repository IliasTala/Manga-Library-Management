
namespace ProjectBase.Services
{
    public partial class UserManagementServices
    {
    }

    //Definir un dataset (dataset defini dans globlas)  avec des contraintes
    // Classe pour créer les tables utilisateur
    public class CreateUserTables{
        public CreateUserTables()
        {
            // Création des DataTables
            DataTable UserTable = new();
            DataTable AccessTable = new();

            // Définition des colonnes de la table Users
            DataColumn User_ID = new DataColumn("User_ID", System.Type.GetType("System.Int16"));
            DataColumn UserName = new DataColumn("UserName", System.Type.GetType("System.String"));
            DataColumn UserPassword = new DataColumn("UserPassword", System.Type.GetType("System.String"));
            DataColumn AccessType = new DataColumn("UserAccessType", System.Type.GetType("System.Int16"));

            // Définition des colonnes de la table Access
            DataColumn Access_ID = new DataColumn("Access_ID", System.Type.GetType("System.Int16"));
            DataColumn AccessName = new DataColumn("AccessName", System.Type.GetType("System.String"));
            DataColumn CreateObject = new DataColumn("CreateObject", System.Type.GetType("System.Boolean"));
            DataColumn DestroyObject = new DataColumn("DestroyObject", System.Type.GetType("System.Boolean"));
            DataColumn ModifyObject = new DataColumn("ModifyObject", System.Type.GetType("System.Boolean"));
            DataColumn ChangeUserRights = new DataColumn("ChangeUserRights", System.Type.GetType("System.Boolean"));

            // Configuration de la table Users
            UserTable.TableName = "users";

            User_ID.AutoIncrement = true;
            User_ID.Unique = true;
            UserTable.Columns.Add(User_ID);

            UserName.Unique = true;
            UserTable.Columns.Add(UserName);

            UserTable.Columns.Add(UserPassword);
            UserTable.Columns.Add(AccessType);

            // Configuration de la table Access
            AccessTable.TableName = "Access";

            Access_ID.AutoIncrement = true;
            Access_ID.Unique = true;
            AccessTable.Columns.Add(Access_ID);

            AccessName.Unique = true;
            AccessTable.Columns.Add(AccessName);

            AccessTable.Columns.Add(CreateObject);
            AccessTable.Columns.Add(DestroyObject);
            AccessTable.Columns.Add(ModifyObject);
            AccessTable.Columns.Add(ChangeUserRights);

            // Ajout des tables au DataSet dans Globals
            Globals.userSet.Tables.Add(AccessTable);
            Globals.userSet.Tables.Add(UserTable);

            // Création de la relation entre les tables Users et Access
            DataColumn parentColumn = Globals.userSet.Tables["Access"].Columns["Access_ID"];
            DataColumn childColumn = Globals.userSet.Tables["Users"].Columns["UserAccessType"];

            DataRelation relation = new DataRelation("Access2User", parentColumn, childColumn);

            Globals.userSet.Tables["Users"].ParentRelations.Add(relation);
        }
    }
}

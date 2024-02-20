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
        internal void ConfigTools() { }
        internal async Task FillUsersFromDB() { }
        internal async Task ReadFromDB() { }
        internal async Task InsertIntoDB(string name, string pass, Int32 access) { }
        internal async Task DeleteIntoDB(string name) { }
        internal async Task UpdateDB(string name) { }
        internal async Task UpdateDB(string userName, string newPassword, int newAccessType) { }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectBase.Model
{
    public class UserModel
    {
        public Int16 User_ID { get; set; }
        public string UserName { get; set; }
        public string UserPassword { get; set; }
        public Int16 UserAccessType { get; set; }
    }
}

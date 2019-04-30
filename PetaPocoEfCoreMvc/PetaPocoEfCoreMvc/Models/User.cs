using PetaPoco;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PetaPocoEfCoreMvc.Models
{
    [TableName("Users")]
    public class User
    {

        public ulong Id { get; set; }
        public Guid Uid { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
    }
}

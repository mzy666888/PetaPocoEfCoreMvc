using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PetaPocoEfCoreMvc.Service
{
    using PetaPoco;
    using PetaPocoEfCoreMvc.Models;

    public class UserService:IUserService
    {
        private readonly IDatabase _database;

        public UserService(IDatabase database)
        {
            _database = database;
        }

        public IEnumerable<User> GetAll()
        {
            return _database.Fetch<User>();
        }
    }
}

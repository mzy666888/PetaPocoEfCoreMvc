using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PetaPocoEfCoreMvc.Service
{
    using PetaPocoEfCoreMvc.Models;

    public interface IUserService
    {
        IEnumerable<User> GetAll();
    }
}

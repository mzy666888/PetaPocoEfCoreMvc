using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PetaPocoEfCoreMvc.Profiles
{
    using AutoMapper;

    using PetaPocoEfCoreMvc.Models;
    using PetaPocoEfCoreMvc.Profiles.DTOs;

    public interface IProfile
    {

    }

    public class PetaPocoEfCoreProfile:Profile,IProfile
    {
        public PetaPocoEfCoreProfile()
        {
            CreateMap<User, UserDto>().ReverseMap();
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PetaPocoEfCoreMvc.DBContext
{
    using JetBrains.Annotations;
    using Microsoft.EntityFrameworkCore;

    using PetaPocoEfCoreMvc.Models;

    public class EfCoreDBContext : DbContext
    {
        public EfCoreDBContext(DbContextOptions<EfCoreDBContext> options) : base(options)
        {
        }
        public DbSet<User> Users { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<Customer> Customers { get; set; }
    }
}

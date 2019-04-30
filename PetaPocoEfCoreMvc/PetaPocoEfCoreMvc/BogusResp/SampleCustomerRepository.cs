using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PetaPocoEfCoreMvc.BogusResp
{
    using Models;
    using Bogus;

    public interface ISampleCustomerRepository
    {
        IEnumerable<Customer> GetCustomers();

        IEnumerable<User> GetUsers();
    }
    public class SampleCustomerRepository : ISampleCustomerRepository
    {
        public IEnumerable<Customer> GetCustomers()
        {
            Randomizer.Seed = new Random(123456);
            var ordergenerator = new Faker<Order>("zh_CN")
                .RuleFor(o => o.Id, f => f.Random.ULong(0, 1000))
                .RuleFor(o => o.Date, f => f.Date.Past(3))
                .RuleFor(o => o.OrderValue, f => f.Finance.Amount(0, 10000))
                .RuleFor(o => o.Shipped, f => f.Random.Bool(0.9f));
            var customerGenerator = new Faker<Customer>()
                .RuleFor(c => c.Id, f => f.Random.ULong(0, 1000))
                .RuleFor(c => c.Name, f => f.Company.CompanyName())
                .RuleFor(c => c.Address, f => f.Address.FullAddress())
                .RuleFor(c => c.City, f => f.Address.City())
                .RuleFor(c => c.Country, f => f.Address.Country())
                .RuleFor(c => c.ZipCode, f => f.Address.ZipCode())
                .RuleFor(c => c.Phone, f => f.Phone.PhoneNumber())
                .RuleFor(c => c.Email, f => f.Internet.Email())
                .RuleFor(c => c.ContactName, (f, c) => f.Name.FullName())
                .RuleFor(c => c.Orders, f => ordergenerator.Generate(f.Random.Number(10)).ToList());
            return customerGenerator.Generate(100);
        }

        public IEnumerable<User> GetUsers()
        {

            Randomizer.Seed = new Random(23456789);
            var userGenerator = new Faker<User>().RuleFor(u => u.Id, f => f.Random.ULong())
                .RuleFor(u => u.Uid, f => f.Random.Guid()).RuleFor(u => u.UserName, f => f.Name.FullName())
                .RuleFor(u => u.Password, f => f.Internet.Password(16));
            return userGenerator.Generate(20);
        }
    }
}

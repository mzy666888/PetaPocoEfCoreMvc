# PetaPocoEfCoreMvc
Us PetaPoco and Entity Framework Core 进行开发的一个样例项目

# [PetaPoco](https://github.com/CollaboratingPlatypus/PetaPoco) and Entity Framework Core
数据增删改查使用PetaPoco进行操作。可惜的是PetaPoco不支持code first，因此项目中使用Entity Framework Core进行code first的处理工作。
根据命名规范，在EFCore的DBContext中对实体的定义使用的复数形式，但是在定义实体的时候使用的是单数。PetaPoco操作时，数据库表名也为复数形式，因此需要在定义实体的时候加上TableName Attribute。
nuget引用：
MySql.Data
[Pomelo.EntityFrameworkCore.MySql](https://github.com/PomeloFoundation/Pomelo.EntityFrameworkCore.MySql)
EFCore DBContext添加：
```
namespace PetaPocoEfCoreMvc.DBContext
{
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

```
PetaPocoDBContext添加：构造函数是代码【Resharper】生成，其实用到的是最后一个构造函数
```
namespace PetaPocoEfCoreMvc.DBContext
{
    public class PetaPocoMvcDBContext:Database
    {
        public PetaPocoMvcDBContext(DbConnection connection, IMapper defaultMapper = null)
            : base(connection, defaultMapper)
        {
        }

        public PetaPocoMvcDBContext(string connectionString, string providerName, IMapper defaultMapper = null)
            : base(connectionString, providerName, defaultMapper)
        {
        }

        public PetaPocoMvcDBContext(string connectionString, DbProviderFactory factory, IMapper defaultMapper = null)
            : base(connectionString, factory, defaultMapper)
        {
        }

        public PetaPocoMvcDBContext(string connectionString, IProvider provider, IMapper defaultMapper = null)
            : base(connectionString, provider, defaultMapper)
        {
        }

        public PetaPocoMvcDBContext(IDatabaseBuildConfiguration configuration)
            : base(configuration)
        {
        }
    }
}

```
数据库连接字符串
```
"ConnectionStrings": {
    "MySQL": {
      "MvcMySQL": "server=127.0.0.1;port=3306;uid=root;pwd=123456;database=PetaPocoEfCoreMvc;",
      "provider": "MySql.Data.MySqlClient"
    }
  }
```
在Startup的ConfigureServices中添加如下代码

```
services.AddDbContext<EfCoreDBContext>(
                options =>
                    {
                        var connectionString = Configuration["ConnectionStrings:MySQL:MvcMySQL"];
                        options.UseMySql(connectionString,
                            myopttion => { myopttion.ServerVersion(new Version(10,2), ServerType.MariaDb); });
                        
                    });
                    
services.AddScoped<IDatabase, PetaPocoMvcDBContext>(
                (x) =>
                    {
                        var connectionStrnig = Configuration["ConnectionStrings:MySQL:MvcMySQL"];

                        var configuration = DatabaseConfiguration.Build().UsingConnectionString(connectionStrnig)
                            .UsingProvider<MariaDbDatabaseProvider>();
                        return new PetaPocoMvcDBContext(configuration);
                    });
```


```
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

```
# [Bogus](https://github.com/bchavez/Bogus)使用
定义接口是为了方便DI使用

 ```
 namespace PetaPocoEfCoreMvc.BogusResp
{
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
 ```
 
 # [AutoMapper](https://github.com/AutoMapper/AutoMapper) [doc](http://docs.automapper.org/en/stable/search.html?q=Assembly&check_keywords=yes&area=default)
 
 [AutoMapper.Extensions.Microsoft.DependencyInjection](https://github.com/AutoMapper/AutoMapper.Extensions.Microsoft.DependencyInjection)
 在Startup的ConfigureServices中添加如下代码
 `services.AddAutoMapper(cfg => { },new List<Assembly>(){ Assembly.GetEntryAssembly() });`
 
 定义继承Profile的类
 ```
 namespace PetaPocoEfCoreMvc.Profiles
{
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

 ```

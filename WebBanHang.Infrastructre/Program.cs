using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Configuration;
using WebBanHang.Domain.Entities;
using WebBanHang.Infrastructre.Models;
using WebBanHang.Infrastructre.User_Admin;

/*IHost host = Host.CreateDefaultBuilder(args)
.ConfigureAppConfiguration((config) =>
{
    config.AddEnvironmentVariables();
    config.Build();
}).ConfigureServices(services =>
{
    var conString = Environment.GetEnvironmentVariable("MYSQLCNNSTR_cnnKey");
    services.AddDbContext<AppDbContext>(option => option.UseMySQL(conString!));
}).Build();*/

// Tao thu 1 customer
/*Customer test = new Customer()
{
    FirstName = "Vu",
    LastName = "Duy Linh",
    Email = "linhvu@gmail.com",
    PhoneNo = "0986555467",
    CreateAt = DateTime.Now,
};*/

User test_user = new User()
{
    UserName = "linhvu",
    Email = "linhvu@gmail.com",
    Password = "Abc56789",
    Status = "Active",
    RoleID = 3,
    CreateAt = DateTime.Now,
};

string connection = Environment.GetEnvironmentVariable("MYSQLCNNSTR_cnnKey")!;
// await new UserInfor(new AppDbContext(connection)).Add(test_user);
User backRes = await new UserInfor(new AppDbContext(connection)).GetById(1);
Console.WriteLine($"{backRes.UserName} - {backRes.CreateAt}");

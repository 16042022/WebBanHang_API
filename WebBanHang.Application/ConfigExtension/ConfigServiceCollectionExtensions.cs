using AutoMapper;
using Microsoft.EntityFrameworkCore;
using System.Text.Json.Serialization;
using System.Xml.Serialization;
using WebBanHang.Domain;
using WebBanHang.Domain.Common;
using WebBanHang.Domain.Entities;
using WebBanHang.Domain.Services;
using WebBanHang.Domain.UseCase.Others;
using WebBanHang.Domain.UseCase.Users_Admin;
using WebBanHang.Infrastructre.Models;
using WebBanHang.Infrastructre.Products;
using WebBanHang.Infrastructre.Security;
using WebBanHang.Infrastructre.User_Admin;


namespace WebBanHang.Application.ConfigExtension
{
    public static class ConfigServiceCollectionExtensions
    {
        public static IServiceCollection ConfigDependencyGroup(this IServiceCollection services)
        {
            var key = Environment.GetEnvironmentVariable("MYSQLCNNSTR_cnnKey");
            services.AddDbContext<AppDbContext>(opt => opt.UseMySQL(key!));
            services.AddDistributedMemoryCache();
            services.AddSession(x =>
            {
                x.IdleTimeout = TimeSpan.FromMinutes(30);
            });
            services.AddControllers().AddJsonOptions(x =>
            {
                // serialize enums as strings in api responses (e.g. Role)
                x.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
            });
            // Anti fogtegy attack config
            // Add auto mapper config
            var mapperConfig = new MapperConfiguration(mc =>
            {
                mc.AddProfile(new MappingConfiguration());
            });

            IMapper mapper = mapperConfig.CreateMapper();
            services.AddSingleton(mapper);
            // Other config
            services.AddScoped<IRepository<Users>, UserInfor>();
            services.AddScoped(typeof(IRepository<>), typeof(TransactionRepository<>));
            services.AddScoped<IUserInfor, UserManagement>();
            services.AddTransient<IAuthenication, AuthenicationProvider>();
            services.AddTransient<IEmailSender, EmailSender>();
            services.AddScoped<IUserSerrvice, UserService>();
            return services;
        }
    }
}

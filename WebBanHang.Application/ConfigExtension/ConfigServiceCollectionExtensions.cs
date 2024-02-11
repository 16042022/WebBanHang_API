using AutoMapper;
using System.Text.Json.Serialization;
using System.Xml.Serialization;
using WebBanHang.Domain;
using WebBanHang.Domain.Common;
using WebBanHang.Domain.Entities;
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
            services.AddDbContext<AppDbContext>();
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
            services.AddScoped<IRepository<Customer>, CustomerInfor>();
            services.AddScoped<IRepository<User>, UserInfor>();
            services.AddScoped(typeof(IRepository<>), typeof(TransactionRepository<>));
            services.AddScoped<IUserInfor, UserManagement>();
            services.AddTransient<IAuthenication, AuthenicationProvider>();
            services.AddSingleton<IEmailSender, EmailSender>();
            
            return services;
        }
    }
}

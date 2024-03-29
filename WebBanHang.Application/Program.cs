using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;
using WebBanHang.Application.ConfigExtension;
using WebBanHang.Domain;
using WebBanHang.Domain.Common;
using WebBanHang.Domain.Entities;
using WebBanHang.Domain.UseCase.Users_Admin;
using WebBanHang.Infrastructre.Security;

namespace WebBanHang.Application
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            // Add services to the container.
            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.ConfigSwaggerGroup();
            // builder.Services.AddSwaggerGen();

            builder.Services.ConfigDependencyGroup();

            builder.Services.AddOptions();
            var JwtConfig = builder.Configuration.GetSection("JWTConfig");
            builder.Services.Configure<JsonConfig>(JwtConfig);

            var MailPort = builder.Configuration.GetSection("MailConfig");
            builder.Services.Configure<MailPortSetting>(MailPort);

            var app = builder.Build();
            app.UseSession();
            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.AddConfigSwagger();
            }

            app.UseHttpsRedirection();
            // Config other security feature ...
            // Add an endpoint so that accept the refresh token feature...
            app.DetectExceptionMiddleware();
            app.UseJWTMiddleware();

            app.MapControllers();

            app.Run();
        }
    }
}
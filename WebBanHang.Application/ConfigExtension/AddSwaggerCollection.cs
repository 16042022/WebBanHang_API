using Google.Protobuf.WellKnownTypes;
using Microsoft.OpenApi.Models;

namespace WebBanHang.Application.ConfigExtension
{
    public static class AddSwaggerCollection
    {
        public static IServiceCollection ConfigSwaggerGroup(this IServiceCollection services)
        {
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "WebBanHangAPI", Version = "v1" });
                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme()
                {
                    Description = $"@\"JWT Authorization header using the Bearer scheme. \r\n\r\n\r\n                      " +
                    "Enter 'Bearer' [space] and then your token in the text input below.\r\n                      " +
                    "\r\n\r\nExample: 'Bearer 12345abcdef'\",",
                    Name = "Authorization",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = "Bearer",
                    BearerFormat = "JWT"
                });
                c.AddSecurityRequirement(new OpenApiSecurityRequirement()
                  {
                    {
                      new OpenApiSecurityScheme
                      {
                        Reference = new OpenApiReference
                          {
                            Type = ReferenceType.SecurityScheme,
                            Id = "Bearer"
                          },
                          Scheme = "oauth2",
                          Name = "Bearer",
                          In = ParameterLocation.Header,
                        },
                        new List<string>()
                      }
                    });
            });
            return services;
        }

        public static IApplicationBuilder AddConfigSwagger(this IApplicationBuilder builder)
        {
            builder.UseSwagger(); // More config here...
            builder.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "WebBanHangAPI v1");
            });
            // app.UseSwaggerUI();
            builder.UseDeveloperExceptionPage();
            return builder;
        }
    }
}

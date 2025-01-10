
using api_gateway.services;
using api_gateway.modules;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using api_gateway.models.GameDTOs;
using api_gateway.models.PaymentDTOs;
using api_gateway.models;
using Microsoft.AspNetCore.Mvc;

namespace api_gateway
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var allowFrontend = "_allowFrontend";
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddControllers();

			builder.Services.AddScoped<ILogService, LogService>();
			builder.Services.AddScoped<IPaymentService, PaymentService>();
            builder.Services.AddScoped<IAccountService, AccountService>();
            builder.Services.AddLogging(con =>
            {
                con.AddConsole();
                con.AddDebug();
            });
            
            builder.Services.AddScoped<IGatewayService, GatewayService>();
            builder.Services.RegisterServices(builder.Configuration);
			builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();
            builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(
                opt =>
                {
                    opt.TokenValidationParameters = new TokenValidationParameters
                    {
                        ClockSkew = TimeSpan.Zero,
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        ValidIssuer = builder.Configuration["Jwt:Issuer"],
                        ValidAudience = builder.Configuration["Jwt:Audience"],
                        IssuerSigningKey = new SymmetricSecurityKey(
                            Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"])
                            )
                    };
                });
            builder.Services.AddCors(options =>
            {
                options.AddPolicy(name: allowFrontend,
                                  policy =>
                                  {
                                      policy.WithOrigins("http://localhost:3000","http://localhost:8085", "http://frontend")
                                      .AllowAnyHeader()
                                      .AllowAnyMethod();
                                  });
            });


            var app = builder.Build();

            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseCors(allowFrontend);

            app.UseHttpsRedirection();
            app.UseAuthentication();
            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}

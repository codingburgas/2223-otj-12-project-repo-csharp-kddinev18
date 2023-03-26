using BridgeAPI.BLL;
using BridgeAPI.BLL.Interfaces;
using BridgeAPI.BLL.Services;
using BridgeAPI.BLL.Services.Interfaces;
using BridgeAPI.Controllers;
using BridgeAPI.DAL.Data;
using BridgeAPI.DAL.Repositories;
using BridgeAPI.DAL.Repositories.Interfaces;
using Microsoft.AspNetCore.Mvc.ApplicationParts;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddScoped<IBridgeAPIDbContext, BridgeAPIDbContext>();
builder.Services.AddScoped<ITokenRepository, TokenRepository>();
builder.Services.AddScoped<IUserRepository, UserRepository>();

builder.Services.AddScoped<IAuthenticationService, AuthenticationService>();
builder.Services.AddScoped<IEncryptionService, EncryptionService>();
builder.Services.AddScoped<ITokenService, TokenService>();
builder.Services.AddScoped<IResponseFormatterService, ResponseFormatterService>();


builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();

using BridgeAPI.BLL;
using BridgeAPI.BLL.Interfaces;
using BridgeAPI.BLL.Services;
using BridgeAPI.BLL.Services.Interfaces;
using BridgeAPI.DAL.Data;
using BridgeAPI.DAL.Repositories;
using BridgeAPI.DAL.Repositories.Interfaces;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddTransient<IBridgeAPIDbContext, BridgeAPIDbContext>();
builder.Services.AddTransient<ITokenRepository, TokenRepository>();
builder.Services.AddTransient<IUserRepository, UserRepository>();

builder.Services.AddTransient<IAuthenticationService, AuthenticationService>();
builder.Services.AddTransient<IEncryptionService, EncryptionService>();
builder.Services.AddTransient<ITokenService, TokenService>();
builder.Services.AddTransient<IResponseFormatterService, ResponseFormatterService>();

//builder.Services.AddTransient<IServer, Server>();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

var scope = app.Services.CreateScope();
var services = scope.ServiceProvider;
var controller = new Server(services);
controller.ServerSetUp();


app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();


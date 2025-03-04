using System.Security.Cryptography;
using System.Text;
using Microsoft.EntityFrameworkCore;
using MinhaApiComMySQL.Data;
using Models;
using WebApplication1.Models;
using WebApplication1.service;
using WebApplication1.Service;  // Corrigido para garantir que o namespace correto seja usado

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Configuração do DbContext com MySQL
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseMySql(builder.Configuration.GetConnectionString("DefaultConnection"),
    ServerVersion.AutoDetect(builder.Configuration.GetConnectionString("DefaultConnection"))));

// Gerando e registrando a chave secreta de configuração para JWT
var secretKey = GenerateSecretKey();
builder.Services.AddSingleton(secretKey);

// Registrando o AuthService para injeção de dependência
builder.Services.AddScoped<AuthService>();

var app = builder.Build();

// Rotas CRUD para usuários

// Criar usuário
app.MapPost("/users", async (User userInput, AppDbContext dbContext) =>
{
    var passwordHash = GeneratePasswordHash(userInput.PasswordHash);
    var newUser = new User
    {
        Username = userInput.Username,
        PasswordHash = passwordHash,
        Email = userInput.Email
    };

    var userServices = new UserServices();
    userServices.Create(newUser, dbContext);

    return Results.Created($"/users/{newUser.Id}", newUser);
});

// Obter todos os usuários
app.MapGet("/users", async (AppDbContext dbContext) =>
{
    var users = await dbContext.Users.ToListAsync();
    return Results.Ok(users);
});

// Obter usuário por ID
app.MapGet("/users/{id}", async (int id, AppDbContext dbContext) =>
{
    var user = await dbContext.Users.FindAsync(id);

    if (user == null)
    {
        return Results.NotFound();
    }

    return Results.Ok(user);
});

// Atualizar usuário
app.MapPut("/users/{id}", async (int id, User userInput, AppDbContext dbContext) =>
{
    var user = await dbContext.Users.FindAsync(id);

    if (user == null)
    {
        return Results.NotFound();
    }

    user.Username = userInput.Username;

    if (!string.IsNullOrEmpty(userInput.PasswordHash))
    {
        user.PasswordHash = GeneratePasswordHash(userInput.PasswordHash);
    }

    await dbContext.SaveChangesAsync();

    return Results.Ok(user);
});

// Deletar usuário
app.MapDelete("/users/{id}", async (int id, AppDbContext dbContext) =>
{
    var user = await dbContext.Users.FindAsync(id);

    if (user == null)
    {
        return Results.NotFound();
    }

    dbContext.Users.Remove(user);
    await dbContext.SaveChangesAsync();

    return Results.NoContent(); // 204
});

// Endpoint de login
app.MapPost("/login", async (LoginModel loginModel, AuthService authService) =>
{
    var user = authService.ValidateLogin(loginModel.Email, loginModel.Password);

    if (user == null)
    {
        return Results.Unauthorized();
    }

    // Gerar o token JWT
    var token = authService.GenerateJwtToken(user);
    return Results.Ok(token);
});

static string GenerateSecretKey()
{
    using (var rng = new RNGCryptoServiceProvider())
    {
        byte[] keyBytes = new byte[32]; // 32 bytes = 256 bits
        rng.GetBytes(keyBytes);
        return Convert.ToBase64String(keyBytes); // Retorna a chave como uma string Base64
    }
}

string GeneratePasswordHash(string password)
{
    using (var sha256 = SHA256.Create())
    {
        byte[] hashBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
        return Convert.ToBase64String(hashBytes);
    }
}

app.UseSwagger();
app.UseSwaggerUI();

app.Run();

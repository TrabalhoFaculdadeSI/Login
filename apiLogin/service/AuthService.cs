using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using MinhaApiComMySQL.Data;
using WebApplication1.Models;
using Models;
using System.Security.Cryptography;

namespace WebApplication1.Service
{
    public class AuthService
    {
        private readonly AppDbContext _dbContext;
        private readonly string _secretKey;

        public AuthService(AppDbContext dbContext, string secretKey)
        {
            _dbContext = dbContext;
            _secretKey = secretKey;
        }

        // Função para validar o login
        public User ValidateLogin(string email, string password)
        {
            var user = _dbContext.Users.FirstOrDefault(u => u.Email == email);

            if (user == null || !VerifyPasswordHash(password, user.PasswordHash))
            {
                return null; // Se as credenciais estiverem erradas
            }

            return user; // Se as credenciais estiverem corretas
        }

        // Função para gerar o token JWT
        public string GenerateJwtToken(User user)
    {
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_secretKey)); // Certifique-se de que _secretKey tenha 256 bits

        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        var claims = new[]
        {
            new Claim("name", user.Username),
            new Claim("email", user.Email),
            // Adicione outros claims conforme necessário
        };

        var token = new JwtSecurityToken(
            issuer: "LoginService",
            audience: "greentech",
            claims: claims,
            expires: DateTime.Now.AddHours(1),
            signingCredentials: credentials
        );

        var jwtHandler = new JwtSecurityTokenHandler();
        return jwtHandler.WriteToken(token); // Retorna o JWT assinado
    }

        // Função para verificar a senha
        private bool VerifyPasswordHash(string password, string passwordHash)
        {
            var hashToVerify = GeneratePasswordHash(password);
            return hashToVerify == passwordHash;
        }

        // Função para gerar o hash da senha com SHA256
        private string GeneratePasswordHash(string password)
        {
            using (var sha256 = SHA256.Create())
            {
                byte[] hashBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
                return Convert.ToBase64String(hashBytes);
            }
        }

    }
}
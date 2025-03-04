using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MinhaApiComMySQL.Data;
using Models;
using WebApplication1.repository;

namespace WebApplication1.service
{
    public class UserServices
    {
        public void Create(User user, AppDbContext dbContext)
        {
            if (user == null)
                throw new ArgumentNullException(nameof(user), "O usuário não pode ser nulo.");

            if (dbContext == null)
                throw new ArgumentNullException(nameof(dbContext), "O DbContext não pode ser nulo.");

            // Adicionando o novo usuário ao banco de dados
            dbContext.Users.Add(user);
            dbContext.SaveChanges();
        }
    }
}

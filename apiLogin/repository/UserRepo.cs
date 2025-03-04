using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MinhaApiComMySQL.Data;
using Models;


namespace WebApplication1.repository
{
    public class UserRepo
    {
        public void Create(User user, AppDbContext dbContext)
        {
            dbContext.Users.Add(user);
            dbContext.SaveChanges();
        }
    }
}
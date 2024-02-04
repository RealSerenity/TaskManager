using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using TaskManager.Context;
using TaskManager.Data.Entities;

namespace TaskManager.Services.Impl
{
    public class UserServiceImpl : IUserService
    {
           private readonly ApplicationDbContext _context;
          private readonly IPasswordHasher _passwordHasher;

          public UserServiceImpl(ApplicationDbContext context, IPasswordHasher passwordHasher)
          {
             _passwordHasher = passwordHasher;
              _context = context;
          }
        
          public async Task<User> CreateUserAsync(User user)
          {
              // request içerinideki username'e ait bir kullanıcının varlığnı kontrol ediliyor
              var isExist = GetByUsername(user.Username) != null;

              if (isExist)
                  throw new Exception("There is already a user with the same username!!");
               // password encoding
              user.Password = _passwordHasher.HashPassword(user.Password);
              await _context.Users.AddAsync(user);
              await _context.SaveChangesAsync();
            return user;
          }

          public async Task<bool> DeleteAsync(int userId)
          {
              var user = await _context.Users.FindAsync(userId);
              if (user == null)
                  return false;

              _context.Users.Remove(user);
              return await _context.SaveChangesAsync() > 0;
          }

          public IEnumerable<User> GetAll()
          {
              return _context.Users.ToList();
          }
          public User GetById(int userId)
          {
              return _context.Users.Find(userId);
          }

          public User GetByUsername(String username)
          {
              return _context.Users.FirstOrDefault(u => u.Username == username);
          }

          public async Task<Tuple<bool, User>> UpdateAsync(int id, User user)
          {
            User oldUser = GetById(id);
            if (oldUser == null) throw new Exception("User not exist");
            oldUser.Username = user.Username;
             user.Password = _passwordHasher.HashPassword(user.Password);
              
            var updatedUser = _context.Users.Update(oldUser).Entity;
              return  Tuple.Create(await _context.SaveChangesAsync() > 0, updatedUser);
          }
    }
}

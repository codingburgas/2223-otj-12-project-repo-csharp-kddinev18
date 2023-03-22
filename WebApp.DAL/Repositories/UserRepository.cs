using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;
using WebApp.DAL.Data;
using WebApp.DAL.Models;
using WebApp.DAL.Repositories.Interfaces;
using WebApp.DTO;
using WebApp.DTO.Interfaces;

namespace WebApp.DAL.Repositories
{
    public class UserRepository : IUserRepository
    {
        private IOTHomeSecurityDbContext _context;
        public UserRepository() { }
        public UserRepository(IIOTHomeSecurityDbContext context)
        {
            _context = context as IOTHomeSecurityDbContext;
        }

        public async Task<bool> AddUserAsync(IRequestDataTransferObject user)
        {
            User newUser = new User(user);
            _context.Users.Add(newUser);
            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<bool> DeleteUserAsync(int userId)
        {
            User user = _context.Users.Where(user => user.Id == userId).FirstOrDefault();
            if (user == null)
            {
                throw new ArgumentException("Cannot find the user you have requested for deletion");
            }
            _context.Users.Remove(user);
            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<IEnumerable<IResponseDataTransferObject>> GetAsync(int pagingSize, int skipAmount)
        {
            ICollection<User> users = await _context.Users.Skip(skipAmount).Take(pagingSize).ToListAsync();

            ICollection<UserResponseDataTransferObject> userTransferObject = new List<UserResponseDataTransferObject>();
            foreach (User user in users)
            {
                userTransferObject.Add(new UserResponseDataTransferObject()
                {
                    Id = user.Id,
                    UserName = user.UserName,
                    Email = user.Email
                });
            }

            return userTransferObject;
        }

        public async Task<IResponseDataTransferObject> GetUserByIdAsync(int userId)
        {
            User user = await _context.Users.Where(user => user.Id == userId).FirstOrDefaultAsync();
            if (user == null)
            {
                throw new ArgumentException("Cannot find the user you have requested for deletion");
            }
            return new UserResponseDataTransferObject()
            {
                Id = user.Id,
                UserName = user.UserName,
                Email = user.Email
            };
        }

        public async Task<bool> UpdateUserAsync(int userId)
        {
            User user = await _context.Users.Where(user => user.Id == userId).FirstOrDefaultAsync();
            if (user == null)
            {
                throw new ArgumentException("Cannot find the user you have requested for deletion");
            }
            user.UserName = user.UserName;
            user.Email = user.Email;
            user.Password = user.Password;

            await _context.SaveChangesAsync();

            return true;
        }
    }
}
